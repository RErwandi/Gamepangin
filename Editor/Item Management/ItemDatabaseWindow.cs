using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Gamepangin.Editor
{
    public class ItemDatabaseWindow : OdinMenuEditorWindow
    {
        [MenuItem("Gamepangin/Item Management")]
        private static void Open()
        {
            var window = GetWindow<ItemDatabaseWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;
            
            tree.AddAllAssetsAtPath("Items", GamepanginGeneralSettings.Instance.ItemsFolderPath, typeof(ItemDefinition), true, true);
            tree.EnumerateTree().AddIcons<ItemDefinition>(x => x.Icon);
            
            tree.AddAllAssetsAtPath("Categories", GamepanginGeneralSettings.Instance.ItemsFolderPath, typeof(ItemCategoryDefinition), true, true);
            tree.AddAllAssetsAtPath("Properties", GamepanginGeneralSettings.Instance.ItemsFolderPath, typeof(ItemPropertyDefinition), true, true);
            tree.AddAllAssetsAtPath("Tags", GamepanginGeneralSettings.Instance.ItemsFolderPath, typeof(ItemTagDefinition), true, true);

            tree.SortMenuItemsByName();

            return tree;
        }
        
        protected override void OnBeginDrawEditors()
        {
            if (MenuTree == null) return;
            
            var selected = MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }
                
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Item")))
                {
                    ScriptableObjectCreator.ShowDialog<ItemDefinition>(GamepanginGeneralSettings.Instance.ItemsFolderPath + "/ItemDefinition");
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Property")))
                {
                    ScriptableObjectCreator.ShowDialog<ItemPropertyDefinition>(GamepanginGeneralSettings.Instance.ItemsFolderPath + "/ItemPropertyDefinition");
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Tag")))
                {
                    ScriptableObjectCreator.ShowDialog<ItemTagDefinition>(GamepanginGeneralSettings.Instance.ItemsFolderPath + "/ItemTagDefinition");
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Category")))
                {
                        ScriptableObjectCreator.ShowDialog<ItemCategoryDefinition>(GamepanginGeneralSettings.Instance.ItemsFolderPath + "/ItemCategoryDefinition");
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}