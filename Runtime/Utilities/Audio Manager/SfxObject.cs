using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Erwandi.Gamepangin.Utilities
{
    [CreateAssetMenu(order = 0, fileName = "SfxObject", menuName = "Gamepangin/SFX Object")]
    public class SfxObject : ScriptableObject
    {
        public string sfxName;
        public bool randomizeVolume;
        
        [HideIf("randomizeVolume"), Range(0f, 1f)]
        public float volume = 1f;
        
        [ShowIf("randomizeVolume"), MinMaxSlider(0f, 1f,  true)]
        public Vector2 randomVolume = new Vector2(0.5f, 1f);
        
        public bool randomizePitch;
        
        [HideIf("randomizePitch"), Range(-3f, 3f)]
        public float pitch = 1f;
        
        [ShowIf("randomizePitch"), MinMaxSlider(-3f, 3f,  true)]
        public Vector2 randomPitch = new Vector2(0.5f, 1.5f);
        
        public bool useDelay;
        
        [ShowIf("useDelay")]
        public float delay;
        
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "variantName", CustomAddFunction = "VariantCount")]
        public List<SfxVariation> variations = new List<SfxVariation>();

        private SfxVariation VariantCount()
        {
            var sfxVariantTemplate = new SfxVariation {variantName = $"Variant {variations.Count + 1}"};
            return sfxVariantTemplate;
        }
    }
}