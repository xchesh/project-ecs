using System;

namespace Features.Lockstep.States
{
    public interface IStatesStore<TState> : IStatesStore, IDisposable where TState : IState
    {
        TState InitState { get; }
        TState LastState { get; }
        TState CurrentState { get; }

        event Action<TState> LastStateChanged;
        event Action<TState> CurrentStateChanged;

        TState GetState(uint step);

        void Execute<TCommand>(TCommand command, uint step) where TCommand : ICommand<TState>;
    }

    public interface IStatesStore
    {
        string Name { get; }

        IState GetStateAt(uint step);

        void RemoveAllAfter(uint step);
        void RemoveAllBefore(uint step);

        void Update(uint step);
        void Reset();
    }
}
