using GameSdk.Core.Loggers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

namespace Core.Physics
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(RaycastsSimulationSystemGroup))]
    [BurstCompile]
    public partial struct RaycastsSystem<TRaycast, TRaycastResult> : ISystem
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

                Ecb.SetComponent(index, entity, newResult);
                Ecb.SetComponentEnabled<TRaycast>(index, entity, false);
                Ecb.SetComponentEnabled<TRaycastResult>(index, entity, true);
            }
        }

        public void OnCreate(ref SystemState state)
        {
            _raycastQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<TRaycast>()
                .WithPresent<TRaycastResult>()
                .Build(ref state);

            state.RequireForUpdate(_raycastQuery);
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<RaycastsCommandBufferSystem.Singleton>();
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
            var raycastHandle = RaycastsUtils.ScheduleBatchRayCast(physicsWorld, inputs, results, prepareHandle);

            var ecbSystem = SystemAPI.GetSingleton<RaycastsCommandBufferSystem.Singleton>();
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
