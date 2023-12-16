using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    public class Inventory : MonoBehaviour, IInventory
    {
        [SerializeField]
        [Tooltip("The initial item containers")]
        private ContainerGenerator[] startupContainers;
        
        private List<ItemContainer> containers = new();
        
        public List<ItemContainer> Containers => containers;
        public ContainerGenerator[] StartupContainers => startupContainers;
        
        public event UnityAction InventoryChanged;
        public event UnityAction<ItemSlot.CallbackContext> ItemChanged;
        public event UnityAction ContainersCountChanged;
        
        private void OnInventoryChanged() => InventoryChanged?.Invoke();
        private void OnItemChanged(ItemSlot.CallbackContext context) => ItemChanged?.Invoke(context);
        
        public void AddContainer(ItemContainer container, bool triggerContainersEvent = true)
        {
            if (containers.Contains(container))
                return;
            
            containers.Add(container);

            container.ContainerChanged += OnInventoryChanged;
            container.SlotChanged += OnItemChanged;

            if (triggerContainersEvent)
                ContainersCountChanged?.Invoke();
        }
        
        public void RemoveContainer(ItemContainer container, bool triggerContainersEvent = true)
        {
            if (!containers.Remove(container))
                return;

            container.ContainerChanged += OnInventoryChanged;
            container.SlotChanged += OnItemChanged;

            if (triggerContainersEvent)
                ContainersCountChanged?.Invoke();
        }
        
        public int AddItem(Item item)
        {
            int addedInTotal = 0;

            foreach (var container in containers)
            {
                addedInTotal += container.AddItem(item);

                if (addedInTotal >= item.StackCount)
                    break;
            }

            return addedInTotal;
        }
        
        [Button]
        public int AddItemsWithId(string itemId, int amountToAdd)
        {
            if (amountToAdd <= 0)
                return 0;

            int addedInTotal = 0;
            
            foreach (var container in containers)
            {
                int added = container.AddItem(itemId, amountToAdd - addedInTotal);
                addedInTotal += added;

                if (addedInTotal == amountToAdd)
                    break;
            }

            return addedInTotal;
        }
        
        public bool RemoveItem(Item item)
        {
            foreach (var container in containers)
            {
                if (container.RemoveItem(item))
                    return true;
            }

            return false;
        }
        
        [Button]
        public int RemoveItemsWithId(string itemId, int amountToRemove)
        {
            if (amountToRemove <= 0)
                return 0;

            int removedInTotal = 0;

            foreach (var container in containers)
            {
                int removedNow = container.RemoveItem(itemId, amountToRemove);
                removedInTotal += removedNow;

                if (removedNow == removedInTotal)
                    break;
            }

            return removedInTotal;
        }
        
        public int GetItemsWithIdCount(string itemId)
        {
            int count = 0;

            foreach (var container in containers)
                count += container.GetItemCount(itemId);

            return count;
        }

        public bool ContainsItem(Item item)
        {
            foreach (var container in containers)
            {
                if (container.ContainsItem(item))
                    return true;
            }

            return false;
        }

        private void RemoveAllContainers()
        {
            if (containers == null)
                return;

            foreach (var container in containers)
            {
                container.ContainerChanged -= OnInventoryChanged;
                container.SlotChanged -= OnItemChanged;
            }

            containers.Clear();
        }
        
        private void Awake()
        {
            if (containers.Count > 0)
            {
                return;
            }

            containers = new List<ItemContainer>();
            for (int i = 0; i < startupContainers.Length; i++)
            {
                var container = startupContainers[i].GenerateContainer();
                AddContainer(container, false);
            }
        }
    }
}