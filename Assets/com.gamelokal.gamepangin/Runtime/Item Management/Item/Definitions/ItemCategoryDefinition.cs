using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamepangin
{
    [CreateAssetMenu(order = 0, fileName = "New Item Category", menuName = "Gamepangin/Inventory/Item Category")]
    public class ItemCategoryDefinition : DataDefinition<ItemCategoryDefinition>
    {
        [Title("Category")]
        [SerializeField, InlineButton("UpdateFilename")]
        [OnValueChanged(nameof(OnNameChanged))]
        private string categoryName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Color frameColor = Color.gray;
        [SerializeField] private Color frameTextColor = Color.white;
        [SerializeField] private ItemTagDefinition defaultTag;
        [SerializeField] private ItemAction[] baseActions = Array.Empty<ItemAction>();
        [ShowInInspector, ReadOnly] private List<ItemDefinition> itemsInCategory = new();

        public string Name => categoryName;
        public Sprite Icon => icon;
        public ItemAction[] BaseActions => baseActions;

#if UNITY_EDITOR
        private void UpdateFilename()
        {
            // Rename the ScriptableObject asset to match the string variable value.
            string assetPath = AssetDatabase.GetAssetPath(this);
            string newAssetPath = assetPath.Replace(name + ".asset", categoryName + ".asset");
            AssetDatabase.RenameAsset(assetPath, categoryName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                itemsInCategory.Clear();
                var allItems = DataDefinition<ItemDefinition>.Definitions;
                foreach (var item in allItems)
                {
                    if (item.Category == this)
                    {
                        itemsInCategory.Add(item);
                    }
                }
            };
        }
#endif
        
        private void OnNameChanged()
        {
            var formattedString = categoryName.ToLower().Replace(" ", "-");
            id = $"category-{formattedString}";
        }
    }
}