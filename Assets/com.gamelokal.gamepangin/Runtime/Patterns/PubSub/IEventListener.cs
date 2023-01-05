namespace Gamepangin
{
    public interface IEventListener<T> : IEventListenerBase
    {
        void OnEvent(T e);
    }
}