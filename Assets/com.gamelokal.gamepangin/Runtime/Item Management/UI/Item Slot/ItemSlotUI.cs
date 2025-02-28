using UnityEngine;
using UnityEngine.EventSystems;

namespace Gamepangin.UI
{
    public class ItemSlotUI : SlotUI<Item>, IBeginDragHandler, IDragHandler, IEndDragHandler, ISelectHandler
    {
        [SerializeField] private RectTransform dragTarget, dragParent;
        [SerializeField] private CanvasGroup dragCanvasGroup;

        public Item Item => itemSlot?.Item;
        public ItemContainer Container => itemSlot?.Container;

        public bool HasItem => Item != null;
        public bool HasItemSlot => itemSlot != null;
        public bool HasContainer => HasItemSlot && itemSlot.HasContainer;
        public int SlotIndex { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set => isSelected = value;
        }

        private ItemSlot itemSlot;
        private bool isSelected;
        private Transform canvasTransform;
        private Vector2 originalPosition;

        private void Start()
        {
            canvasTransform = GetComponentInParent<Canvas>().transform;
        }

        public void SetItemSlot(ItemSlot itemSlot, int index)
        {
            SlotIndex = index;

            if (this.itemSlot == itemSlot)
                return;

            if (this.itemSlot != null)
                this.itemSlot.ItemChanged -= OnSlotChanged;

            this.itemSlot = itemSlot;

            if (this.itemSlot != null)
            {
                SetData(this.itemSlot.Item);
                this.itemSlot.ItemChanged += OnSlotChanged;
            }
            else
                SetData(null);

            void OnSlotChanged(ItemSlot.CallbackContext context)
            {
                SetData(this.itemSlot.Item);
                if (isSelected)
                {
                    ItemSelectedEvent.Trigger(Container, Item, SlotIndex);
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!itemSlot.HasItem) return;

            originalPosition = dragTarget.anchoredPosition;
            dragCanvasGroup.blocksRaycasts = false;
            dragCanvasGroup.alpha = 0.7f;
            dragTarget.anchoredPosition = eventData.position;
            dragTarget.SetParent(canvasTransform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!itemSlot.HasItem) return;

            dragTarget.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!itemSlot.HasItem) return;

            dragCanvasGroup.blocksRaycasts = true;
            dragCanvasGroup.alpha = 1f;

            //Check if we are dropping over a slot
            if (eventData.pointerEnter != null)
            {
                ItemSlotUI slot = eventData.pointerEnter.GetComponentInParent<ItemSlotUI>();
                if (slot != null)
                {
                    var targetItemSlot = slot.itemSlot;
                    (targetItemSlot.Item, itemSlot.Item) = (itemSlot.Item, targetItemSlot.Item);
                }
                
                // Drop Item if dragged to GameObject with ItemDropUI.cs
                ItemDropUI drop = eventData.pointerEnter.GetComponentInParent<ItemDropUI>();
                if (drop != null)
                {
                    drop.Drop(Container, SlotIndex);
                }
            }

            dragTarget.SetParent(dragParent);
            dragTarget.anchoredPosition = originalPosition;
        }

        public void OnSelect(BaseEventData eventData)
        {
            isSelected = true;
            
            ItemSelectedEvent.Trigger(Container, Item, SlotIndex);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            isSelected = false;
        }
    }
}