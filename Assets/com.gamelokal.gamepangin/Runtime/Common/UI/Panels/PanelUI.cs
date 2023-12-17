using UnityEngine;
using UnityEngine.Serialization;

namespace Gamepangin.UI
{
    /// <summary>
    /// Basic UI Panel that can be toggled on and off.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("Gamepangin/UI/Panels/Panel")]
    public class PanelUI : MonoBehaviour
    {
        public CanvasGroup CanvasGroup => canvasGroup;
        public bool IsVisible { get; private set; }

        [SerializeField, FormerlySerializedAs("m_ShowOnEnable")]
        private bool showOnStart;

        [SerializeField]
        private CanvasGroup canvasGroup;


        /// <summary>
        /// Show/Hide the panel.
        /// </summary>
        public void Show(bool show)
        {
            if (IsVisible != show)
                Show_Internal(show);
        }

        protected void Show_Internal(bool show)
        {
#if UNITY_EDITOR
            if (ApplicationManager.IsExiting)
                return;
#endif

            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;

            bool wasVisible = IsVisible;
            IsVisible = show;

            if (show) ShowPanel();
            else HidePanel();
        }

        protected virtual void Start() => Show_Internal(showOnStart);
        protected virtual void ShowPanel() => canvasGroup.alpha = 1f;
        protected virtual void HidePanel() => canvasGroup.alpha = 0f;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();	
        }
#endif
    }
}