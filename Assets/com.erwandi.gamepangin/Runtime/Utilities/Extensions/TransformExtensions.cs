using UnityEngine;

namespace Erwandi.Gamepangin.Utilities
{
    /// <summary>
    /// Extension methods for Transform type
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Destroy all children on a transform.
        /// </summary>
        /// <param name="transform"></param>
        public static void DestroyAllChildren(this Transform transform)
        {
            for (var t = transform.childCount - 1; t >= 0; t--)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(transform.GetChild(t).gameObject);
                }
                else
                {
                    Object.DestroyImmediate(transform.GetChild(t).gameObject);
                }
            }
        }
    }
}