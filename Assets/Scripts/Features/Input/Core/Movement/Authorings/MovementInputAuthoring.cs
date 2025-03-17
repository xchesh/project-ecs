using Unity.Entities;
using UnityEngine;

namespace Features.Input
{
    /// <summary>
    /// Authoring component for adding movement input capability to an entity.
    /// This component is used in the Unity Editor to mark entities that should receive movement input.
    /// The actual input can come from any source: player controls, AI, network, etc.
    /// </summary>
    public class MovementInputAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class responsible for converting MovementInputAuthoring into ECS components
        /// during the baking process.
        /// </summary>
        public class Baker : Baker<MovementInputAuthoring>
        {
            public override void Bake(MovementInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                // Add MovementInput component to the entity
                AddComponent<MovementInput>(entity);
            }
        }
    }
}
