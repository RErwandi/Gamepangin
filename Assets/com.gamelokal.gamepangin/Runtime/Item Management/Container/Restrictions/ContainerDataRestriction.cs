using System;

namespace Gamepangin
{
    [Serializable]
    public sealed class ContainerDataRestriction : ContainerRestriction
    {
        private Type type;
        
        public ContainerDataRestriction(Type type)
        {
            this.type = type;
        }

        public override int GetAllowedAddAmount(Item item, int count) => item.Definition.HasDataOfType(type) ? count : 0;
        public override int GetAllowedRemoveAmount(Item item, int count) => count;
    }
}