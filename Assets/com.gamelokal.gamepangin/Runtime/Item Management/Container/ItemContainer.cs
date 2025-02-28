using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    [Serializable]
    public sealed class ItemContainer
    {
        [SerializeField] private string containerName;
        [SerializeField] private List<ItemSlot> slots;
        [SerializeField] private List<ContainerRestriction> restrictions;
        
        public ItemSlot this[int i] => slots[i];
        public string Name => containerName;
        public int Capacity => slots.Count;
        public IReadOnlyList<ItemSlot> Slots => slots;
        public List<ContainerRestriction> Restrictions => restrictions;
        
        public event UnityAction ContainerChanged;
        public event UnityAction<ItemSlot.CallbackContext> SlotChanged;
        public event UnityAction<ItemDefinition, int> ItemsAdded;
        
        public ItemContainer(string name, int size, List<ContainerRestriction> restrictions, List<ItemHolder> items = null)
        {
            containerName = name;

            // Initialize and populate the slots.
            slots = new List<ItemSlot>(size);
            for (int i = 0; i < slots.Capacity; i++)
                slots.Add(new ItemSlot(this));

            // Initialize the restrictions.
            this.restrictions = restrictions;
            foreach (var restriction in restrictions)
                restriction.OnInitialized(this);

            CreateSlots();

            if (items != null)
            {
	            foreach (var item in items)
	            {
		            if(item.item == null) continue;
		            AddItem(new Item(item.item, item.amount));
	            }
            }
        }
        
        public void OnLoad()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];

                slot = new ItemSlot(slot.Item, this);
                slot.ItemChanged += OnSlotChanged;

                slots[i] = slot;
            }

            foreach (var restriction in restrictions)
                restriction.OnInitialized(this);
        }
        
        public void SetCapacity(int capacity)
        {
            if (capacity == Capacity)
                return;

            if (capacity > Capacity)
            {
                slots.Capacity = capacity;
                return;
            }

            if (capacity < Capacity)
            {
                int removeCount = Capacity - capacity + 1;
                slots.RemoveRange(capacity - 1, removeCount);
            }
        }

        private void CreateSlots()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i] = new ItemSlot(this);
                slots[i].ItemChanged += OnSlotChanged;
            }
        }

        private void OnSlotChanged(ItemSlot.CallbackContext context)
        {
            if (context.type == ItemSlot.CallbackType.PropertyChanged)
                return;

            ContainerChanged?.Invoke();
            SlotChanged?.Invoke(context);
        }
        
		public IEnumerator<ItemSlot> GetEnumerator() => slots.GetEnumerator();

		public int AddItem(string id, int amount)
		{
			if (ItemDefinition.TryGetWithId(id, out var itemDef))
				return AddItemWithDefinition(itemDef, amount);

			return 0;
		}

		public int AddItem(Item item)
		{
			if (GetAllowedCount(item, item.StackCount) < 1)
				return 0;

			// If the item is stackable try to stack it with other items.
			if (item.Definition.StackSize > 1)
			{
				int stackAddedCount = AddItemWithDefinition(item.Definition, item.StackCount);
				item.StackCount -= stackAddedCount;
				
				return stackAddedCount;
			}
			
			// The item's not stackable, try find an empty slot for it.
			foreach (var slot in slots)
			{
				if (!slot.HasItem)
				{
					slot.Item = item;
					ItemsAdded?.Invoke(slot.Item.Definition, 1);
					return 1;
				}
			}

			return 0;
		}

		private int AddItemWithDefinition(ItemDefinition itemDef, int amount)
		{
			int allowedAmount = GetAllowedCount(itemDef, amount);
			int added = 0;

			if (allowedAmount == 0) return 0;
			
			// Go through each slot and see where we can add the item(s)
			foreach (var slot in slots)
			{
				added += AddItemToSlot(slot, itemDef, allowedAmount - added);
				
				// We've added all the items, we can stop now
				if (added == allowedAmount)
				{
					ItemsAdded?.Invoke(itemDef, added);
					return added;
				}
			}
			
			ItemsAdded?.Invoke(itemDef, added);
			return added;
		}

		private int AddItemToSlot(ItemSlot slot, ItemDefinition itemDef, int amount)
		{
			if (slot.HasItem)
			{
				// If the slot already has an item in it or the item is not of the same type return.
				if (!itemDef.Id.Equals(slot.Item.Id))
					return 0;

				// Add to stack.
				return slot.Item.ChangeStack(amount);
			}
			
			// If the slot is empty, create a new item.
			slot.Item = new Item(itemDef, amount);
			return slot.Item.StackCount;
		}

		public void RemoveAllItems()
		{
			foreach (var slot in slots)
			{
				if (slot.HasItem)
				{
					slot.Item = null;
				}
			}

			Debug.Log("All items have been removed from the container.");
		}

		public bool RemoveItem(Item item)
		{
			foreach (var slot in slots)
			{
				if (slot.Item == item)
				{
					slot.Item = null;
					return true;
				}
			}

			return false;
		}

		public bool RemoveAtIndex(int index)
		{
			if (slots[index].HasItem)
			{
				slots[index].Item = null;
				return true;
			}

			return false; 
		}

		public int RemoveItem(string id, int amount)
		{
			Debug.Log($"Trying to remove itemId {id} x{amount}");
			int removed = 0;

			foreach (var slot in slots)
			{
				if (!slot.HasItem || slot.Item.Id != id)
					continue;
				
				removed += slot.Item.ChangeStack(-(amount - removed));

				// We've removed all the items, we can stop now
				if (removed == amount)
					return removed;
			}

			return removed;
		}

		public bool ContainsItem(Item item)
		{
			foreach (var slot in slots)
			{
				if (slot.Item == item)
					return true;
			}

			return false;
		}

		public int GetItemCount(string id)
		{
			int count = 0;

			foreach (var slot in slots)
			{
				if (slot.HasItem && slot.Item.Id.Equals(id))
                {
					count += slot.Item.StackCount;
				}
			}

			return count;
		}

		public int GetAllowedCount(Item item, int count)
		{
			if (item == null)
				return 0;

			int allowAmount = count;
			foreach (var restriction in restrictions)
			{
				allowAmount = restriction.GetAllowedAddAmount(item, allowAmount);

				if (allowAmount <= 0)
					return 0;
			}

			return allowAmount;
		}

		public int GetAllowedCount(Item item, int count, out string rejectReason)
		{
			if (item == null)
			{
				rejectReason = "Item is NULL";
				return 0;
			}

			int allowAmount = count;
			foreach (var restriction in restrictions)
			{
				allowAmount = restriction.GetAllowedAddAmount(item, allowAmount);

				if (allowAmount <= 0)
				{
					rejectReason = restriction.GetRejectionString();
					return 0;
				}
			}

			rejectReason = string.Empty;
			return allowAmount;
		}

		private Item dummyItem;
		private int GetAllowedCount(ItemDefinition itemDef, int count)
		{
			if (itemDef == null)
				return 0;
			
			dummyItem = new Item(itemDef, count);
			return GetAllowedCount(dummyItem, count);
		}
    }
}