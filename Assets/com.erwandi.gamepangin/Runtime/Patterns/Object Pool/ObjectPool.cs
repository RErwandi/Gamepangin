using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Erwandi.Gamepangin.Patterns
{
    /// <summary>
    /// Generic object pool implementation.
    /// See examples.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : MonoBehaviour where T : Component
    {
        public T prefab;
        public bool autoExpand = true;
        [HideIf("autoExpand")]
        public int maxInstances = 10;
        
        private List<T> pools = new List<T>();
        
        private void Awake()
        {
            if (!autoExpand)
            {
                InitPool();
            }
        }
        
        protected virtual T CreateInstance()
        {
            var instance = Instantiate(prefab, transform);
            pools.Add(instance);
            return instance;
        }

        protected virtual void OnBeforeRent(T instance)
        {
            if (!instance) return;
            
            instance.gameObject.SetActive(true);
        }

        protected virtual void OnBeforeReturn(T instance)
        {
            if (!instance) return;
            
            instance.gameObject.SetActive(false);
        }

        protected virtual void OnClear(T instance)
        {
            if (instance == null) return;
            
            Destroy(instance);
        }

        private void InitPool()
        {
            for (int i = 0; i < maxInstances; i++)
            {
                var instance = CreateInstance();
                OnBeforeRent(instance);
                Return(instance);
            }
        }

        public T Rent()
        {
            var instance = pools.FirstOrDefault(pool => !pool.gameObject.activeInHierarchy);

            if (instance == null)
            {
                if (autoExpand)
                {
                    instance = CreateInstance();
                }
                else
                {
                    instance = pools[0];
                    pools.Remove(instance);
                    pools.Add(instance);
                }
            }
            
            OnBeforeRent(instance);
            return instance;
        }

        public void Return(T instance)
        {
            if (instance == null) return;
            
            OnBeforeReturn(instance);
        }
        
        public void Clear(bool callOnBeforeRent = false)
        {
            if (pools == null) return;
            foreach (var pool in pools)
            {
                if (callOnBeforeRent)
                {
                    OnBeforeRent(pool);
                }

                OnClear(pool);
            }
        }
    }
}