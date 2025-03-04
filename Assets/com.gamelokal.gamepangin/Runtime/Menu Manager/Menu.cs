using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gamepangin
{
    public abstract class Menu<T> : Menu where T : Menu<T>
    {
        public static T Instance { get; private set; }

        public static UnityEvent onOpen = new();
        public static UnityEvent onClose = new();

        public static bool IsActive => Instance.gameObject.activeInHierarchy;

        protected override void Awake()
        {
            base.Awake();
            Instance = (T)this;

            if (Instance.BackButton != null)
            {
                Instance.BackButton.onClick.AddListener(Close);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance.BackButton != null)
            {
                Instance.BackButton.onClick.RemoveListener(Close);
            }
            
            Instance = null;
        }

        public static T Open()
        {
            if (Instance == null)
            {
                MenuManager.Instance.CreateInstance(typeof(T).Name, out var clonedGameObject);
                MenuManager.Instance.OpenMenu(clonedGameObject.GetMenu());
            }
            else if (IsActive)
            {
                Debug.LogWarning($"{Instance.gameObject.name} already opened");
                return Instance;
            }
            else if (!IsActive)
            {
                MenuManager.Instance.OpenMenu(Instance);
            }
            
            Instance.OnOpen();
            onOpen?.Invoke();
            return Instance;
        }

        public static void Close()
        {
            if (!Instance)
            {
                return;
            }

            Instance.OnClose();
            onClose?.Invoke();
            MenuManager.Instance.CloseMenu(Instance);
        }

        public override void OnBackPressed()
        {
            if(Instance.BackOnEscPressed)
                Close();
        }
        
        protected override void OnOpen(){ }
        protected override void OnClose(){ }
    }

    [RequireComponent(typeof(Canvas))]
    public abstract class Menu : MonoBehaviour
    {
        [SerializeField] private bool resetAndDisableOnAwake = true;

        [SerializeField] private bool closeOtherMenuWhenOpen = true;
        public bool CloseOtherMenuWhenOpen => closeOtherMenuWhenOpen;
        
        [Tooltip("Destroy the Game Object when menu is closed (reduces memory usage)")]
        [SerializeField] private bool destroyOnClosed;
        [SerializeField] private bool backOnEscPressed = true;
        [SerializeField] private Button backButton;
        public Button BackButton => backButton;
        public bool BackOnEscPressed => backOnEscPressed;
        public bool DestroyOnClosed => destroyOnClosed;
        
        
        private Canvas canvas;
        public Canvas Canvas => canvas;

        protected virtual void Awake()
        {
            canvas = GetComponent<Canvas>();
            
            if (resetAndDisableOnAwake)
            {
                var rect = gameObject.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.zero;
                gameObject.SetActive(false);
            }
        }

        public void ReOpen()
        {
            OnOpen();
        }

        public abstract void OnBackPressed();

        protected abstract void OnOpen();

        protected abstract void OnClose();
    }
}
