using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamepangin
{
    [CreateAssetMenu(order = 0, fileName = "New Item", menuName = "Gamepangin/Inventory/Item")]
    public sealed class ItemDefinition : DataDefinition<ItemDefinition>
    {
        private const string LEFT_VERTICAL_GROUP             = "Split/Left";
        private const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";
        private const string INVENTORY_BOX_GROUP                 = "Split/Left/Inventory";
        
        [HideLabel, PreviewField(55)]
        [VerticalGroup(LEFT_VERTICAL_GROUP)]
        [HorizontalGroup(LEFT_VERTICAL_GROUP + "/General Settings/Split", 55, LabelWidth = 67)]
        [SerializeField] private Sprite icon;
        
        [BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
        [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        [SerializeField, InlineButton("UpdateFilename")] private string itemName;

        [AssetsOnly]
        [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        [SerializeField]
        private GameObject itemPrefab;
        
        [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        //[ValueDropdown(Constants.EDITOR_ALL_ITEM_CATEGORY, FlattenTreeView = true)]
        [SerializeField, Required] private ItemCategoryDefinition category;
        
        [VerticalGroup("Split/Right")]
        [BoxGroup("Split/Right/Description")]
        [HideLabel, TextArea(4, 14)]
        [SerializeField] private string description;
        
        [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
        [BoxGroup("Split/Right/Notes")]
        [HideLabel, TextArea(4, 9)]
        public string notes;
        
        [BoxGroup(INVENTORY_BOX_GROUP)]
        [Range(1, 1000)]
        [SerializeField] private int stackSize = 99;
        
        [BoxGroup(INVENTORY_BOX_GROUP)]
        [Range(0.01f, 10f)]
        [SerializeField] private float weight = 0.01f;
        
        [BoxGroup(INVENTORY_BOX_GROUP)]
        //[ValueDropdown(Constants.EDITOR_ALL_ITEM_TAG, FlattenTreeView = true)]
        [SerializeField] private ItemTagDefinition tag;

        [InfoBox("Simple data that can be changed at runtime (not shared between item instances)")]
        [VerticalGroup("Split/Left")]
        [SerializeField] private List<ItemProperty> properties = new();
        
        [InfoBox("Complex data, shared between every item instance of this type")]
        [VerticalGroup("Split/Left")]
        [InlineProperty]
        [SerializeReference] private List<ItemData> itemDatas = new();

        public string Name => itemName;
        public Sprite Icon => icon;
        public string Description => description;
        public GameObject Prefab => itemPrefab;
        public int StackSize => stackSize;
        public float Weight => weight;
        public ItemCategoryDefinition Category => category;
        public List<ItemProperty> Properties => properties;
        public ItemTagDefinition Tag => tag;
        public List<ItemData> ItemDatas => itemDatas;

        #region Item Tag

        public static List<ItemDefinition> GetAllItemsWithTag(ItemTagDefinition tag)
        {
            if (tag == null) return null;
            string tagId = tag.Id;
            if (tagId.Equals("")) return null;

            List<ItemDefinition> items = new List<ItemDefinition>();

            // foreach (var item in Definitions)
            // {
            //     if (item.m_Tag == tagId)
            //         items.Add(item);
            // }

            return items;
        }
        
        #endregion

        #region Item Data

        /// <summary>
        /// Returns all of the custom data present on this item.
        /// </summary>
        public List<ItemData> GetAllData() => ItemDatas;
        
        /// <summary>
        /// Tries to return an item data of type T.
        /// </summary>
        public bool TryGetDataOfType<T>(out T data) where T : ItemData
        {
            var targetType = typeof(T);

            foreach (var itemData in itemDatas)
            {
                if (itemData.GetType() == targetType)
                {
                    data = (T)itemData;
                    return true;
                }
            }

            data = null;
            return false;
        }
        
        /// <summary>
        /// Returns an item data of the given type (if available).
        /// </summary>
        public T GetDataOfType<T>() where T : ItemData
        {
            var targetType = typeof(T);

            foreach (var itemData in itemDatas)
            {
                if (itemData.GetType() == targetType)
                    return (T)itemData;
            }

            return null;
        }
        
        /// <summary>
        /// Checks if this item has an item data of type T attached.
        /// </summary>
        public bool HasDataOfType(Type type)
        {
            foreach (var itemData in itemDatas)
            {
                if (itemData.GetType() == type)
                    return true;
            }

            return false;
        }
        
        public bool TryGetListDataOfType<T>(out List<T> data) where T : ItemData
        {
            var targetType = typeof(T);
            data = itemDatas.Where(itemData => itemData.GetType() == targetType).Cast<T>().ToList();

            return data.Count > 0;
        }

        public List<T> GetListDataOfType<T>() where T : ItemData
        {
            var targetType = typeof(T);
            return itemDatas.Where(itemData => itemData.GetType() == targetType).Cast<T>().ToList();
        }
        
        #endregion
        
        #region Item Properties
        
        public bool HasProperty(string propertyId)
        {
            foreach (var prop in Properties)
            {
                if (prop.Id.Equals(propertyId))
                    return true;
            }

            return false;
        }
        
        #endregion

#if UNITY_EDITOR
        private void UpdateFilename()
        {
            // Rename the ScriptableObject asset to match the string variable value.
            string assetPath = AssetDatabase.GetAssetPath(this);
            string newAssetPath = assetPath.Replace(name + ".asset", itemName + ".asset");
            AssetDatabase.RenameAsset(assetPath, itemName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
}