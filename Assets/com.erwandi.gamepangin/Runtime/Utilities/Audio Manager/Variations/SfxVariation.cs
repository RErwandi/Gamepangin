using Sirenix.OdinInspector;
using UnityEngine;

namespace Erwandi.Gamepangin.Utilities
{
    [System.Serializable]
    public class SfxVariation
    {
        public string variantName;
        public AudioClip sfx;
        public bool randomizeVolume;
        
        [HideIf("randomizeVolume"), Range(0f, 1f)]
        public float volume = 1f;
        
        [ShowIf("randomizeVolume"), MinMaxSlider(0f, 1f,  true)]
        public Vector2 randomVolume = new Vector2(0.5f, 1f);
        
        public bool randomizePitch;
        
        [HideIf("randomizePitch"), Range(0f, 3f)]
        public float pitch = 1f;
        
        [ShowIf("randomizePitch"), MinMaxSlider(0f, 3f,  true)]
        public Vector2 randomPitch = new Vector2(0.5f, 1.5f);
        
        public bool useDelay;
        
        [ShowIf("useDelay")]
        public float delay;

        public bool mute;
        public bool bypassEffect;
        public bool bypassReverbZone;
        
        [Range(0, 256)]
        public int priority = 128;

        [Range(-1f, 1f)]
        public float stereoPan;
        
        [Range(0f, 1f)]
        public float spatialBlend;
        
        [Range(0f, 1f)]
        public float reverbZoneMix;
    }
}