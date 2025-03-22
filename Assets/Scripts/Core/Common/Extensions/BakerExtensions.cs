using Unity.Entities;

namespace Core.Common.Extensions
{
    public static class BakerExtensions
    {
        public static void AddComponent<T>(this IBaker baker, Entity entity, bool enabled) where T : unmanaged, IComponentData, IEnableableComponent
        {
            baker.AddComponent<T>(entity);
            baker.SetComponentEnabled<T>(entity, enabled);
        }
    }
}
