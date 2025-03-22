using Core.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Features.Input
{
    /// <summary>
    /// Component that stores the results of a raycast operation.
    /// This component is automatically updated by the RaycastSystem.
    /// </summary>
    public struct ClickRaycastResult : IRaycastResult<ClickRaycastResult>
    {
        /// <summary>The entity that was hit by the ray, or Entity.Null if no hit occurred.</summary>
        public Entity HitEntity;

        /// <summary>The point in world space where the ray hit a collider.</summary>
        public float3 HitPosition;

        /// <summary>Whether the ray hit anything within its range.</summary>
        public bool HasHit;

        /// <summary>
        /// Converts the RaycastHit to a RaycastResult.
        /// </summary>
        /// <param name="hit">The RaycastHit to convert.</param>
        /// <returns>A new RaycastResult instance.</returns>
        public ClickRaycastResult ToRaycastResult(RaycastHit hit)
        {
            return new ClickRaycastResult
            {
                HitEntity = hit.Entity,
                HitPosition = hit.Position,
                HasHit = hit.Entity != Entity.Null
            };
        }
    }
}
