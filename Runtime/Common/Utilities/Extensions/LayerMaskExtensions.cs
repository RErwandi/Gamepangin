using UnityEngine;

namespace Gamepangin
{
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Returns bool if layer is within LayerMask
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return ((mask.value & (1 << layer)) > 0);
        }

        /// <summary>
        /// Returns true if gameObject is within LayerMask
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool Contains(this LayerMask mask, GameObject gameObject)
        {
            return ((mask.value & (1 << gameObject.layer)) > 0);
        }
    }
}