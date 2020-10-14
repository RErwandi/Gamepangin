using System;
using System.Collections.Generic;

namespace Erwandi.Gamepangin.Patterns
{
    static class EventBusDispatcher<T> where T : IEvent
    {
        private static readonly Dictionary<IEventBus, Action<T>> Actions = new Dictionary<IEventBus, Action<T>>();

        public static void AddListener(IEventBus bus, Action<T> listener)
        {
            if (!Actions.ContainsKey(bus))
            {
                Actions.Add(bus, delegate { });
            }

            Actions[bus] += listener;
        }

        public static void RemoveListener(IEventBus bus, Action<T> listener)
        {
            if (Actions.ContainsKey(bus))
            {
                Actions[bus] -= listener;
            }
        }

        public static void Trigger(IEventBus bus, T @event)
        {
            if (Actions.TryGetValue(bus, out var action))
            {
                action.Invoke(@event);
            }
        }
    }
}