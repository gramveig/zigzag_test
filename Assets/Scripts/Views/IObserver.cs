namespace Alexey.ZigzagTest.Views
{
    public interface IObserver<T>
    {
        IObserver<T> Subscribe(IObservable<T> observable);
        void Notify(T value);
    }
}