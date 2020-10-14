using System;

namespace Erwandi.Gamepangin.Patterns
{
    public sealed class EventBus : IEventBus
    {
        public void AddListener<T>(Action<T> listener) where T : IEvent
        {
            EventBusDispatcher<T>.AddListener(this, listener);
        }

        public void RemoveListener<T>(Action<T> listener) where T : IEvent
        {
            EventBusDispatcher<T>.RemoveListener(this, listener);
        }

        public void Trigger<T>(T @event) where T : IEvent
        {
            EventBusDispatcher<T>.Trigger(this, @event);
        }
    }
}