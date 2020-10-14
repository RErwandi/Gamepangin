using System;

namespace Erwandi.Gamepangin.Patterns
{
    /// <summary>
    /// Static generic class for event messaging implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class EventMessaging<T> where T : IEvent
    {
        private static Action<T> _action = delegate { };

        /// <summary>
        /// Subscribes listener to a certain event type.
        /// </summary>
        /// <param name="listener">Listener instance.</param>
        public static void AddListener(Action<T> listener)
        {
            _action += listener;
        }

        /// <summary>
        /// Unsubscribes listener to a certain event type.
        /// </summary>
        /// <param name="listener">Listener instance.</param>
        public static void RemoveListener(Action<T> listener)
        {
            _action -= listener;
        }

        /// <summary>
        /// Trigger an event.
        /// </summary>
        /// <param name="event">An event instance to trigger.</param>
        public static void Trigger(T @event)
        {
            _action.Invoke(@event);
        }
    }
}