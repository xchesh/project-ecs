using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Input
{
    /// <summary>
    /// Component that stores click input data for the initial click.
    /// </summary>
    public struct ClickInput : IComponentData, IEnableableComponent
    {
        /// <summary>
        /// The ray from camera through click position.
        /// </summary>
        public Ray Ray;

        /// <summary>
        /// The position in world space where the click occurred.
        /// </summary>
        public float3 Position;
    }
}
