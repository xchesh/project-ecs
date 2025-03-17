using System;
using System.Collections.Generic;

namespace Features.Lockstep.States
{
    public partial class StatesStore<TState> : IStatesStore<TState> where TState : IState
    {
        private Snapshot CurrentSnapshot { get; set; }
        private LinkedList<Snapshot> StateSnapshots { get; } = new();

        public TState InitState { get; }
        public TState LastState { get; private set; }
        public TState CurrentState { get; private set; }

        public event Action<TState> LastStateChanged;
        public event Action<TState> CurrentStateChanged;

        public string Name => typeof(TState).Name;

        public StatesStore(TState initState)
        {
            if (initState == null)
                throw new ArgumentNullException(nameof(initState));

            InitState = initState;
            LastState = initState;
            CurrentState = initState;

            var initialSnapshot = new Snapshot(0, new CommandInitialization(), initState);
            StateSnapshots.AddLast(initialSnapshot);
            CurrentSnapshot = initialSnapshot;
        }

        public virtual TState GetState(uint step)
        {
            var last = FindLastSnapshotBefore(step);

            if (last != null)
            {
                return last.Value.State;
            }

            return InitState;
        }

        public virtual IState GetStateAt(uint step)
        {
            return GetState(step);
        }

        public virtual void Execute<TCommand>(TCommand command, uint step) where TCommand : ICommand<TState>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var last = FindLastSnapshotBefore(step);
            var state = command.Execute(last.Value.State);

            var current = StateSnapshots.AddAfter(last, new Snapshot(step, command, state));

            while (current.Next != null)
            {
                current.Next.Value = current.Next.Value.WithState(current.Next.Value.Command.Execute(current.Value.State));
                current = current.Next;
            }

            LastState = current.Value.State;
            LastStateChanged?.Invoke(LastState);
        }

        public virtual void Update(uint step)
        {
            var last = FindLastSnapshotBefore(step);

            if (last == null || last.Value.Step == CurrentSnapshot.Step || last.Value.Equals(CurrentSnapshot))
            {
                return;
            }

            CurrentSnapshot = last.Value;
            CurrentState = last.Value.State;
            CurrentStateChanged?.Invoke(CurrentState);
        }

        public virtual void Reset()
        {
            StateSnapshots.Clear();
            StateSnapshots.AddLast(new Snapshot(0, new CommandInitialization(), InitState));

            LastState = InitState;
            CurrentState = InitState;

            LastStateChanged?.Invoke(LastState);
            CurrentStateChanged?.Invoke(CurrentState);
        }

        public virtual void RemoveAllAfter(uint step)
        {
            var node = StateSnapshots.Last;

            while (node != null && node.Value.Step > step)
            {
                var nextNode = node.Previous;

                StateSnapshots.Remove(node);

                node = nextNode;
            }
        }

        public virtual void RemoveAllBefore(uint step)
        {
            var node = StateSnapshots.First;

            while (node != null && node.Value.Step < step)
            {
                var nextNode = node.Next;

                if (nextNode != null)
                {
                    StateSnapshots.Remove(node);
                }

                node = nextNode;
            }
        }

        public virtual void Dispose()
        {
            StateSnapshots.Clear();
        }

        private LinkedListNode<Snapshot> FindLastSnapshotBefore(uint step)
        {
            var node = StateSnapshots.Last;

            while (node != null && node.Value.Step > step)
            {
                node = node.Previous;
            }

            return node;
        }
    }
}
