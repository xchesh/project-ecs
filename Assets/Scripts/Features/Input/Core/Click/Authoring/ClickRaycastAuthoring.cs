using Core.Physics;
using Unity.Entities;
using UnityEngine;

namespace Features.Input
{
    public class ClickRaycastAuthoring : MonoBehaviour
    {
        public class Baker : Baker<ClickRaycastAuthoring>
        {
            public override void Bake(ClickRaycastAuthoring authoring)
            {
                var entity = GetEntityWithoutDependency();

                AddComponent<ClickRaycast>(entity);
                SetComponentEnabled<ClickRaycast>(entity, true);

                AddComponent<ClickRaycastResult>(entity);
                SetComponentEnabled<ClickRaycastResult>(entity, false);
            }
        }
    }
}
