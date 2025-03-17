using Unity.Entities;
using UnityEngine;
using Features.Input;
using Unity.Mathematics;

namespace Features.Movement.Player
{
    /// <summary>
    /// Authoring component for setting up a player entity with all required components.
    /// Adds MovementInput, ClickInput, NavAgent and PlayerTag components during conversion.
    /// </summary>
    public class PlayerAuthoring : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;

        public class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var position = authoring.transform.position;
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                // Add player tag
                AddComponent<PlayerTag>(entity);

                // Add input components
                AddComponent<MovementInput>(entity);
                AddComponent<ClickInput>(entity);

                // Add nav agent component with configuration
                AddComponent(entity, new NavAgent
                {
                    Speed = authoring._speed,
                    Destination = new float3(position.x, position.y, position.z),
                    HasPath = false,
                    MaxProjectionDistance = NavAgent.STANDARD_PROJECTION
                });
            }
        }
    }
}
