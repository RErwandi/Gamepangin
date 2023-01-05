using UnityEngine;

namespace Gamepangin
{
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        public static bool IsExiting { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void OnSubsystemsInit()
        {
            IsExiting = false;
            Instance.WakeUp();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            ApplicationEvent.Trigger(hasFocus ? AppEventType.OnApplicationFocus : AppEventType.OnApplicationBackground);
        }

        private void OnApplicationQuit()
        {
            IsExiting = true;
            
            ApplicationEvent.Trigger(AppEventType.OnApplicationQuit);
        }
    }
}