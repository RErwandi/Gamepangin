using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamepangin
{
    [Serializable]
    public sealed class ContainerPropertyRestriction : ContainerRestriction
    {
        public List<ItemPropertyDefinition> RequiredProperties => requiredProperties;

        [SerializeField]
        private List<ItemPropertyDefinition> requiredProperties;


        public ContainerPropertyRestriction(List<ItemPropertyDefinition> requiredProperties)
        {
            this.requiredProperties = requiredProperties;
        }

        public override int GetAllowedAddAmount(Item item, int count)
        {
            bool isValid = true;
            var def = item.Definition;

            foreach (var property in requiredProperties)
                isValid &= def.HasProperty(property.Id);

            if (!isValid)
                return 0;

            return count;
        }

        public override int GetAllowedRemoveAmount(Item def, int count) => count;
    }
}