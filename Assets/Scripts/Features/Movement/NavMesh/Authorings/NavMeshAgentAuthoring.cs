using Unity.Entities;
using UnityEngine;

namespace Features.Movement
{
    /// <summary>
    /// MonoBehaviour component used in the Unity Editor to configure NavAgent properties.
    /// Converts traditional Unity components into ECS components during baking process.
    /// </summary>
    public class NavMeshAgentAuthoring : MonoBehaviour
    {
        /// <summary>Default movement speed for the agent</summary>
        [SerializeField] private float _speed = 3.5f;

        /// <summary>
        /// Baker class responsible for converting NavMeshAgentAuthoring into ECS components
        /// during the baking process.
        /// </summary>
        public class Baker : Baker<NavMeshAgentAuthoring>
        {
            public override void Bake(NavMeshAgentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                // Add NavAgent component with initial configuration
                AddComponent(entity, new NavAgent
                {
                    Speed = authoring._speed,
                    HasPath = false
                });
            }
        }
    }
}
