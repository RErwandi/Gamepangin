using UnityEngine;

namespace Gamepangin.UI
{
    [System.Serializable]
    public sealed class AudioFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private AudioClipData highlightSound;

        [SerializeField]
        private AudioClipData selectedSound;

        
        public void OnNormal(bool instant) {}

        public void OnHighlighted(bool instant)
        {
            if (!instant)
                AudioManager.Instance.PlaySound(highlightSound);
        }

        public void OnSelected(bool instant)
        {
            if (!instant)
                AudioManager.Instance.PlaySound(selectedSound);
        }

        public void OnPressed(bool instant) {}

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable) {}
#endif
    }
}