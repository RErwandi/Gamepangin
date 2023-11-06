using System;

namespace Gamepangin
{
    [Serializable]
    public sealed class ContainerAddRestriction : ContainerRestriction
    {
        public override int GetAllowedAddAmount(Item item, int count) => 0;
        public override int GetAllowedRemoveAmount(Item item, int count) => count;
    }
}
