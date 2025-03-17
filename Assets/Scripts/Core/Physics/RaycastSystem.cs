using Core.Physics;
using GameSdk.Core.Loggers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[assembly: RegisterGenericComponentType(typeof(Raycast))]
[assembly: RegisterGenericComponentType(typeof(RaycastResult))]
[assembly: RegisterGenericSystemType(typeof(RaycastSystem<Raycast, RaycastResult>))]

namespace Core.Physics
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [BurstCompile]
    public partial struct RaycastSystem<TRaycast, TRaycastResult> : ISystem
        where TRaycast : unmanaged, IRaycast
        where TRaycastResult : unmanaged, IRaycastResult<TRaycastResult>
    {
        public const string TAG = "RaycastSystem";
        public const int BATCH_SIZE = 64;

        private EntityQuery _raycastQuery;

        [BurstCompile]
        private struct PrepareRaycastInputJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Entity> Entities; // Передаем Entity явно
            [ReadOnly] public ComponentLookup<TRaycast> Raycasts;
            public NativeArray<RaycastInput> Inputs;

            public void Execute(int index)
            {
                var entity = Entities[index];
                if (Raycasts.HasComponent(entity))
                {
                    Inputs[index] = Raycasts[entity].ToRaycastInput();
                }
            }
        }

        [BurstCompile]
        private struct ProcessRaycastResultsJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Entity> Entities;
            [ReadOnly] public NativeArray<RaycastHit> ResultHits;
            [ReadOnly] public ComponentLookup<TRaycastResult> RaycastResults;
            public EntityCommandBuffer.ParallelWriter Ecb;

            public void Execute(int index)
            {
                var entity = Entities[index];
                var hit = ResultHits[index];
                var result = default(TRaycastResult);
                var newResult = result.ToRaycastResult(hit);

                if (RaycastResults.HasComponent(entity))
                {
                    Ecb.SetComponent(index, entity, newResult);
                }
                else
                {
                    Ecb.AddComponent(index, entity, newResult);
                }

                Ecb.RemoveComponent<TRaycast>(index, entity);
            }
        }

        public void OnCreate(ref SystemState state)
        {
            _raycastQuery = state.GetEntityQuery(ComponentType.ReadOnly<TRaycast>());

            state.RequireForUpdate(_raycastQuery);
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var raycastCount = _raycastQuery.CalculateEntityCount();

            if (raycastCount == 0) return;

            SystemLog.Log(TAG, $"Processing {raycastCount} raycasts");

            var inputs = new NativeArray<RaycastInput>(raycastCount, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(raycastCount, Allocator.TempJob);
            var entities = _raycastQuery.ToEntityArray(Allocator.TempJob);

            var prepareJob = new PrepareRaycastInputJob
            {
                Entities = entities, // Передаем массив Entity
                Raycasts = SystemAPI.GetComponentLookup<TRaycast>(true),
                Inputs = inputs
            };

            var prepareHandle = prepareJob.Schedule(raycastCount, BATCH_SIZE, state.Dependency);

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var raycastHandle = RaycastUtils.ScheduleBatchRayCast(physicsWorld, inputs, results, prepareHandle);

            var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var processJob = new ProcessRaycastResultsJob
            {
                Entities = entities,
                ResultHits = results,
                RaycastResults = SystemAPI.GetComponentLookup<TRaycastResult>(true),
                Ecb = ecb,
            };

            state.Dependency = processJob.Schedule(raycastCount, BATCH_SIZE, raycastHandle);

            state.Dependency = JobHandle.CombineDependencies(
                inputs.Dispose(state.Dependency),
                results.Dispose(state.Dependency),
                entities.Dispose(state.Dependency)
            );
        }
    }
}
