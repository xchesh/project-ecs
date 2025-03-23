using Unity.Entities;
using Unity.Mathematics;

namespace Features.Input
{
    /// <summary>
    /// Component that stores movement input data.
    /// Can be used by any entity that needs movement direction input,
    /// regardless of the input source (player, AI, network, etc.)
    /// </summary>
    public struct MovementInput : IComponentData, IEnableableComponent
    {
        /// <summary>Normalized direction vector for movement</summary>
        public float2 Direction;

        /// <summary>Indicates if the movement input is active</summary>
        public bool IsActive;
    }
}

