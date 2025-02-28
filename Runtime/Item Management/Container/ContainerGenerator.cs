using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    [Serializable]
    public class ContainerGenerator
    {
        public string Name => containerName;

        [Title("General")] [SerializeField] [Tooltip("The name of the item container.")]
        private string containerName;

        [SerializeField, Range(1, 100),]
        [Tooltip("Number of item slots that this container has (e.g. Holster 8, Backpack 25 etc.).")]
        private int maxSize = 1;

        [SerializeField, Range(-1, 100)]
        [Tooltip("The max weight that this container can hold, no item can be added if it exceeds the limit.")]
        private int maxWeight = 30;

        [Title("Item Filtering")]
        [SerializeField]
        [Tooltip("Only items that are tagged with the specified tag can be added.")]
        private List<ItemTagDefinition> validTags = new();

        [SerializeField] [Tooltip("Only items with the specified properties can be added.")]
        private List<ItemPropertyDefinition> requiredProperties = new();
        
        [Title("Default Items")]
        [SerializeField] private List<ItemHolder> defaultItems = new();


        public ItemContainer GenerateContainer()
        {
            var container = new ItemContainer(
                containerName,
                maxSize,
                GetAllRestrictions(),
                defaultItems
            );

            return container;
        }

        private List<ContainerRestriction> GetAllRestrictions()
        {
            var restrictions = new List<ContainerRestriction>();

            if (validTags.Count > 0)
                restrictions.Add(new ContainerTagRestriction(validTags));

            if (requiredProperties.Count > 0)
                restrictions.Add(new ContainerPropertyRestriction(requiredProperties));

            if (maxWeight != -1)
                restrictions.Add(new ContainerWeightRestriction((float)maxWeight / (float)maxSize));

            return restrictions;
        }
    }
}