using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    public abstract class Menu<T> : Menu where T : Menu<T>
    {
        public static T Instance { get; private set; }

        public static UnityEvent onOpen = new();
        public static UnityEvent onClose = new();
        
        protected virtual void Awake()
        {
            Instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }

        public static T Open()
        {
            if (Instance == null)
            {
                MenuManager.Instance.CreateInstance(typeof(T).Name, out var clonedGameObject);
                MenuManager.Instance.OpenMenu(clonedGameObject.GetMenu());
            }
            else
            {
                Instance.gameObject.SetActive(true);
                MenuManager.Instance.OpenMenu(Instance);
            }
            
            onOpen?.Invoke();
            return Instance;
        }

        public static void Close()
        {
            if (!Instance)
            {
                return;
            }

            MenuManager.Instance.CloseMenu(Instance);
            onClose?.Invoke();
        }

        public override void OnBackPressed()
        {
            Close();
        }
    }

    public abstract class Menu : MonoBehaviour
    {
        [Tooltip("Destroy the Game Object when menu is closed (reduces memory usage)")]
        public bool destroyWhenClosed = true;

        [Tooltip("Disable menus that are under this one in the stack")]
        public bool disableMenusUnderneath = true;

        public abstract void OnBackPressed();
    }
}
