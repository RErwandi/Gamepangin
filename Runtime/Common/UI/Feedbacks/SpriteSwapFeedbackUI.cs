using UnityEngine;
using UnityEngine.UI;

namespace Gamepangin.UI
{
    [System.Serializable]
    public sealed class SpriteSwapFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private Image image;

        [SerializeField, HideInInspector]
        private Sprite normalSprite;

        [SerializeField]
        private Sprite highlightedSprite;

        [SerializeField]
        private Sprite selectedSprite;

        [SerializeField]
        private Sprite pressedSprite;
        
        
        public void OnNormal(bool instant) => image.sprite = normalSprite;
        public void OnHighlighted(bool instant) => image.sprite = highlightedSprite;
        public void OnSelected(bool instant) => image.sprite = selectedSprite;
        public void OnPressed(bool instant) => image.sprite = pressedSprite;

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable)
        {
            if (selectable.TryGetComponent(out image))
                image.sprite = normalSprite;
        }
#endif
    }
}