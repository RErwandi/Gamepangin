using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    [Serializable]
    public sealed class Item
    {
        [SerializeField] private string id;
        [SerializeField] private int stackCount;
        [SerializeField] private ItemDefinition definition;
        [SerializeField] private List<ItemProperty> properties;

        public string Id => id;
        public string Name => definition.Name;
        public ItemDefinition Definition => definition;
        public List<ItemProperty> Properties => properties;

        public int StackCount
        {
            get => stackCount;
            set
            { 
                int oldStack = stackCount;
                stackCount = Mathf.Clamp(value, 0, definition.StackSize);
				
                StackCountChanged?.Invoke();
            } 
        }
        
        public float TotalWeight
        {
            get
            {
                if (Properties == null)
                {
                    return Definition.Weight * stackCount;
                }

                float weight = Definition.Weight;
                return weight * stackCount;
            }
        }
        
        public event UnityAction PropertyChanged;
        public event UnityAction StackCountChanged;


        /// <summary>
        /// Constructor that requires an item definition.
        /// </summary>
        public Item(ItemDefinition itemDef, int count = 1)
        {
            definition = itemDef;

            if (itemDef == null)
                throw new NullReferenceException("Cannot create an item from a null item definition.");

            id = itemDef.Id;
            stackCount = Mathf.Clamp(count, 1,  definition.StackSize);

            properties = CloneProperties(itemDef.Properties);

            // Listen to the property changed callbacks.
            foreach (var prop in properties)
                prop.PropertyChanged += OnPropertyChanged;

            void OnPropertyChanged(ItemProperty property) => PropertyChanged?.Invoke();
        }
        
        /// <summary>
        /// Constructor that requires an item definition and an array of custom properties.
        /// </summary>
        public Item(ItemDefinition itemDef, int count, List<ItemProperty> customProperties)
        {
            definition = itemDef;

            if (itemDef == null)
                throw new NullReferenceException("Cannot create an item from a null item definition.");

            id = itemDef.Id;
            stackCount = Mathf.Clamp(count, 1,  definition.StackSize);

            properties = CloneProperties(customProperties);

            // Listen to the property changed callbacks.
            foreach (var property in properties)
                property.PropertyChanged += OnPropertyChanged;

            void OnPropertyChanged(ItemProperty property) => PropertyChanged?.Invoke();
        }
        
        private List<ItemProperty> CloneProperties(IReadOnlyList<ItemProperty> properties)
        {
            return properties.Select(prop => prop.GetClone()).ToList();
        }
    }
}