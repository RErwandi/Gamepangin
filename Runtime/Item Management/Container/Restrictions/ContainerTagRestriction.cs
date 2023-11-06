using System.Collections.Generic;
using UnityEngine;

namespace Gamepangin
{
    [System.Serializable]
    public sealed class ContainerTagRestriction : ContainerRestriction
    {
        public bool HasValidTags => validTags.Count > 0;
        public List<ItemTagDefinition> ValidTags => validTags;

        [SerializeField] private List<ItemTagDefinition> validTags;



        public ContainerTagRestriction(List<ItemTagDefinition> validTags)
        {
            this.validTags = validTags;
        }

        public override int GetAllowedAddAmount(Item item, int count)
        {
            var defTag = item.Definition.Tag;
            if (defTag == null) return 0;

            foreach (var tag in validTags)
            {
                if (defTag.Id.Equals(tag.Id))
                    return count;
            }

            return 0;
        }

        public override int GetAllowedRemoveAmount(Item item, int count) => count;
    }
}