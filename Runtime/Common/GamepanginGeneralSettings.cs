using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace Gamepangin
{
    [GlobalConfig("Assets/_Gamepangin/Resources")]
    public class GamepanginGeneralSettings : GlobalConfig<GamepanginGeneralSettings>
    {
        [FolderPath]
        public string[] databaseFolders;
    }
}