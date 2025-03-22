
using Unity.Entities;

namespace Features.Input
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    [UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
    public partial class InputSimalationSystemGroup : ComponentSystemGroup
    {
    }
}

