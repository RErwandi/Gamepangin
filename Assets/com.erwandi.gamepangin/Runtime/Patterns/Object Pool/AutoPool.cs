using UnityEngine;

namespace Erwandi.Gamepangin.Patterns
{
    /// <summary>
    /// Automatically releases an object after the specified amount of time has surpassed.
    /// </summary>
    [DisallowMultipleComponent]
    public class AutoPool : PoolableMonoBehaviour
    {
        [Tooltip("The duration of time to wait before releasing the object to the pool.")]
        [SerializeField] private float poolTimer = 1;

        [Tooltip("Whether to use scaled or unscaled time.")]
        [SerializeField] private bool scaledTime = true;

        private float elapsedTime;

        private void OnEnable()
        {
            elapsedTime = 0;
        }

        private void Update()
        {
            if (scaledTime) { elapsedTime += Time.deltaTime; }
            else { elapsedTime += Time.unscaledDeltaTime; }

            if (elapsedTime > poolTimer && PoolReady) { Release(); }
        }
    }
}