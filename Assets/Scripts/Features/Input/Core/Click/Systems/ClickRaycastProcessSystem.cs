using Core.Physics;
using GameSdk.Core.Loggers;
using Unity.Burst;
using Unity.Entities;

namespace Features.Input
{
    /// <summary>
    /// System that processes raycast results and creates click world position components.
    /// </summary>
    [UpdateInGroup(typeof(RaycastsSimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(RaycastsSystem<ClickRaycast, ClickRaycastResult>))]
    [UpdateAfter(typeof(RaycastsCommandBufferSystem))]
    [BurstCompile]
    public partial struct ClickRaycastProcessSystem : ISystem
    {
        public const string TAG = "ClickRaycastProcessSystem";

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ClickInput>();
            state.RequireForUpdate<ClickRaycastResult>();
            state.RequireForUpdate<ClickInputWorldPosition>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var clickPositionsLookup = SystemAPI.GetComponentLookup<ClickInputWorldPosition>();

            foreach (var (raycastResult, entity) in SystemAPI.Query<RefRO<ClickRaycastResult>>().WithEntityAccess())
            {
                SystemLog.Log(TAG, $"Click raycast processed = {raycastResult.ValueRO.HasHit} | {raycastResult.ValueRO.HitPosition}");

                var clickWorldPosition = new ClickInputWorldPosition
                {
                    Position = raycastResult.ValueRO.HitPosition,
                    HasPosition = raycastResult.ValueRO.HasHit
                };

                if (!clickPositionsLookup.HasComponent(entity))
                {
                    state.EntityManager.AddComponent<ClickInputWorldPosition>(entity);
                }

                clickPositionsLookup[entity] = clickWorldPosition;
                SystemAPI.SetComponentEnabled<ClickInputWorldPosition>(entity, true);
                SystemAPI.SetComponentEnabled<ClickRaycastResult>(entity, false);
            }
        }
    }
}
