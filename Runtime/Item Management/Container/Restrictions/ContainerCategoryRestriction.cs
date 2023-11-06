using UnityEngine;

namespace Gamepangin
{
    [System.Serializable]
    public sealed class ContainerCategoryRestriction : ContainerRestriction
    {
        public ItemCategoryDefinition[] ValidCategories => validCategories;

        [SerializeField]
        private ItemCategoryDefinition[] validCategories;


        public ContainerCategoryRestriction(ItemCategoryDefinition[] validCategories)
        {
            this.validCategories = validCategories;
        }

        public override int GetAllowedAddAmount(Item item, int count)
        {
            if (validCategories == null) 
                return count;
            
            var def = item.Definition;
            bool isValid = false;

            foreach (var category in validCategories)
                isValid |= def.Category == category;

            return isValid ? count : 0;
        }

        public override int GetAllowedRemoveAmount(Item def, int count) => count;
    }
}