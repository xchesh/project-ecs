using Core.Physics;
using Unity.Burst;
using Unity.Entities;
using GameSdk.Core.Loggers;

namespace Features.Input
{
    /// <summary>
    /// System that processes raycast results and creates click world position components.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RaycastSystem<Raycast, RaycastResult>))]
    [BurstCompile]
    public partial struct ClickRaycastProcessSystem : ISystem
    {
        public const string TAG = "ClickRaycastProcessSystem";

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RaycastResult>();
            state.RequireForUpdate<ClickInput>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Process raycast results
            foreach (var (_, raycastResult, entity) in SystemAPI.Query<RefRO<ClickInput>, RefRO<RaycastResult>>().WithEntityAccess())
            {
                var clickWorldPosition = new ClickWorldPosition
                {
                    Position = raycastResult.ValueRO.HitPosition,
                    HasPosition = raycastResult.ValueRO.HasHit
                };

                var clickPositions = SystemAPI.GetComponentLookup<ClickWorldPosition>();

                if (clickPositions.HasComponent(entity))
                {
                    ecb.SetComponent(entity, clickWorldPosition);
                }
                else
                {
                    ecb.AddComponent(entity, clickWorldPosition);
                }

                ecb.RemoveComponent<RaycastResult>(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
