using UnityEngine;

namespace Gamepangin
{
    public struct AudioManagerPlaySoundEvent
    {
        public AudioClip audioClip;
        public AudioManagerPlayOptions options;

        public AudioManagerPlaySoundEvent(AudioClip audioClip, AudioManagerPlayOptions options)
        {
            this.audioClip = audioClip;
            this.options = options;
        }

        private static AudioManagerPlaySoundEvent _event;

        public static void Trigger(AudioClip audioClip, AudioManagerPlayOptions options)
        {
            _event.audioClip = audioClip;
            _event.options = options;
            EventManager.TriggerEvent(_event);
        }
    }
}