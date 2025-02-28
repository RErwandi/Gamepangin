using UnityEngine;

namespace Gamepangin
{
    public abstract class ItemAction : ScriptableObject
    {
        public string actionName;
        public Sprite icon;

        public virtual bool IsPerformable(ItemSlot itemSlot)
        {
            return itemSlot != null && itemSlot.HasItem;
        }
        public abstract void PerformAction(ItemSlot itemSlot);
    }
}
