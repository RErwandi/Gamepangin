namespace Gamepangin
{
    [System.Serializable]
    public abstract class ContainerRestriction
    {
        protected ItemContainer itemContainer;
        
        /// <summary>
        /// Initializes this restriction.
        /// </summary>
        /// <param name="container"></param>
        public virtual void OnInitialized(ItemContainer container) => itemContainer = container;

        /// <summary>
        /// Returns the amount of items that can be added of the given item and count.
        /// </summary>
        public abstract int GetAllowedAddAmount(Item item, int count);
        
        /// <summary>
        /// Returns the amount of items that can be removed of the given item and count.
        /// </summary>
        public abstract int GetAllowedRemoveAmount(Item item, int count);

        /// <summary>
        /// Returns a string that specifies why the item cannot be added.
        /// </summary>
        public virtual string GetRejectionString() => "Item is not valid";
    }
}