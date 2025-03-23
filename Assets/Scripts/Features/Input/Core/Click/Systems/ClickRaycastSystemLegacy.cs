using Unity.Entities;
using UnityEngine;
using Core.Physics;
using Unity.Burst;
using GameSdk.Core.Loggers;
using Unity.Mathematics;

namespace Features.Input
{
    [UpdateInGroup(typeof(RaycastsSimulationSystemGroup))]
    [UpdateAfter(typeof(ClickRaycastRequestSystem))]
    [UpdateBefore(typeof(ClickRaycastProcessSystem))]
    [BurstCompile]
    public partial class ClickRaycastSystemLegacy : SystemBase
    {
        public const string TAG = "ClickRaycastSystemLegacy";

        [BurstCompile]
        protected override void OnUpdate()
        {
            foreach (var (clickRaycast, raycastResult, entity) in SystemAPI.Query<RefRW<ClickRaycast>, RefRW<ClickRaycastResult>>().WithPresent<ClickRaycastResult>().WithEntityAccess())
            {
                SystemLog.Log(TAG, $"Processing  legacy raycast");

                var hasHit = Physics.Raycast(clickRaycast.ValueRO.Ray, out var hit);

                raycastResult.ValueRW.HasHit = hasHit;
                raycastResult.ValueRW.HitEntity = Entity.Null;
                raycastResult.ValueRW.HitPosition = hasHit ? hit.point : float3.zero;

                SystemAPI.SetComponentEnabled<ClickRaycast>(entity, false);
                SystemAPI.SetComponentEnabled<ClickRaycastResult>(entity, true);
            }
        }
    }
}
