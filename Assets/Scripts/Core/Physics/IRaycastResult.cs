using Unity.Entities;
using Unity.Physics;

namespace Core.Physics
{
    /// <summary>
    /// Interface for components that can store raycast results.
    /// Implementing components must be unmanaged types.
    /// </summary>
    public interface IRaycastResult<T> : IComponentData, IEnableableComponent where T : unmanaged, IRaycastResult<T>
    {
        /// <summary>
        /// Creates a new instance of raycast result from hit data.
        /// </summary>
        /// <param name="hit">The raycast hit data to process.</param>
        /// <returns>New instance of raycast result.</returns>
        T ToRaycastResult(RaycastHit hit);
    }
}
