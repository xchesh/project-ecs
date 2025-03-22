using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Input
{
    /// <summary>
    /// Component that stores click input data for cancelled clicks.
    /// </summary>
    public struct ClickCancelInput : IComponentData, IEnableableComponent
    {
        /// <summary>
        /// The ray from camera through the position where the click was cancelled.
        /// </summary>
        public Ray CancelRay;

        /// <summary>
        /// The position in world space where the click was cancelled.
        /// </summary>
        public float3 CancelPosition;

        /// <summary>
        /// How long the click was held before being cancelled in seconds.
        /// </summary>
        public float HoldDuration;
    }
}
