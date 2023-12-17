using UnityEngine;
using UnityEngine.UI;

namespace Gamepangin.UI
{
    [System.Serializable]
    public sealed class HighlightGraphicFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private Image selectionGraphic;

        [SerializeField]
        private Color selectedColor;

        [SerializeField]
        private Color highlightedColor;


        public void OnNormal(bool instant)
        {
            selectionGraphic.enabled = false;
        }

        public void OnHighlighted(bool instant)
        {
            selectionGraphic.enabled = true;
            selectionGraphic.color = highlightedColor;
        }

        public void OnSelected(bool instant)
        {
            selectionGraphic.enabled = true;
            selectionGraphic.color = selectedColor;
        }

        public void OnPressed(bool instant)
        {
        }

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable) { }
#endif
    }
}