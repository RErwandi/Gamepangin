using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Gamepangin.Editor
{
    public class AudioClipEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Gamepangin/Audio Database")]
        private static void OpenWindow()
        {
            var window = GetWindow<AudioClipEditorWindow>();
            window.minSize = new Vector2(800, 600);
            window.Show();
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Config.DrawSearchToolbar = true;
            tree.AddAllAssetsAtPath("Audio", GamepanginGeneralSettings.Instance.AudioFolderPath, typeof(AudioClipSettings), true);
            return tree;
        }
    }
}