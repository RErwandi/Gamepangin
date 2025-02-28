using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    public class Inventory : SerializedMonoBehaviour, IInventory
    {
        [SerializeField] private Dictionary<Currency, int> currencies = new();
        
        [SerializeField]
        [Tooltip("The initial item containers")]
        private ContainerGenerator[] startupContainers;
        
        private List<ItemContainer> containers = new();
        
        public List<ItemContainer> Containers => containers;
        public ContainerGenerator[] StartupContainers => startupContainers;
        
        public event UnityAction InventoryChanged;
        public event UnityAction<ItemSlot.CallbackContext> ItemChanged;
        public event UnityAction ContainersCountChanged;
        public event UnityAction CurrencyChanged;
        
        private void OnInventoryChanged() => InventoryChanged?.Invoke();
        private void OnItemChanged(ItemSlot.CallbackContext context) => ItemChanged?.Invoke(context);
        
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
        
        public int RemoveItemsWithId(string itemId, int amountToRemove)
        {
            if (amountToRemove <= 0)
                return 0;

            int removedInTotal = 0;

            if (GetItemsWithIdCount(itemId) <= 0) { return 0; }

            foreach (var container in containers)
            {
                int removedNow = container.RemoveItem(itemId, amountToRemove);
                removedInTotal += removedNow;

                if (removedInTotal == amountToRemove)
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

        public List<ItemDefinition> GetAllItemDefinitions()
        {
            List<ItemDefinition> allItems = new List<ItemDefinition>();

            foreach (var container in Containers)
            {
                foreach (var slot in container.Slots)
                {
                    if (slot.HasItem)
                    {
                        allItems.Add(slot.Item.Definition);
                    }
                }
            }

            return allItems;
        }

        public List<Item> GetAllItems()
        {
            List<Item> allItems = new List<Item>();

            foreach (var container in Containers)
            {
                foreach (var slot in container.Slots)
                {
                    if (slot.HasItem)
                    {
                        allItems.Add(slot.Item);
                    }
                }
            }

            return allItems;
        }

        public void ClearInventory()
        {
            foreach (var container in containers)
            {
                container.RemoveAllItems();
            }

            OnInventoryChanged();
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

        #region Currency

        public void AddCurrency(Currency currency, int amount)
        {
            if (currencies.TryGetValue(currency, out var currentAmount))
            {
                currentAmount += amount;
                currencies[currency] = currentAmount;
            }
            else
            {
                currencies.TryAdd(currency, amount);
            }
            
            CurrencyChanged?.Invoke();
        }

        public void AddCurrency(string currencyId, int amount)
        {
            if (Currency.TryGetWithId(currencyId, out var currency))
            {
                AddCurrency(currency, amount);
            }
        }
        
        public void SubtractCurrency(Currency currency, int amount)
        {
            if (currencies.TryGetValue(currency, out var currentAmount))
            {
                currentAmount = Mathf.Clamp(currentAmount - amount, 0, currentAmount);
                currencies[currency] = currentAmount;
            }
            else
            {
                currencies.TryAdd(currency, 0);
            }
            
            CurrencyChanged?.Invoke();
        }

        public void SubtractCurrency(string currencyId, int amount)
        {
            if (Currency.TryGetWithId(currencyId, out var currency))
            {
                SubtractCurrency(currency, amount);
            }
        }

        public bool SpentCurrency(Currency currency, int amount)
        {
            if (currencies.TryGetValue(currency, out var currentAmount))
            {
                if (currentAmount <= amount)
                {
                    SubtractCurrency(currency, amount);
                    return true;
                }
            }

            return false;
        }

        public int GetCurrency(Currency currency)
        {
            return currencies.GetValueOrDefault(currency, 0);
        }

        #endregion
    }
}