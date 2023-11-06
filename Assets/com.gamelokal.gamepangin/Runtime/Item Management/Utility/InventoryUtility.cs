using System.Collections.Generic;

namespace Gamepangin
{
    public static class InventoryUtility
    {
	    /// <summary>
	    /// Returns a container with the given name.
	    /// </summary>
	    public static ItemContainer GetContainerWithName(this IInventory inventory, string name)
	    {
		    var containers = inventory.Containers;
		    
		    foreach (var container in containers)
		    {
			    if (container.Name == name)
				    return container;
		    }

		    return null;
	    }
	    
	    /// <summary>
	    /// Adds an amount of items with the given name to the inventory.
	    /// </summary>
	    public static int AddItemsWithId(this IInventory inventory, string id, int amountToAdd)
	    {
		    if (amountToAdd <= 0)
			    return 0;
		    
		    int addedInTotal = 0;
		    var containers = inventory.Containers;
		    
		    foreach (var container in containers)
		    {
			    int added = container.AddItem(id, amountToAdd);
			    addedInTotal += added;

			    if (added == addedInTotal)
				    return addedInTotal;
		    }

		    return addedInTotal;
	    }
	    
	    /// <summary>
	    /// Removes an amount of items with the given name from the inventory.
	    /// </summary>
	    public static int RemoveItemsWithId(this IInventory inventory, string id, int amountToRemove)
	    {
		    if (amountToRemove <= 0)
			    return 0;
		    
		    int removedInTotal = 0;
		    var containers = inventory.Containers;

		    foreach (var container in containers)
		    {
			    int removedNow = container.RemoveItem(id, amountToRemove);
			    removedInTotal += removedNow;

			    if (removedNow == removedInTotal)
				    return removedInTotal;
		    }

		    return removedInTotal;
	    }
	    
	    /// <summary>
	    /// Counts all the items with the given name, in all containers.
	    /// </summary>
	    public static int GetItemsWithIdCount(this IInventory inventory, string id)
	    {
		    int count = 0;
		    var containers = inventory.Containers;

		    foreach (var container in containers)
			    count += container.GetItemCount(id);

		    return count;
	    }
	    
		/// <summary>
		/// Returns a list of the containers with the given container restriction type.
		/// </summary>
		public static List<ItemContainer> GetContainersWithRestriction<T>(this IInventory inventory) where T : ContainerRestriction
		{
			var containersWithRestriction = new List<ItemContainer>();

			foreach (var container in inventory.Containers)
			{
				if (container.HasContainerRestriction<T>())
					containersWithRestriction.Add(container);
			}

			return containersWithRestriction;
		}

		/// <summary>
		/// Returns a list of the containers with the given container restriction type and a matching list of the restrictions.
		/// </summary>
		public static List<ItemContainer> GetContainersWithRestriction<T>(this IInventory inventory, out List<T> restrictions) where T : ContainerRestriction
		{
			var containersWithRestriction = new List<ItemContainer>();
			restrictions = new List<T>();

			foreach (var container in inventory.Containers)
			{
				if (container.TryGetContainerRestriction<T>(out T restriction))
				{
					containersWithRestriction.Add(container);
					restrictions.Add(restriction);
				}
			}

			return containersWithRestriction;
		}

		/// <summary>
		/// Returns a list of all of the containers in this inventory that are not implmenting a restriction of type T.
		/// </summary>
		public static List<ItemContainer> GetContainersWithoutRestriction<T>(this IInventory inventory) where T : ContainerRestriction
		{
			var containersWithoutRestrictions = new List<ItemContainer>();
			var containers = inventory.Containers;

			int count = containers.Count;
            for (int i = 0; i < count; i++)
            {
				if (!containers[i].HasContainerRestriction<T>())
					containersWithoutRestrictions.Add(containers[i]);
            }

			return containersWithoutRestrictions;
		}

		/// <summary>
		/// Returns a list of containers with the given tag.
		/// </summary>
		public static List<ItemContainer> GetContainersWithTag(this IInventory inventory, ItemTagDefinition tag)
		{
			var containers = inventory.GetContainersWithRestriction<ContainerTagRestriction>(out var tagRestrictions);

			int index = 0;

			while (index < containers.Count)
			{
				if (!tagRestrictions[index].ValidTags.Contains(tag))
				{
					containers.RemoveAt(index);
					tagRestrictions.RemoveAt(index);
				}
				else
					index++;
			}

			return containers;
		}

		/// <summary>
		/// Returns a list of containers without a tag.
		/// </summary>
		public static List<ItemContainer> GetContainersWithoutTag(this IInventory inventory)
		{
			var containersWithoutTag = new List<ItemContainer>();
			var containers = inventory.Containers;

            foreach (var container in containers)
            {
                if (!container.TryGetContainerRestriction(out ContainerTagRestriction tagRestriction) || !tagRestriction.HasValidTags)
                    containersWithoutTag.Add(container);
            }

            return containersWithoutTag;
		}
	}
}