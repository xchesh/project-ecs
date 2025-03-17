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
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ClickRaycastProcessSystem))]
    public partial struct PlayerClickMovementSystem : ISystem
    {
        public const string TAG = "PlayerClickMovementSystem";

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ClickWorldPosition>();
            state.RequireForUpdate<NavAgent>();
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (clickPos, clickInput, entity) in SystemAPI.Query<RefRO<ClickWorldPosition>, RefRO<ClickInput>>().WithEntityAccess())
            {
                if (!clickInput.ValueRO.HasClick)
                {
                    continue;
                }

                foreach (var (agent, _) in SystemAPI.Query<RefRW<NavAgent>, RefRO<PlayerTag>>())
                {
                    agent.ValueRW.MaxProjectionDistance = NavAgent.PRECISE_PROJECTION;
                    agent.ValueRW.Destination = clickPos.ValueRO.Position;
                    agent.ValueRW.HasPath = clickPos.ValueRO.HasPosition;
                }
            }
        }
    }
}
