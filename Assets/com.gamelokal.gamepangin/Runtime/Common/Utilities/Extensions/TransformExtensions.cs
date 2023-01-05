using UnityEngine;
using System.Collections.Generic;

namespace Gamepangin
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Destroys a transform's children
        /// </summary>
        /// <param name="transform"></param>
        public static void DestroyAllChildren(this Transform transform)
        {
            for (int t = transform.childCount - 1; t >= 0; t--)
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

        /// <summary>
        /// Finds children by name, breadth first
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="transformName"></param>
        /// <returns></returns>
        public static Transform FindDeepChildBreadthFirst(this Transform parent, string transformName)
        {
            var queue = new Queue<Transform>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                var child = queue.Dequeue();
                if (child.name == transformName)
                {
                    return child;
                }
                foreach (Transform t in child)
                {
                    queue.Enqueue(t);
                }
            }
            return null;
        }

        /// <summary>
        /// Finds children by name, depth first
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="transformName"></param>
        /// <returns></returns>
        public static Transform FindDeepChildDepthFirst(this Transform parent, string transformName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == transformName)
                {
                    return child;
                }

                var result = child.FindDeepChildDepthFirst(transformName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
