#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamepangin
{
    public class MenuDatabase : AssetRepository<MenuRepository>
    {
#if UNITY_EDITOR
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnChangePlayMode;
            EditorApplication.playModeStateChanged += OnChangePlayMode;
            
            RefreshItemList();
        }

        private void OnChangePlayMode(PlayModeStateChange playModeStateChange)
        {
            RefreshItemList();
        }

        private void RefreshItemList()
        {
            string[] itemsGuids = AssetDatabase.FindAssets($"t:{nameof(Menu)}", GamepanginGeneralSettings.Instance.databaseFolders);
            Menu[] menus = new Menu[itemsGuids.Length];

            for (int i = 0; i < itemsGuids.Length; i++)
            {
                string itemsGuid = itemsGuids[i];
                string itemPath = AssetDatabase.GUIDToAssetPath(itemsGuid);
                menus[i] = AssetDatabase.LoadAssetAtPath<Menu>(itemPath);
            }

            Get().Menus = menus;
        }
#endif
    }
}