using System.Collections.Generic;
using UnityEngine.Events;

namespace Gamepangin
{
    public interface IInventory
    {
        List<ItemContainer> Containers { get; }
        
        event UnityAction InventoryChanged;
        event UnityAction<ItemSlot.CallbackContext> ItemChanged;
        event UnityAction ContainersCountChanged;

        /// <summary>
        /// Adds a container to this inventory.
        /// </summary>
        void AddContainer(ItemContainer itemContainer, bool triggerContainersEvent = true);

        /// <summary>
        /// Removes a container from this inventory.
        /// </summary>
        void RemoveContainer(ItemContainer itemContainer, bool triggerContainersEvent = true);

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <returns> stack Added Count. </returns>
        int AddItem(Item item);

        /// <summary>
        /// Adds an amount of items with the given id to the inventory.
        /// </summary>
        int AddItemsWithId(string itemId, int amountToAdd);

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        bool RemoveItem(Item item);

        /// <summary>
        /// Removes an amount of items with the given id from the inventory.
        /// </summary>
        int RemoveItemsWithId(string itemId, int amountToRemove);

        /// <summary>
        /// Counts all the items with the given id, in all containers.
        /// </summary>
        int GetItemsWithIdCount(string itemId);

        /// <summary>
        /// Returns true if the item is found in any of the child containers.
        /// </summary>
        bool ContainsItem(Item item);
    }
}