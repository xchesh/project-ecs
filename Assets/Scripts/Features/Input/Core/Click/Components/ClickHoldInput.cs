using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Input
{
    /// <summary>
    /// Component that stores click input data for held clicks.
    /// </summary>
    public struct ClickHoldInput : IComponentData, IEnableableComponent
    {
        /// <summary>
        /// The ray from camera through current mouse position.
        /// </summary>
        public Ray CurrentRay;

        /// <summary>
        /// The current mouse position in world space.
        /// </summary>
        public float3 CurrentPosition;

        /// <summary>
        /// How long the click has been held in seconds.
        /// </summary>
        public float HoldDuration;
    }
}
