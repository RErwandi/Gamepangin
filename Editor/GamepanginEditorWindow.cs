using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Gamepangin.Editor
{
    public class GamepanginEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Gamepangin/Settings")]
        private static void OpenWindow()
        {
            var window = GetWindow<GamepanginEditorWindow>();
            window.minSize = new Vector2(800, 600);
            window.Show();
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Add("Editor", GamepanginGeneralSettings.Instance);
            return tree;
        }
    }
}