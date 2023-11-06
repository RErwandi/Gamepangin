using System;
using Sirenix.OdinInspector;

namespace Gamepangin
{
    [Serializable]
    public class AudioManagerState
    {
        [TabGroup("Master")]
        public bool masterOn = true;
        [TabGroup("Master")]
        public float masterVolume = 1f;
        [TabGroup("Master")]
        public float masterSavedVolume = 1f;
        
        [TabGroup("SFX")]
        public bool sfxOn = true;
        [TabGroup("SFX")]
        public float sfxVolume = 1f;
        [TabGroup("SFX")]
        public float sfxSavedVolume = 1f;
        
        [TabGroup("Music")]
        public bool musicOn = true;
        [TabGroup("Music")]
        public float musicVolume = 1f;
        [TabGroup("Music")]
        public float musicSavedVolume = 1f;
        
        [TabGroup("UI")]
        public bool uiOn = true;
        [TabGroup("UI")]
        public float uiVolume = 1f;
        [TabGroup("UI")]
        public float uiSavedVolume = 1f;
    }
}