using Core.Physics;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace Features.Input
{
    /// <summary>
    /// System that converts click input into raycast requests.
    /// </summary>
    [UpdateInGroup(typeof(RaycastsSimulationSystemGroup))]
    [UpdateBefore(typeof(RaycastsSystem<ClickRaycast, ClickRaycastResult>))]
    [BurstCompile]
    public partial struct ClickRaycastRequestSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ClickInput>();
            state.RequireForUpdate<ClickRaycast>();
            state.RequireForUpdate<ClickRaycastResult>();
            state.RequireForUpdate<ClickInputWorldPosition>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var raycastLookup = SystemAPI.GetComponentLookup<ClickRaycast>();

            foreach (var (clickInput, entity) in SystemAPI.Query<RefRW<ClickInput>>().WithAll<ClickInput>().WithEntityAccess())
            {
                var raycast = new ClickRaycast
                {
                    Start = clickInput.ValueRO.Ray.origin,
                    End = clickInput.ValueRO.Ray.origin + clickInput.ValueRO.Ray.direction * 100f, // 100 units range
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u, // All layers
                        CollidesWith = ~0u, // All layers
                        GroupIndex = 0
                    }
                };

                if (!raycastLookup.HasComponent(entity))
                {
                    state.EntityManager.AddComponent<ClickRaycast>(entity);
                }

                raycastLookup[entity] = raycast;
                SystemAPI.SetComponentEnabled<ClickRaycast>(entity, true);
                SystemAPI.SetComponentEnabled<ClickInput>(entity, false);
            }
        }
    }
}
