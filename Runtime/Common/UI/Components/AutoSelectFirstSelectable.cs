using UnityEngine;

namespace Gamepangin.UI
{
    public class AutoSelectFirstSelectable : MonoBehaviour
    {
        [SerializeField] private ItemContainerUI itemContainer;
        [SerializeField] private FramedSelectableGroupUI selectableGroup;

        private bool isInitialized;
        private void OnEnable()
        {
            if (selectableGroup.Selected == null && !isInitialized)
            {
                SelectFirst(null);
            }
        }

        private void Start()
        {
            if (itemContainer.IsAttached)
            {
                SelectFirst(null);
            }
            else
            {
                itemContainer.onContainerAttached += SelectFirst;
            }
        }

        private void SelectFirst(ItemContainer container)
        {
            if(!itemContainer.IsAttached) return;
            
            var first = itemContainer.Slots[0].GetComponent<SelectableUI>();
            selectableGroup.SelectSelectable(first);
            itemContainer.onContainerAttached -= SelectFirst;
        }
    }
}