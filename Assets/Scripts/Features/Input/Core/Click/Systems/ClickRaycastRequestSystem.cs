using Core.Physics;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace Features.Input
{
    /// <summary>
    /// System that converts click input into raycast requests.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(ClickInputSystem))]
    [BurstCompile]
    public partial struct ClickRaycastRequestSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ClickInput>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (clickInput, entity) in SystemAPI.Query<RefRW<ClickInput>>().WithEntityAccess())
            {
                if (!clickInput.ValueRO.HasClick) continue;

                // Create raycast request
                ecb.AddComponent(entity, new Raycast
                {
                    Start = clickInput.ValueRO.ClickRay.origin,
                    End = clickInput.ValueRO.ClickRay.origin + clickInput.ValueRO.ClickRay.direction * 100f, // 100 units range
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u, // All layers
                        CollidesWith = ~0u, // All layers
                        GroupIndex = 0
                    }
                });
            }
        }
    }
}
