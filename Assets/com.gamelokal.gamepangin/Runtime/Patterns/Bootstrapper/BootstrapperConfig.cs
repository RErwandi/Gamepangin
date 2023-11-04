using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Gamepangin
{
    [GlobalConfig("_Gamepangin/Resources")]
    public class BootstrapperConfig : GlobalConfig<BootstrapperConfig>
    {
        [InfoBox("Prefabs listed here will be instantiated and marked DontDestroyOnLoad on initialization before scene load.")]
        [SerializeField] private List<GameObject> prefabs;
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            foreach (var prefab in Instance.prefabs)
            {
                var go = Instantiate(prefab);
                DontDestroyOnLoad(go);
            }
        }
    }
}