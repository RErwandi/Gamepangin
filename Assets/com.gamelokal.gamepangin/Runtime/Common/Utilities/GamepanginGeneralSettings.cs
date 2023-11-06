using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Gamepangin
{
    [GlobalConfig("Assets/_Gamepangin/Resources")]
    public class GamepanginGeneralSettings : GlobalConfig<GamepanginGeneralSettings>
    {
#if UNITY_EDITOR
        [InlineButton(nameof(CreateNewMenuDatabase), "New")]
#endif
        public MenuDatabase menuDatabase;
        
        [FolderPath, SerializeField] private string audioFolderPath;
        public string AudioFolderPath => string.IsNullOrEmpty(audioFolderPath) ? "_Gamepangin/Resources/AudioClipData" : audioFolderPath;
        
        [FolderPath, SerializeField] private string itemsFolderPath;
        public string ItemsFolderPath => string.IsNullOrEmpty(itemsFolderPath) ? "_Gamepangin/Resources/Items" : itemsFolderPath;

#if UNITY_EDITOR
        private void CreateNewMenuDatabase()
        {
            ScriptableObjectCreator.ShowDialog<MenuDatabase>("Assets/", OnSuccessCreateNewDatabase);
        }

        private void OnSuccessCreateNewDatabase(MenuDatabase so)
        {
            menuDatabase = so;
        }
#endif
    }
}