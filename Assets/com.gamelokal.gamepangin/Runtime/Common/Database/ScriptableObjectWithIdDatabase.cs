#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamepangin
{
    public class ScriptableObjectWithIdDatabase : AssetRepository<ScriptableObjectWithIdRepository>
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
            var itemsGuids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObjectWithId)}", GamepanginGeneralSettings.Instance.databaseFolders);
            var so = new ScriptableObjectWithId[itemsGuids.Length];

            for (int i = 0; i < itemsGuids.Length; i++)
            {
                var itemsGuid = itemsGuids[i];
                var itemPath = AssetDatabase.GUIDToAssetPath(itemsGuid);
                so[i] = AssetDatabase.LoadAssetAtPath<ScriptableObjectWithId>(itemPath);
            }

            Get().ScriptableObjects = so;
        }
#endif
    }
}