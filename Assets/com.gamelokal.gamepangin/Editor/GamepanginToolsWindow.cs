using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Gamepangin.Editor
{
    public class GamepanginToolsWindow : OdinMenuEditorWindow
    {
        [MenuItem("Gamepangin/Tools")]
        private static void OpenWindow()
        {
            var window = GetWindow<GamepanginToolsWindow>();
            window.minSize = new Vector2(800, 600);
            window.Show();
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Add("Editor", GamepanginGeneralSettings.Instance);
            tree.Add("Bootstrapper", BootstrapperConfig.Instance);
            tree.Add("Batch Texture Optimization", BatchTextureOptimization.Instance);
            return tree;
        }
    }
}