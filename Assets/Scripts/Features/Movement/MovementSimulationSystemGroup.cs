
using Core.Physics;
using Unity.Entities;
using Unity.Transforms;

namespace Features.Movement
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RaycastsSimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial class MovementSimalationSystemGroup : ComponentSystemGroup
    {
    }
}

