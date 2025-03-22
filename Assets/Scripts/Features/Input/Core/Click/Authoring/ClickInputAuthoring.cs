using Unity.Entities;
using UnityEngine;

namespace Features.Input
{
    public class ClickInputAuthoring : MonoBehaviour
    {
        public class Baker : Baker<ClickInputAuthoring>
        {
            public override void Bake(ClickInputAuthoring authoring)
            {
                var entity = GetEntityWithoutDependency();

                AddComponent<ClickInput>(entity);
                SetComponentEnabled<ClickInput>(entity, false);

                AddComponent<ClickPerformInput>(entity);
                SetComponentEnabled<ClickPerformInput>(entity, false);

                AddComponent<ClickHoldInput>(entity);
                SetComponentEnabled<ClickHoldInput>(entity, false);

                AddComponent<ClickCancelInput>(entity);
                SetComponentEnabled<ClickCancelInput>(entity, false);

                AddComponent<ClickInputWorldPosition>(entity);
                SetComponentEnabled<ClickInputWorldPosition>(entity, false);
            }
        }
    }
}
