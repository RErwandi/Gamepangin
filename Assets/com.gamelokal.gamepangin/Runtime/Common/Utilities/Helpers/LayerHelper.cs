using UnityEngine;

namespace Gamepangin
{
    public static class LayerHelper
    {
        public static bool LayerInLayerMask(int layer, LayerMask layerMask)
        {
            return ((1 << layer) & layerMask) != 0;
        }
    }
}