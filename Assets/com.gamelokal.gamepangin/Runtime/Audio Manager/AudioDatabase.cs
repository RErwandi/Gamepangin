#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamepangin
{
    public class AudioDatabase : AssetRepository<AudioRepository>
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
            string[] itemsGuids = AssetDatabase.FindAssets($"t:{nameof(AudioClipData)}", GamepanginGeneralSettings.Instance.databaseFolders);
            AudioClipData[] clips = new AudioClipData[itemsGuids.Length];

            for (int i = 0; i < itemsGuids.Length; i++)
            {
                string itemsGuid = itemsGuids[i];
                string itemPath = AssetDatabase.GUIDToAssetPath(itemsGuid);
                clips[i] = AssetDatabase.LoadAssetAtPath<AudioClipData>(itemPath);
            }

            Get().Clips = clips;
        }
#endif
    }
}