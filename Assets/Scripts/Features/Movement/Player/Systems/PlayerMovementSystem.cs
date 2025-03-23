using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Features.Input;
using GameSdk.Core.Loggers;
using UnityEngine;

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
        private Camera _mainCamera;

        protected override void OnStartRunning()
        {
            _mainCamera = Camera.main;
        }

        protected override void OnUpdate()
        {
            foreach (var (_, input, agent, transform) in SystemAPI.Query<RefRO<PlayerTag>, RefRO<MovementInput>, RefRW<NavAgent>, RefRO<LocalTransform>>())
            {
                // Get raw input direction (x: left/right, y: forward/backward)
                var direction = input.ValueRO.Direction;

                // Create rotation quaternion from camera's Y-axis rotation only
                // This ignores camera tilt (X rotation) and roll (Z rotation)
                // to ensure movement stays on the horizontal plane
                var cameraRotationY = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0);

                // Transform input direction to world space relative to camera orientation
                // Convert 2D input (x,y) into 3D space (x,0,z) and rotate it by camera's Y rotation
                var worldDirection = cameraRotationY * new Vector3(direction.x, 0, direction.y);

                // Get current position and calculate new target position
                // by adding the transformed direction to current position
                var currentPos = transform.ValueRO.Position;
                var targetPos = new float3(
                    currentPos.x + worldDirection.x,  // Apply X movement
                    currentPos.y,                     // Keep same Y position (no vertical movement)
                    currentPos.z + worldDirection.z   // Apply Z movement
                );

                // Update NavAgent state
                agent.ValueRW.MaxProjectionDistance = NavAgent.PRECISE_PROJECTION;
                agent.ValueRW.Destination = targetPos;
                agent.ValueRW.HasPath = input.ValueRO.IsActive;
            }
        }
    }
}
