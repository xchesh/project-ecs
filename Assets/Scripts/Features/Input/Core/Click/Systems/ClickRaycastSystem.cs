using Core.Physics;
using Unity.Entities;
using Features.Input;

[assembly: RegisterGenericComponentType(typeof(ClickRaycast))]
[assembly: RegisterGenericComponentType(typeof(ClickRaycastResult))]
[assembly: RegisterGenericSystemType(typeof(RaycastsSystem<ClickRaycast, ClickRaycastResult>))]
