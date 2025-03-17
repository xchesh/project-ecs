namespace Features.Lockstep
{
    public interface ILockstepSimulation
    {
        int Order { get; }

        void Simulate(uint step);
    }
}
