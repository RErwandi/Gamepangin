using System;

namespace Erwandi.Gamepangin.Patterns
{
    public interface IEventBus
    {
        void AddListener<T>(Action<T> listener) where T : IEvent;
        
        void RemoveListener<T>(Action<T> listener) where T : IEvent;
        
        void Trigger<T>(T @event) where T : IEvent;
    }
}