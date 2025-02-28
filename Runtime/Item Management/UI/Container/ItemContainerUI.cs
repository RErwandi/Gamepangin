using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin.UI
{
    public class ItemContainerUI : MonoBehaviour
    {
        public ItemContainer ItemContainer
        {
            get
            {
                if (isAttached)
                    return itemContainer;
                else
                {
                    Debug.LogError("There's no item container linked. Can't retrieve any!");
                    return null;
                }
            }
        }

        public List<ItemSlotUI> Slots => slots;
        public bool IsAttached => isAttached;

        [SerializeField] private Inventory inventory;
        [SerializeField] private string containerName;
        [SerializeField, Required] private RectTransform root;
        [SerializeField, Required] private ItemSlotUI slotTemplate;

        private ItemContainer itemContainer;
        private List<ItemSlotUI> slots;
        private bool isAttached;

        public UnityAction<ItemContainer> onContainerAttached;

        private void Start()
        {
	        if (inventory != null)
	        {
		        var container = inventory.GetContainerWithName(containerName);
		        AttachToContainer(container);
	        }
        }

        public virtual void AttachToContainer(ItemContainer container)
        {
            if (container == null)
            {
                Debug.Log($"Cannot attach a null container to {gameObject.name}");
                return;
            }

            if (slots == null || slots.Count != container.Capacity)
                GenerateSlots(container.Capacity);

            itemContainer = container;
	        SetItems();

            isAttached = true;
            onContainerAttached?.Invoke(itemContainer);
        }

        public virtual void DetachFromContainer()
        {
            if (itemContainer == null)
                return;

            for (int i = 0; i < slots.Count; i++)
                slots[i].SetItemSlot(null, i);

            isAttached = false;
        }

        protected void SetItems()
        {
	        if (itemContainer == null)
	        {
		        Debug.LogError("Item container is null", gameObject);
		        return;
	        }

	        int iSlot = 0;
	        for (int i = 0; i < itemContainer.Capacity; i++)
	        {
		        if (CanBeShown(itemContainer[i]))
		        {
			        slots[iSlot].SetItemSlot(itemContainer[i], iSlot);
			        iSlot++;
		        }
		        else
		        {
			        slots[i].SetItemSlot(null, iSlot);
		        }
	        }
        }

        protected virtual bool CanBeShown(ItemSlot itemSlot)
        {
	        return true;
        }
		
		[Button(ButtonSizes.Medium, Name = "Generate Default Slots")]
		public void GenerateSlots(int count)
		{
			if (count < 0 || count > 100)
				throw new System.IndexOutOfRangeException();

			// Get the children slots.
			if (slots == null || !Application.isPlaying)
			{
				var firstSlot = root.gameObject.GetComponentsInFirstChildren<ItemSlotUI>();
				slots = firstSlot;
			}

			if (count == slots.Count)
			{
				return;
			}

			if (count < slots.Count)
			{
				int slotsToDestroy = slots.Count - count;
				int indexToDestroy = 0;
				while (indexToDestroy < slotsToDestroy)
				{
#if UNITY_EDITOR
					if (!Application.isPlaying)
					{
						DestroyImmediate(slots[indexToDestroy].gameObject);
						indexToDestroy++;
						continue;
					}
#endif
					Destroy(slots[indexToDestroy].gameObject);
					indexToDestroy++;
				}

				slots.RemoveRange(0, slotsToDestroy);
				return;
			}

			if (count > slots.Count)
			{
				if (slotTemplate == null)
				{
					Debug.LogError("No slot template is provided, can't generate any slots.", gameObject);
					return;
				}

				int slotsToCreate = count - slots.Count;

#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					EditorUtility.SetDirty(this);
					for (int i = 0; i < slotsToCreate; i++)
					{
						ItemSlotUI slotInterface = PrefabUtility.InstantiatePrefab(slotTemplate, root) as ItemSlotUI;
						slotInterface.gameObject.SetActive(true);
						slotInterface.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
						EditorUtility.SetDirty(slotInterface);
					}

					return;
				}
#endif

				slots.Capacity = count;
				for (int i = 0; i < slotsToCreate; i++)
				{
					ItemSlotUI slotInterface = Instantiate(slotTemplate, root);
					slotInterface.gameObject.SetActive(true);
					slotInterface.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
					slots.Add(slotInterface);
				}
			}
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (root == null)
				root = transform as RectTransform;
		}
#endif
	}
}