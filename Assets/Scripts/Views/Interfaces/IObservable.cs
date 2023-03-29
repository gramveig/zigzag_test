namespace Alexey.ZigzagTest.Views
{
    public interface IObservable<T>
    {
        void Unsubscribe(IObserver<T> observer);
    }
}