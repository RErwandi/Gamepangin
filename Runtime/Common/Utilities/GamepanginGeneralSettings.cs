using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Gamepangin
{
    [GlobalConfig("Assets/_Gamepangin/Resources")]
    public class GamepanginGeneralSettings : GlobalConfig<GamepanginGeneralSettings>
    {
        
        [FolderPath, SerializeField] private string audioFolderPath;
        public string AudioFolderPath => string.IsNullOrEmpty(audioFolderPath) ? "_Gamepangin/Resources/AudioClipData" : audioFolderPath;
        
        [FolderPath, SerializeField] private string itemsFolderPath;
        public string ItemsFolderPath => string.IsNullOrEmpty(itemsFolderPath) ? "_Gamepangin/Resources/Items" : itemsFolderPath;
    }
}