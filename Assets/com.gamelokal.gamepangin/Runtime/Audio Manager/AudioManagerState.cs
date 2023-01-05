using System;

namespace Gamepangin
{
    [Serializable]
    public class AudioManagerState
    {
        public bool masterOn = true;
        public float masterVolume = 1f;
        public float masterSavedVolume = 1f;
        public bool sfxOn = true;
        public float sfxVolume = 1f;
        public float sfxSavedVolume = 1f;
        public bool musicOn = true;
        public float musicVolume = 1f;
        public float musicSavedVolume = 1f;
        public bool uiOn = true;
        public float uiVolume = 1f;
        public float uiSavedVolume = 1f;
    }
}