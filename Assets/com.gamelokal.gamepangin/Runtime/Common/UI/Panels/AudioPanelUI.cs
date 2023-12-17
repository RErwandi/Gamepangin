using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin.UI
{
    [AddComponentMenu("Gamepangin/UI/Panels/Audio Panel")]
    public class AudioPanelUI : PanelUI
    {
        [Title("Audio")]

        [SerializeField]
        private AudioClipData showAudio;

        [SerializeField]
        private AudioClipData hideAudio;

        protected override void ShowPanel() => AudioManager.Instance.PlaySound(showAudio);
        protected override void HidePanel() => AudioManager.Instance.PlaySound(hideAudio);
    }
}