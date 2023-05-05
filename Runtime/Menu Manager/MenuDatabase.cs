using System.Collections.Generic;
using UnityEngine;

namespace Gamepangin 
{
    [CreateAssetMenu(menuName = "Gamepangin/Menu Database", order = 0, fileName = "Menu Database.asset")]
    public class MenuDatabase : ScriptableObject
    {
        [SerializeField]
        private List<Menu> menuScreens = new();

        public List<Menu> MenuScreens => menuScreens;
    }
}
