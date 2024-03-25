using UnityEngine;

namespace Gamepangin.UI
{
    public class ItemDropUI : MonoBehaviour
    {
        private enum DropBehaviour
        {
            RemoveItem,
            SpawnPrefab
        }
        
        [SerializeField] private DropBehaviour dropBehaviour;
        
        public void Drop(ItemContainer container, int slotIndex)
        {
            switch (dropBehaviour)
            {
                case DropBehaviour.RemoveItem:
                    Remove(container, slotIndex);
                    break;
                case DropBehaviour.SpawnPrefab:
                    SpawnPrefab(container, slotIndex);
                    break;
            }
        }

        protected void SpawnPrefab(ItemContainer container, int slotIndex)
        {
            var itemPrefab = container.Slots[slotIndex].Item.Definition.Prefab;
            var itemDropOrigin = ItemDropOrigin.Instance.transform;
            
            if (itemPrefab != null)
            {
                Instantiate(itemPrefab, itemDropOrigin.position, itemDropOrigin.rotation);
                Debug.Log($"Spawning {itemPrefab} on {itemDropOrigin.position}");
            }

            Remove(container, slotIndex);
        }

        protected void Remove(ItemContainer container, int slotIndex)
        {
            if (container.RemoveAtIndex(slotIndex))
            {
                Debug.Log($"Removing item on container {container.Name} on index {slotIndex}");
            }
        }
    }
}
