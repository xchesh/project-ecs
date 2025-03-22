using GameSdk.Core.Loggers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using Core.Debug;

namespace Features.Movement
{
    /// <summary>
    /// System responsible for synchronizing NavAgent movement with Unity's NavMesh system.
    /// Handles pathfinding and movement of entities with NavAgent components.
    /// </summary>
    [UpdateInGroup(typeof(MovementSimalationSystemGroup))]
    public partial class NavAgentSyncSystem : SystemBase
    {
        public const string TAG = "NavAgentSyncSystem";

        /// <summary>
        /// Cached NavMeshPath instance used for pathfinding calculations.
        /// Reused to avoid garbage collection overhead.
        /// </summary>
        private NavMeshPath _path;

#if UNITY_EDITOR
        private static readonly Color ActiveSegmentColor = new(0f, 1f, 0f, 1f);    // Bright green for current segment
        private static readonly Color FutureSegmentColor = new(0f, 0.5f, 0f, 0.5f); // Semi-transparent green for future segments
        private const float PATH_DISPLAY_TIME = 0f; // 0 means update every frame

        private void DrawPath(NavMeshPath path)
        {
            if (path.corners.Length <= 1) return;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                var start = path.corners[i];
                var end = path.corners[i + 1];

                // Use different colors for current segment and future segments
                var color = i == 0 ? ActiveSegmentColor : FutureSegmentColor;
                Debug.DrawLine(start, end, color, PATH_DISPLAY_TIME);
            }
        }

        private void DrawTargetPosition(float3 targetPosition)
        {
            DebugDrawer.DrawWireSphere(targetPosition, 0.1f, Color.red, PATH_DISPLAY_TIME);
        }
#endif

        protected override void OnCreate()
        {
            _path = new NavMeshPath();
        }

        protected override void OnUpdate()
        {
            foreach (var (agent, transform) in SystemAPI.Query<RefRW<NavAgent>, RefRW<LocalTransform>>())
            {
                if (agent.ValueRW.HasPath is false)
                {
                    continue;
                }

                var startPosition = transform.ValueRW.Position;
                var targetPosition = agent.ValueRW.Destination;

                // Check if target point is on the NavMesh
                NavMeshHit targetHit;
                var targetOnMesh = NavMesh.SamplePosition(targetPosition, out targetHit, agent.ValueRO.MaxProjectionDistance, NavMesh.AllAreas);

                if (!targetOnMesh)
                {
                    SystemLog.LogWarning(TAG, $"Target point is off the NavMesh! Position: {targetPosition} (max projection distance: {agent.ValueRO.MaxProjectionDistance})");
                    agent.ValueRW.HasPath = false;
                    continue;
                }

                // Calculate path using projected target point
                var isCalculated = NavMesh.CalculatePath(startPosition, targetHit.position, NavMesh.AllAreas, _path);

                if (!isCalculated)
                {
                    SystemLog.LogWarning(TAG, $"Failed to calculate path to {targetHit.position}");
                    agent.ValueRW.HasPath = false;
                    continue;
                }

                if (_path.corners.Length > 1)
                {
#if UNITY_EDITOR
                    DrawPath(_path);
                    DrawTargetPosition(targetPosition);
#endif

                    // Move towards the next corner in the calculated path
                    var nextCorner = _path.corners[1];
                    var speed = agent.ValueRW.Speed;
                    transform.ValueRW.Position = Vector3.MoveTowards(startPosition, nextCorner, speed * SystemAPI.Time.DeltaTime);
                }
                else
                {
                    // If path calculation failed, disable path following
                    agent.ValueRW.HasPath = false;
                }
            }
        }
    }
}
