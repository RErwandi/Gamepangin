using UnityEngine;
using UnityEngine.Rendering;

namespace Gamepangin
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SortingGroup))]
    public class DepthSortByY : MonoBehaviour
    {
        public SortingGroup sortingGroup;
        private const int ISOMETRIC_RANGE_PER_Y_UNIT = 100;

        private void Awake()
        {
            sortingGroup = GetComponent<SortingGroup>();
        }

        void Update()
        {
            sortingGroup.sortingOrder = -(int)(transform.position.y * ISOMETRIC_RANGE_PER_Y_UNIT);
        }
    }
}