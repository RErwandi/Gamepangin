using UnityEngine;

namespace Erwandi.Gamepangin.Patterns
{
    /// <summary>
    /// This will create a new empty GameObject to act as a parent for all Singleton classes
    /// </summary>
    static class SingletonService
    {
        private static Transform _servicesObjectTransform;
        
        public static Transform Parent
        {
            get
            {
                if (_servicesObjectTransform != null) return _servicesObjectTransform;
                
                _servicesObjectTransform = new GameObject("=== Singletons ===").transform;
                Object.DontDestroyOnLoad(_servicesObjectTransform);

                return _servicesObjectTransform;
            }
        }
    }
}