using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;

namespace Core.Physics
{
    /// <summary>
    /// Component that defines input parameters for a raycast operation.
    /// Attach this component to entities that need continuous raycast checking.
    /// Can be temporarily disabled using SetEnabled/IsEnabled.
    /// </summary>
    public struct Raycast : IRaycast, IEnableableComponent
    {
        /// <summary>The starting point of the ray in world space.</summary>
        public float3 Start;

        /// <summary>The end point of the ray in world space.</summary>
        public float3 End;

        /// <summary>Filter to determine which physics layers to interact with.</summary>
        public CollisionFilter Filter;

        /// <summary>
        /// Converts the Raycast component to a RaycastInput.
        /// </summary>
        /// <returns>A RaycastInput representing the raycast parameters.</returns>
        public RaycastInput ToRaycastInput()
        {
            return new RaycastInput
            {
                Start = Start,
                End = End,
                Filter = Filter
            };
        }
    }
}
