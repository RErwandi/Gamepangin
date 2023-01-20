using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    [DisallowMultipleComponent]
    public class MenuManager : Singleton<MenuManager>
    {
        protected override bool IsPersistBetweenScenes => false;
        
        [SerializeField]
        private List<Menu> menuScreens = new();

        public List<Menu> MenuScreens
        {
            get => menuScreens;
            set => menuScreens = value;
        }

        [SerializeField]
        private Menu startMenu;

        [ShowInInspector]
        private Stack<Menu> menuStack = new();

        private void Start()
        {
            if (startMenu != null)
            {
                var startMenuInst = CreateInstance(startMenu.name);
                OpenMenu(startMenuInst.GetMenu());
            }
        }

        public GameObject CreateInstance(string menuName)
        {
            var prefab = GetPrefab(menuName);

            return Instantiate(prefab, transform);
        }

        public void CreateInstance(string menuName, out GameObject menuInstance)
        {
            var prefab = GetPrefab(menuName);

            menuInstance = Instantiate(prefab, transform);
        }

        public void OpenMenu(Menu menuInstance)
        {
            // De-activate top menu
            if (menuStack.Count > 0)
            {
                if (menuInstance.disableMenusUnderneath)
                {
                    foreach (var menu in menuStack)
                    {
                        menu.gameObject.SetActive(false);

                        if (menu.disableMenusUnderneath)
                            break;
                    }
                }

                var topCanvas = menuInstance.GetComponent<Canvas>();
                var previousCanvas = menuStack.Peek().GetComponent<Canvas>();
                topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
            }

            menuStack.Push(menuInstance);
        }

        private GameObject GetPrefab(string prefabName)
        {
            for (int i = 0; i < MenuScreens.Count; i++)
            {
                if (MenuScreens[i].name == prefabName)
                {
                    return MenuScreens[i].gameObject;
                }
            }
            Debug.LogError("Prefab not found for " + prefabName);
            return null;
        }

        public void CloseMenu(Menu menu)
        {
            if (menuStack.Count == 0)
            {
                Debug.LogWarningFormat(menu, "{0} cannot be closed because menu stack is empty", menu.GetType());
                return;
            }

            if (menuStack.Peek() != menu)
            {
                Debug.LogWarningFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
                return;
            }

            CloseTopMenu();
        }

        public void CloseTopMenu()
        {
            var menuInstance = menuStack.Pop();

            if (menuInstance.destroyWhenClosed)
                Destroy(menuInstance.gameObject);
            else
                menuInstance.gameObject.SetActive(false);

            // Re-activate top menu
            // If a re-activated menu is an overlay we need to activate the menu under it
            foreach (var menu in menuStack)
            {
                menu.gameObject.SetActive(true);

                if (menu.disableMenusUnderneath)
                    break;
            }
        }

        private void Update()
        {
            // On Android the back button is sent as Esc
            if (Input.GetKeyDown(KeyCode.Escape) && menuStack.Count > 0)
            {
                menuStack.Peek().OnBackPressed();
            }
        }
    }

    public static class MenuExtensions
    {
        public static Menu GetMenu(this GameObject go)
        {
            return go.GetComponent<Menu>();
        }
    }
}