namespace Features.Lockstep.States
{
    public partial class StatesStore<TState> where TState : IState
    {
        private readonly struct CommandInitialization : ICommand<TState>
        {
            public override string ToString()
            {
                return "Initialization command";
            }

            public TState Execute(TState state)
            {
                return state;
            }
        }

        private readonly struct Snapshot
        {
            public readonly uint Step;
            public readonly ICommand<TState> Command;
            public readonly TState State;

            public Snapshot(uint step, ICommand<TState> command, TState state)
            {
                Step = step;
                Command = command;
                State = state;
            }

            public Snapshot WithState(TState newState)
            {
                return new Snapshot(Step, Command, newState);
            }

            public override string ToString()
            {
                return $"[{Step}] {Command.GetType().Name} {State}";
            }
        }
    }
}
