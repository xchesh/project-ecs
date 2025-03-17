using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Input
{
    /// <summary>
    /// Component that stores click input data.
    /// </summary>
    public struct ClickInput : IComponentData
    {
        /// <summary>
        /// The ray from camera through click position.
        /// </summary>
        public Ray ClickRay;

        /// <summary>
        /// The position in world space where the click occurred.
        /// </summary>
        public float3 ClickPosition;

        /// <summary>
        /// Whether a click has occurred.
        /// </summary>
        public bool HasClick;
    }
}
