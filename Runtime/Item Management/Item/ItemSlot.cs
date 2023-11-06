using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    [Serializable]
    public sealed class ItemSlot
    {
        public enum CallbackType
        {
            ItemAdded,
            ItemRemoved,
            StackChanged,
            PropertyChanged
        }
        
        public struct CallbackContext
        {
            public readonly ItemSlot slot;
            public readonly CallbackType type;
            
            public CallbackContext(ItemSlot slot, CallbackType type)
            {
                this.slot = slot;
                this.type = type;
            }
        }

        [SerializeField] private Item item;
        
        public event UnityAction<CallbackContext> ItemChanged;
        
        public bool HasItem => item != null && item.Definition != null;
        public Item Item
        {
            get => item;
            set
            {
                if (item == value)
                    return;
				
                // Stop listening for changes to the previously attached item.
                if (item != null)
                {
                    item.PropertyChanged -= OnPropertyChanged;
                    item.StackCountChanged -= OnStackChanged;
                }

                item = value;

                // Start listening for changes to the newly attached item.
                if (item != null)
                {
                    item.PropertyChanged += OnPropertyChanged;
                    item.StackCountChanged += OnStackChanged;

                    ItemChanged?.Invoke(new CallbackContext(this, CallbackType.ItemAdded));
                }
                else
                    ItemChanged?.Invoke(new CallbackContext(this, CallbackType.ItemRemoved));
				
                void OnPropertyChanged() => ItemChanged?.Invoke(new CallbackContext(this, CallbackType.PropertyChanged));

                void OnStackChanged()
                {
                    if (item.StackCount == 0)
                    {
                        Item = null;
                        return;
                    }

                    ItemChanged?.Invoke(new CallbackContext(this, CallbackType.StackChanged));
                }
            }
        }
        
        private readonly ItemContainer container;
        public bool HasContainer => container != null;

        public ItemContainer Container
        {
            get
            {
                if (container == null)
                    Debug.LogError("This slot does not have a parent container.");

                return container;
            }
        }


        public ItemSlot(ItemContainer container)
        {
            this.item = null;
            this.container = container;
        }

        public ItemSlot(Item item, ItemContainer container)
        {
            this.item = item;
            this.container = container;
        }
    }
}