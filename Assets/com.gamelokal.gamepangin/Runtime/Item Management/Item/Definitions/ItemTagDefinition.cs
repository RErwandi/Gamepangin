using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gamepangin
{
    [CreateAssetMenu(order = 0, fileName = "New Item Tag", menuName = "Gamepangin/Inventory/Item Tag")]
    public class ItemTagDefinition : DataDefinition<ItemTagDefinition>
    {
        [InlineButton("UpdateFilename")]
        public string tagName;

#if UNITY_EDITOR
        private void UpdateFilename()
        {
            // Rename the ScriptableObject asset to match the string variable value.
            string assetPath = AssetDatabase.GetAssetPath(this);
            string newAssetPath = assetPath.Replace(name + ".asset", tagName + ".asset");
            AssetDatabase.RenameAsset(assetPath, tagName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
}