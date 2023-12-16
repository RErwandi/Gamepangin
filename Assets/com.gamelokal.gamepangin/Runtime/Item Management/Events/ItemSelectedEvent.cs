namespace Gamepangin
{
    public struct ItemSelectedEvent
    {
        public Item item;

        public ItemSelectedEvent(Item item)
        {
            this.item = item;
        }

        private static ItemSelectedEvent itemSelectedEvent;

        public static void Trigger(Item item)
        {
            itemSelectedEvent.item = item;
            EventManager.TriggerEvent(itemSelectedEvent);
        }
    }
}