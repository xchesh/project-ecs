using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Features.Input;
using GameSdk.Core.Loggers;

namespace Features.Movement.Player
{
    /// <summary>
    /// System that handles player movement based on input.
    /// Converts movement input into NavAgent destination updates.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(MovementSimalationSystemGroup))]
    [UpdateBefore(typeof(NavAgentSyncSystem))]
    public partial class PlayerMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (_, input, agent, transform) in SystemAPI.Query<RefRO<PlayerTag>, RefRO<MovementInput>, RefRW<NavAgent>, RefRO<LocalTransform>>())
            {
                var direction = input.ValueRO.Direction;

                // Calculate target position based on current position and input direction
                var currentPos = transform.ValueRO.Position;
                var targetPos = new float3(
                    currentPos.x + direction.x,
                    currentPos.y,
                    currentPos.z + direction.y
                );

                // Update NavAgent state
                agent.ValueRW.MaxProjectionDistance = NavAgent.PRECISE_PROJECTION;
                agent.ValueRW.Destination = targetPos;
                agent.ValueRW.HasPath = input.ValueRO.IsActive;
            }
        }
    }
}
