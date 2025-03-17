using Unity.Entities;
using Unity.Mathematics;

namespace Features.Movement
{
    /// <summary>
    /// Component that represents an agent capable of pathfinding using Unity's NavMesh system.
    /// Contains essential data for navigation and path following.
    /// </summary>
    public struct NavAgent : IComponentData
    {
        /// <summary>Target position the agent should move towards</summary>
        public float3 Destination;

        /// <summary>Movement speed of the agent</summary>
        public float Speed;

        /// <summary>Indicates whether the agent currently has an active path to follow</summary>
        public bool HasPath;

        /// <summary>Maximum distance to search for nearest point on NavMesh</summary>
        public float MaxProjectionDistance;
        // Preset values for different use cases
        public const float PRECISE_PROJECTION = 1f;     // For precise positioning and small objects
        public const float STANDARD_PROJECTION = 3f;    // Good balance for most cases
        public const float FLEXIBLE_PROJECTION = 5f;    // More flexible but might be less precise
        public const float LARGE_WORLD_PROJECTION = 100f; // For map clicks and large distances
    }
}
