using Unity.Entities;
using Unity.Mathematics;

namespace Features.Input
{
    /// <summary>
    /// Component that stores the world position of a click.
    /// Enabled only for the frame when click occurs.
    /// </summary>
    public struct ClickWorldPosition : IComponentData
    {
        public float3 Position;
        public bool HasPosition;
    }
}
