namespace Gamepangin
{
    public struct GameEvent
    {
        public string eventName;

        public GameEvent(string newName)
        {
            eventName = newName;
        }

        private static GameEvent gameEvent;

        public static void Trigger(string newName)
        {
            gameEvent.eventName = newName;
            EventManager.TriggerEvent(gameEvent);
        }
    }
}