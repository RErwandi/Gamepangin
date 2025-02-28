using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gamepangin
{
    [CreateAssetMenu(order = 0, fileName = "New Item Property", menuName = "Gamepangin/Inventory/Item Property")]
    public class ItemPropertyDefinition : DataDefinition<ItemPropertyDefinition>
    {
        [InlineButton("UpdateFilename"), OnValueChanged(nameof(OnNameChanged))]
        public string propertyName;
        public string description;
        public ItemPropertyType propertyType;

#if UNITY_EDITOR
        private void UpdateFilename()
        {
            // Rename the ScriptableObject asset to match the string variable value.
            string assetPath = AssetDatabase.GetAssetPath(this);
            string newAssetPath = assetPath.Replace(name + ".asset", propertyName + ".asset");
            AssetDatabase.RenameAsset(assetPath, propertyName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
        
        private void OnNameChanged()
        {
            var formattedString = propertyName.ToLower().Replace(" ", "-");
            id = $"property-{formattedString}";
        }
    }
}