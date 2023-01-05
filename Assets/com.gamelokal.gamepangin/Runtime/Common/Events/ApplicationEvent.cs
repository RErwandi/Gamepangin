namespace Gamepangin
{
    public enum AppEventType
    {
        OnApplicationBackground,
        OnApplicationFocus,
        OnApplicationQuit
    }
    public struct ApplicationEvent
    {
        public AppEventType appEvent;

        public ApplicationEvent(AppEventType newAppEvent)
        {
            appEvent = newAppEvent;
        }

        private static ApplicationEvent e;

        public static void Trigger(AppEventType newAppEvent)
        {
            e.appEvent = newAppEvent;
            EventManager.TriggerEvent(e);
        }
    }
}