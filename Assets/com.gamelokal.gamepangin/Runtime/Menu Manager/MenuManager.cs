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

        private List<Menu> menuScreens = new();

        [ShowInInspector, ReadOnly]
        private List<Menu> menuStack = new();
        private Menu LastMenu => menuStack[^1];

        private void Start()
        {
            menuScreens = GetComponentsInChildren<Menu>().ToList();
            foreach (var menu in menuScreens)
            {
                if(menu.gameObject.activeInHierarchy)
                    menuStack.Add(menu);
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
                var topCanvas = menuInstance.Canvas;
                var previousCanvas = LastMenu.Canvas;
                topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
            }

            if (menuInstance.CloseOtherMenuWhenOpen)
            {
                foreach (var menu in menuStack)
                {
                    menu.gameObject.SetActive(false);
                }
            }

            menuInstance.gameObject.SetActive(true);
            if (menuStack.Contains(menuInstance))
            {
                menuStack.Remove(menuInstance);
                menuStack.Add(menuInstance);
            }
            else
            {
                menuStack.Add(menuInstance);
            }
        }

        private GameObject GetPrefab(string prefabName)
        {
            for (int i = 0; i < menuScreens.Count; i++)
            {
                if (menuScreens[i].name == prefabName)
                {
                    return menuScreens[i].gameObject;
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

            if (LastMenu != menu)
            {
                Debug.LogWarningFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
                return;
            }

            if (menu.CloseOtherMenuWhenOpen)
            {
                if (menuStack.Count > 1)
                {
                    menuStack[^2].gameObject.SetActive(true);
                    menuStack[^2].ReOpen();
                }
            }
            
            CloseTopMenu();
        }

        public void CloseTopMenu()
        {
            if (LastMenu.DestroyOnClosed)
                Destroy(LastMenu.gameObject);
            else
                LastMenu.gameObject.SetActive(false);

            menuStack.RemoveAt(menuStack.Count - 1);
        }

        private void Update()
        {
            // On Android the back button is sent as Esc
            if (Input.GetKeyDown(KeyCode.Escape) && menuStack.Count > 0)
            {
                LastMenu.OnBackPressed();
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