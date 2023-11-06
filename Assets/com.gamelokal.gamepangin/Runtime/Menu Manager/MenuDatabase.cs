using System.Collections.Generic;
using UnityEngine;

namespace Gamepangin 
{
    public class MenuDatabase : ScriptableObject
    {
        [SerializeField]
        private List<Menu> menuScreens = new();

        public List<Menu> MenuScreens => menuScreens;
    }
}
