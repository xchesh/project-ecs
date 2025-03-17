using Unity.Entities;
using Unity.Physics;

namespace Core.Physics
{
    /// <summary>
    /// Interface for components that provide raycast input data.
    /// Implementing components must be unmanaged types.
    /// </summary>
    public interface IRaycast : IComponentData
    {
        /// <summary>
        /// Converts the component data to raycast input.
        /// </summary>
        RaycastInput ToRaycastInput();
    }
}
