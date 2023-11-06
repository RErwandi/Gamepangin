using System;

namespace Gamepangin
{
    [Serializable]
    public sealed class ContainerRemoveRestriction : ContainerRestriction
    {
        public override int GetAllowedAddAmount(Item item, int count) => count;
        public override int GetAllowedRemoveAmount(Item item, int count) => 0;
    }
}
