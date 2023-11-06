using System;

namespace Gamepangin
{
	public static class ContainerUtility
	{
		/// <summary>
		/// Adds or swaps an item to a different container.
		/// </summary>
		public static bool AddOrSwapItem(this ItemContainer slotParent, ItemSlot itemToSwap, ItemContainer targetContainer)
		{
			if (!itemToSwap.HasItem || (slotParent == targetContainer))
				return false;

			Item item = itemToSwap.Item;

			if (targetContainer.AllowsItem(item))
			{
				if (targetContainer.AddItem(item) > 0)
				{
					itemToSwap.Item = null;
					return true;
				}

				var lastEmptySlot = targetContainer.GetLastEmptySlot();
				if (lastEmptySlot.Item == null || slotParent.AllowsItem(lastEmptySlot.Item))
				{
					Item tempItem = lastEmptySlot.Item;
					lastEmptySlot.Item = item;
					itemToSwap.Item = tempItem;

					return true;
				}

				return false;
			}

			return false;
		}

		/// <summary>
		/// Checks if this container has a restriction of the given type.
		/// </summary>
		/// <param name="container"> Target container. </param>
		/// <typeparam name="T"> Restriction type. </typeparam>
		/// <returns> True if found, false otherwise. </returns>
		public static bool HasContainerRestriction<T>(this ItemContainer container) where T : ContainerRestriction
		{
			Type type = typeof(T);
			var restrictions = container.Restrictions;

			foreach (var restriction in restrictions)
			{
				if (restriction.GetType() == type)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the restriction of the given type from this container.
		/// </summary>
		/// <param name="container"> Target container. </param>
		/// <typeparam name="T"> Restriction type. </typeparam>
		/// <returns> Restriction. </returns>
		public static T GetContainerRestriction<T>(this ItemContainer container) where T : ContainerRestriction
		{
			Type type = typeof(T);
			var restrictions = container.Restrictions;

			foreach (var restriction in restrictions)
			{
				if (restriction.GetType() == type)
					return (T)restriction;
			}

			return null;
		}
		
		/// <summary>
		/// Tries to get a restriction of the given type from this container.
		/// </summary>
		/// <param name="container"> Target container. </param>
		/// <param name="restriction"> Output variable for the restriction. </param>
		/// <typeparam name="T"> Restriction type. </typeparam>
		/// <returns> True if found, false otherwise. </returns>
		public static bool TryGetContainerRestriction<T>(this ItemContainer container, out T restriction) where T : ContainerRestriction
		{
			Type type = typeof(T);
			var restrictions = container.Restrictions;

			foreach (var rest in restrictions)
			{
				if (rest.GetType() == type)
				{
					restriction = (T)rest;
					return true;
				}
			}

			restriction = null;
			return false;
		}

		public static bool HasTag(this ItemContainer container, ItemTagDefinition tag)
		{
			if (container.TryGetContainerRestriction<ContainerTagRestriction>(out var tagRestriction))
				return tagRestriction.ValidTags.Contains(tag);

			return false;
		}
		
		/// <summary>
		/// Creates and adds an amount of items with the given name to this container.
		/// </summary>
		/// <param name="container"> Target container. </param>
		/// <param name="id"> Item definition name. </param>
		/// <param name="amount"> Amount to add. </param>
		/// <returns> Added count. </returns>
		public static int AddItem(this ItemContainer container, string id, int amount)
		{
			if (ItemDefinition.TryGetWithId(id, out var itemDef))
				return container.AddItem(itemDef.Id.ToString(), amount);

			return 0;
		}
		
		/// <summary>
		/// Removes an amount of items with the given name from this container.
		/// </summary>
		/// <param name="container"> Target container. </param>
		/// <param name="id"> Item definition name </param>
		/// <param name="amount"> Amount to remove </param>
		/// <returns> Removed count. </returns>
		public static int RemoveItem(this ItemContainer container, string id, int amount)
		{
			int removed = 0;
			var slots = container.Slots;

			foreach (var slot in slots)
			{
				if (slot.HasItem && slot.Item.Id.Equals(id))
				{
					removed += slot.Item.ChangeStack(-(amount - removed));

					// We've removed all the items, we can stop now
					if (removed == amount)
						return removed;
				}
			}

			return removed;
		}

		/// <summary>
		/// Counts and returns the amount of items with the given name.
		/// </summary>
		/// <param name="container"> Target container. </param>
		/// <param name="id"> Item definition name to search for. </param>
		/// <returns> Count. </returns>
		public static int GetItemCount(this ItemContainer container, string id)
		{
			int count = 0;
			var slots = container.Slots;

			foreach (var slot in slots)
			{
				if (slot.HasItem && slot.Item.Id.Equals(id))
					count += slot.Item.StackCount;
			}

			return count;
		}

		/// <summary>
		/// Finds and returns the index of the given slot, -1 otherwise.
		/// </summary>
		/// <param name="container"> Target container. </param>
		/// <param name="slot"> Target slot. </param>
		/// <returns> Index of given slot. </returns>
		public static int GetSlotIndex(this ItemContainer container, ItemSlot slot)
		{
			var slots = container.Slots;
			int count = slots.Count;

			for (int i = 0; i < count; i++)
			{
				if (slots[i] == slot)
					return i;
			}

			return -1;
		}

		/// <summary>
		/// Finds and returns the last empty slot, if there's no empty slot, it'll return the last slot instead.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public static ItemSlot GetLastEmptySlot(this ItemContainer container)
		{
			var slots = container.Slots;
			int count = slots.Count;

			for (int i = 0; i < count; i++)
			{
				if (!slots[i].HasItem)
					return slots[i];
			}

			return slots[count - 1];
		}

		public static bool AllowsItem(this ItemContainer container, Item item)
		{
			if (item == null)
				return false;

			return container.GetAllowedCount(item, item.StackCount) > 0;
		}
	}
}