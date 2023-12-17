using UnityEngine;
using UnityEngine.UI;

namespace Gamepangin.UI
{
    [System.Serializable]
    public sealed class ColorTintFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private Graphic targetGraphic;

        [SerializeField]
        private Color normalColor = Color.grey;
        
        [SerializeField]
        private Color highlightedColor = Color.grey;

        [SerializeField]
        private Color selectedColor = Color.grey;

        [SerializeField]
        private Color pressedColor = Color.grey;

        [SerializeField, Range(0.01f, 1f)]
        private float fadeDuration = 0.1f;


        public void OnNormal(bool instant)
        {
            if (instant)
                targetGraphic.CrossFadeColor(normalColor, 0f, true, true);
            else
                targetGraphic.CrossFadeColor(normalColor, fadeDuration, true, true);
        }

        public void OnHighlighted(bool instant)
        {
            if (instant)
                targetGraphic.CrossFadeColor(highlightedColor, 0f, true, true);
            else
                targetGraphic.CrossFadeColor(highlightedColor, fadeDuration, true, true);
        }

        public void OnSelected(bool instant)
        {
            if (instant)
                targetGraphic.CrossFadeColor(selectedColor, 0f, true, true);
            else
                targetGraphic.CrossFadeColor(selectedColor, fadeDuration, true, true);
        }

        public void OnPressed(bool instant)
        {
            if (instant)
                targetGraphic.CrossFadeColor(pressedColor, 0f, true, true);
            else
                targetGraphic.CrossFadeColor(pressedColor, fadeDuration, true, true);
        }

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable)
        {
            targetGraphic = selectable.GetComponent<Graphic>();

            if (selectable.TryGetComponent(out CanvasRenderer canRenderer))
                canRenderer.SetColor(normalColor);
        }
#endif
    }
}