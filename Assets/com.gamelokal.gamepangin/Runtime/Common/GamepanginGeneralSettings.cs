using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace Gamepangin
{
    [GlobalConfig("Assets/_Gamepangin/Resources")]
    public class GamepanginGeneralSettings : GlobalConfig<GamepanginGeneralSettings>
    {
#if UNITY_EDITOR
        [InlineButton(nameof(CreateNewMenuDatabase), "New")]
#endif
        public MenuDatabase menuDatabase;
        [FolderPath]
        public string[] databaseFolders;

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