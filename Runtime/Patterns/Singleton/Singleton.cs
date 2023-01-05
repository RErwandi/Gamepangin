using UnityEngine;

namespace Gamepangin
{
    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    if (ApplicationManager.IsExiting) return null;
                    
                    var singleton = new GameObject();
                    instance = singleton.AddComponent<T>();
                    singleton.name = $"(Singleton) {typeof(T)}";

                    var component = instance.GetComponent<Singleton<T>>();
                    component.OnCreate();

                    if (component.IsPersistBetweenScenes) DontDestroyOnLoad(singleton);
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (ApplicationManager.IsExiting) return;

            if (IsPersistBetweenScenes)
            {
                DontDestroyOnLoad(this);
            }

            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // Helper to create a singleton instance i.e to create instance even before Awake called. see ApplicationManager.cs
        protected void WakeUp() { }
        
        // Called after singleton is created
        protected virtual void OnCreate() { }
        
        // Decide if singleton persist between scenes or not
        protected virtual bool IsPersistBetweenScenes => true;

        protected virtual void OnDestroy()
        {
            instance = null;
        }
    }
}