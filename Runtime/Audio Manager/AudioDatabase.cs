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
            string[] itemsGuids = AssetDatabase.FindAssets($"t:{nameof(AudioClipSettings)}", GamepanginGeneralSettings.Instance.databaseFolders);
            AudioClipSettings[] clips = new AudioClipSettings[itemsGuids.Length];

            for (int i = 0; i < itemsGuids.Length; i++)
            {
                string itemsGuid = itemsGuids[i];
                string itemPath = AssetDatabase.GUIDToAssetPath(itemsGuid);
                clips[i] = AssetDatabase.LoadAssetAtPath<AudioClipSettings>(itemPath);
            }

            Get().Clips = clips;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}