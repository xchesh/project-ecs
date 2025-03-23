using Core.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Features.Input
{
    public struct ClickRaycast : IRaycast, IEnableableComponent
    {
        /// <summary>The ray from camera through click position.</summary>
        public UnityEngine.Ray Ray;

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
