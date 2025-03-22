using Unity.Burst;
using Unity.Entities;
using Features.Input;
using GameSdk.Core.Loggers;

namespace Features.Movement.Player
{
    /// <summary>
    /// System that handles player movement based on click position.
    /// Converts click world position into NavAgent destination updates.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(MovementSimalationSystemGroup))]
    [UpdateAfter(typeof(PlayerMovementSystem))]
    [UpdateBefore(typeof(NavAgentSyncSystem))]
    public partial struct PlayerClickMovementSystem : ISystem
    {
        public const string TAG = "PlayerClickMovementSystem";

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ClickInputWorldPosition>();
            state.RequireForUpdate<NavAgent>();
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (clickPos, agent, _, entity) in SystemAPI.Query<RefRO<ClickInputWorldPosition>, RefRW<NavAgent>, RefRO<PlayerTag>>().WithEntityAccess())
            {
                agent.ValueRW.MaxProjectionDistance = NavAgent.PRECISE_PROJECTION;
                agent.ValueRW.Destination = clickPos.ValueRO.Position;
                agent.ValueRW.HasPath = clickPos.ValueRO.HasPosition;

                SystemLog.Log(TAG, $"PlayerClickMovementSystem: {entity}");

                SystemAPI.SetComponentEnabled<ClickInputWorldPosition>(entity, false);
            }
        }
    }
}
