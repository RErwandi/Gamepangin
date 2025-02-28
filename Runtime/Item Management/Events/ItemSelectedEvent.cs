namespace Gamepangin
{
    public struct ItemSelectedEvent
    {
        public Item item;
        public ItemContainer container;
        public int index;

        public ItemSelectedEvent(ItemContainer container, Item item, int index)
        {
            this.container = container;
            this.item = item;
            this.index = index;
        }

        private static ItemSelectedEvent itemSelectedEvent;

        public static void Trigger(ItemContainer container, Item item, int index)
        {
            itemSelectedEvent.item = item;
            itemSelectedEvent.container = container;
            itemSelectedEvent.index = index;
            EventManager.TriggerEvent(itemSelectedEvent);
        }
    }
}