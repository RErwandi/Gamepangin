using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gamepangin
{
    [Serializable]
    public class MenuRepository : TRepository<MenuRepository>
    {
        public override string RepositoryId => "Menu.assets";

        [SerializeField] private Menu[] menus;

        public Menu[] Menus
        {
            get => menus;
            set => menus = value;
        }

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode() => Instance = null;
#endif
    }
}