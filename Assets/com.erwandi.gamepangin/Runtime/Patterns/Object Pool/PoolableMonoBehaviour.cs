using UnityEngine;

namespace Erwandi.Gamepangin.Patterns
{
    /// <summary>
    /// MonoBehaviour that has a reference to its parent pool, and can thus be released to the pool without the user needing a reference to its pool, making it self contained.
    /// Usable either as a standalone component or as a base class for other components.
    /// </summary>
    public class PoolableMonoBehaviour : MonoBehaviour, IPoolable
    {
        /// <summary>
        /// If its parent pool has been initialized yet.
        /// </summary>
        public bool PoolReady => parentPool;

        [SerializeField]
        [HideInInspector]
        private ObjectPool parentPool;

        void IPoolable.InitializeTemplate(ObjectPool pool)
        {
            parentPool = pool;
        }

        /// <summary>
        /// Releases the object and returns it back to its pool, effectively 'destroying' it from the scene.
        /// </summary>
        public void Release()
        {
            parentPool.Release(gameObject);
        }
    }
}