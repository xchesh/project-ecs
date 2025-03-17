namespace Features.Lockstep.States
{
    public interface ICommand<T> where T : IState
    {
        T Execute(T state);
    }
}