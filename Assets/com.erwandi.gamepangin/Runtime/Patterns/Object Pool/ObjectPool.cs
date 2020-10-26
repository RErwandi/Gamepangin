using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Erwandi.Gamepangin.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Erwandi.Gamepangin.Patterns
{
    /// <summary>
    /// Object pool containing several copies of a template object (usually a prefab). Using the pool with GetObject and Release 
    /// provides a high speed alternative to repeatedly calling Instantiate and Destroy.
    /// </summary>
    [CreateAssetMenu(fileName = "Untitled Pool", menuName = "Master Object Pooler 2/Object Pool", order = 0)]
    public class ObjectPool : ScriptableObject
    {
        [Tooltip("The name of the pool. Used for identification and as the key when using a MasterObjectPooler.")]
        [SerializeField] private string poolId = string.Empty;

        [Tooltip("The template object to center the pool on. All objects in the pool will be a copy of this object.")]
        [SerializeField] private GameObject prefab;

        [Tooltip("The default number of objects to create in this pool when initializing it.")]
        [SerializeField] private int defaultSize;

        [Tooltip("The maximum number of objects that can be kept in this pool. If it is exceeded, objects will be destroyed instead of pooled when returned. Set to -1 for no limit.")]
        [SerializeField] private int maxSize = -1;

        [Tooltip("If enabled, object instances will be renamed to ObjectName#XXX where XXX is the instance number. This is useful if you want them all to be uniquely named.")]
        [SerializeField] private bool incrementalInstanceNames;

        [Tooltip("Auto initializes the pool. In the editor this occurs when play-mode is entered. In builds, this occurs on startup")]
        [SerializeField] private bool autoInitialize = default;

        [Tooltip("Repopulate the pool with objects when the scene changes to replace objects that were unloaded/destroyed.")]
        [SerializeField] private bool repopulateOnSceneChange = default;

        /// <summary>
        /// If enabled, object instances will be renamed to ObjectName#XXX where XXX is the instance number. This is useful if you want them all to be uniquely named.
        /// </summary>
        public bool IncrementalInstanceNames
        {
            get => incrementalInstanceNames;
            set => incrementalInstanceNames = value;
        }

        /// <summary>
        /// The name of the pool. Used for identification and as the key when using a MasterObjectPooler.
        /// </summary>
        public string PoolId => poolId;

        /// <summary>
        /// Parent transform for all pooled objects.
        /// </summary>
        public Transform ObjectParent
        {
            get
            {
                if (!objectParent)
                {
                    objectParent = new GameObject(string.Format("{0} Pool", poolId)).transform;
                }

                return objectParent;
            }
        }
        private Transform objectParent;

        private bool HasMaxSize => maxSize > 0;
        private bool HasPooledObjects => pooledObjects.Count > 0;
        public bool Initialized { get; private set; }

        private int instanceCounter;
        private readonly Regex poolRegex = new Regex("[_ ]*[Pp]ool");

        #region Caches
        private readonly List<GameObject> pooledObjects = new List<GameObject>();
        private Dictionary<int, GameObject> aliveObjects = new Dictionary<int, GameObject>();

        private readonly List<GameObject> releaseAllBuffer = new List<GameObject>();
        private readonly Dictionary<Tuple2<int, Type>, object> componentCache = new Dictionary<Tuple2<int, Type>, object>();
        #endregion

        #region Initialization/Creation
        private ObjectPool()
        {
            prefab = null;
        }

        /// <summary>
        /// Creates an ObjectPool.
        /// </summary>
        /// <param name="template">The template object to center the pool on. All objects in the pool will be a copy of this object.</param>
        /// <param name="defaultSize">The default number of objects to create in this pool when initializing it.</param>
        /// <param name="maxSize">The maximum number of objects that can be kept in this pool. If it is exceeded, objects will be destroyed instead of pooled when returned. Set to -1 for no limit.</param>
        /// <returns>The created ObjectPool.</returns>
        public static ObjectPool Create(GameObject template, int defaultSize = 0, int maxSize = -1)
        {
            return Create(template, template.name, defaultSize, maxSize);
        }

        /// <summary>
        /// Creates an ObjectPool.
        /// </summary>
        /// <param name="template">The template object to center the pool on. All objects in the pool will be a copy of this object.</param>
        /// <param name="name">The name of the pool. Used for identification and as the key when using a MasterObjectPooler.</param>
        /// <param name="defaultSize">The default number of objects to create in this pool when initializing it.</param>
        /// <param name="maxSize">The maximum number of objects that can be kept in this pool. If it is exceeded, objects will be destroyed instead of pooled when returned. Set to -1 for no limit.</param>
        /// <returns>The created ObjectPool.</returns>
        public static ObjectPool Create(GameObject template, string name, int defaultSize = 0, int maxSize = -1)
        {
            ObjectPool pool = CreateInstance<ObjectPool>();
            pool.poolId = name;
            pool.prefab = template;
            pool.defaultSize = defaultSize;
            pool.maxSize = maxSize;

            return pool;
        }

        /// <summary>
        /// Creates an ObjectPool and initializes it.
        /// </summary>
        /// <param name="template">The template object to center the pool on. All objects in the pool will be a copy of this object.</param>
        /// <param name="defaultSize">The default number of objects to create in this pool when initializing it.</param>
        /// <param name="maxSize">The maximum number of objects that can be kept in this pool. If it is exceeded, objects will be destroyed instead of pooled when returned. Set to -1 for no limit.</param>
        /// <returns>The created ObjectPool.</returns>
        public static ObjectPool CreateAndInitialize(GameObject template, int defaultSize = 0, int maxSize = -1)
        {
            ObjectPool pool = Create(template, defaultSize, maxSize);
            pool.Initialize();

            return pool;
        }

        /// <summary>
        /// Creates an ObjectPool and initializes it.
        /// </summary>
        /// <param name="template">The template object to center the pool on. All objects in the pool will be a copy of this object.</param>
        /// <param name="name">The name of the pool. Used for identification and as the key when using a MasterObjectPooler.</param>
        /// <param name="defaultSize">The default number of objects to create in this pool when initializing it.</param>
        /// <param name="maxSize">The maximum number of objects that can be kept in this pool. If it is exceeded, objects will be destroyed instead of pooled when returned. Set to -1 for no limit.</param>
        /// <returns>The created ObjectPool.</returns>
        public static ObjectPool CreateAndInitialize(GameObject template, string name, int defaultSize = 0, int maxSize = -1)
        {
            ObjectPool pool = Create(template, name, defaultSize, maxSize);
            pool.Initialize();

            return pool;
        }

        private void OnEnable()
        {
            instanceCounter = 0;
            SceneManager.sceneUnloaded += OnSceneUnload;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChange;
#endif
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnload;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
#endif
        }

        /// <summary>
        /// Initializes the ObjectPool.
        /// </summary>
        public void Initialize(bool forceReinitialization = false)
        {
            if (!Initialized || forceReinitialization)
            {
                Initialized = true;

                AutoFillName();
                InitializeIPoolables();
                Populate(defaultSize);
            }
        }

        internal void AutoFillName()
        {
            if (string.IsNullOrWhiteSpace(poolId))
            {
                poolId = poolRegex.Replace(name, string.Empty);
                ObjectParent.name = poolId;
            }
            else if (string.IsNullOrWhiteSpace(name))
            {
                name = poolId;
            }
        }

        private void InitializeIPoolables()
        {
            foreach (IPoolable poolable in prefab.GetComponentsInChildren<IPoolable>())
            {
                poolable.InitializeTemplate(this);
            }
        }
        #endregion

        #region Internal
        private GameObject CreateNewObject() { return CreateNewObject(prefab.transform.position, prefab.transform.rotation); }
        private GameObject CreateNewObject(Vector3 position, Quaternion rotation)
        {
            GameObject newObj = Instantiate(prefab, position, rotation);
            newObj.transform.SetParent(ObjectParent, false);

            if (incrementalInstanceNames)
            {
                newObj.name = string.Format("{0}#{1:000}", prefab.name, instanceCounter);
            }
            else
            {
                newObj.name = prefab.name;
            }

            instanceCounter++;
            return newObj;
        }

        private void CleanseInternal()
        {
            if (!objectParent)
            {
                pooledObjects.Clear();
                aliveObjects.Clear();
                componentCache.Clear();
            }
            else
            {
                pooledObjects.RemoveAll(x => !x);
            }
        }
        #endregion

        #region GetObject/Component
        /// <summary>
        /// Gets an object from the pool.
        /// </summary>
        /// <returns>The retrieved object.</returns>
        public GameObject GetObject() { return GetObject(prefab.transform.position); }

        /// <summary>
        /// Gets an object from the pool.
        /// </summary>
        /// <param name="position">The position to set the object to.</param>
        /// <returns>The retrieved object.</returns>
        public GameObject GetObject(Vector3 position) { return GetObject(position, prefab.transform.rotation); }

        /// <summary>
        /// Gets an object from the pool.
        /// </summary>
        /// <param name="position">The position to set the object to.</param>
        /// <param name="rotation">The rotation to set the object to.</param>
        /// <returns>The retrieved object.</returns>
        public GameObject GetObject(Vector3 position, Quaternion rotation)
        {
            GameObject obj;
            if (HasPooledObjects)
            {
                obj = pooledObjects[pooledObjects.Count - 1];
                pooledObjects.RemoveAt(pooledObjects.Count - 1);

                if (!obj)
                {
                    Debug.LogWarning(string.Format("Object in pool '{0}' was null or destroyed; it may have been destroyed externally. Attempting to retrieve a new object", poolId));
                    return GetObject(position, rotation);
                }

                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            else
            {
                obj = CreateNewObject(position, rotation);
            }

            obj.SetActive(true);

            aliveObjects.Add(obj.GetInstanceID(), obj);
            return obj;
        }

        /// <summary>
        /// Gets an object from the pool, and then retrieves the specified component using a cache to improve performance.
        /// </summary>
        /// <typeparam name="T">The component type to get.</typeparam>
        /// <returns>The retrieved component.</returns>
        public T GetObjectComponent<T>() where T : class
        {
            return GetObjectComponent<T>(prefab.transform.position);
        }

        /// <summary>
        /// Gets an object from the pool, and then retrieves the specified component using a cache to improve performance.
        /// </summary>
        /// <typeparam name="T">The component type to get.</typeparam>
        /// <param name="position">The position to set the object to.</param>
        /// <returns>The retrieved component.</returns>
        public T GetObjectComponent<T>(Vector3 position) where T : class
        {
            return GetObjectComponent<T>(position, prefab.transform.rotation);
        }

        /// <summary>
        /// Gets an object from the pool, and then retrieves the specified component using a cache to improve performance.
        /// </summary>
        /// <typeparam name="T">The component type to get.</typeparam>
        /// <param name="position">The position to set the object to.</param>
        /// <param name="rotation">The rotation to set the object to.</param>
        /// <returns>The retrieved component.</returns>
        public T GetObjectComponent<T>(Vector3 position, Quaternion rotation) where T : class
        {
            GameObject obj = GetObject(position, rotation);
            return GetObjectComponent<T>(obj);
        }

        /// <summary>
        /// Retrieves the specified component from an object using a cache to improve performance.
        /// </summary>
        /// <typeparam name="T">The component type to get.</typeparam>
        /// <param name="obj">The object to get the component from.</param>
        /// <returns>The retrieved component.</returns>
        public T GetObjectComponent<T>(GameObject obj) where T : class
        {
            Tuple2<int, Type> key = new Tuple2<int, Type>(obj.GetInstanceID(), typeof(T));
            T component;

            if (componentCache.ContainsKey(key))
            {
                component = componentCache[key] as T;
                if (component == null) { componentCache.Remove(key); }
                else { return component; }
            }

            component = obj.GetComponent<T>();
            if (component != null) { componentCache[key] = component; }
            return component;
        }
        #endregion

        #region Release/Destroy
        /// <summary>
        /// Releases an object and returns it back to the pool, effectively 'destroying' it from the scene.
        /// Pool equivalent of Destroy.
        /// </summary>
        /// <param name="obj">The object to release.</param>
        public void Release(GameObject obj)
        {
            if (!aliveObjects.Remove(obj.GetInstanceID()))
            {
                Debug.LogWarning(string.Format("Object '{0}' could not be found in pool '{1}'; it may have already been released.", obj, poolId));
                return;
            }

            if (obj)
            {
                if (HasMaxSize && pooledObjects.Count >= maxSize)
                {
                    Object.Destroy(obj);
                }
                else
                {
                    pooledObjects.Add(obj);
                    obj.SetActive(false);
                    obj.transform.SetParent(ObjectParent, false);
                }
            }
        }

        /// <summary>
        /// Releases a collection of objects and returns them back to the pool, effectively 'destroying' them from the scene.
        /// </summary>
        /// <param name="objs">the objects to release.</param>
        public void Release(IEnumerable<GameObject> objs)
        {
            foreach (GameObject obj in objs)
            {
                Release(obj);
            }
        }

        /// <summary>
        /// Releases every active object in this pool.
        /// </summary>
        public void ReleaseAll()
        {
            releaseAllBuffer.Clear();
            releaseAllBuffer.AddRange(aliveObjects.Values);
            Release(releaseAllBuffer);
        }

        /// <summary>
        /// Forcibly destroys the object and does not return it to the pool.
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        public void Destroy(GameObject obj)
        {
            aliveObjects.Remove(obj.GetInstanceID());
            Object.Destroy(obj);
        }

        /// <summary>
        /// Forcibly destroys a collection of objects and does not return them to the pool.
        /// </summary>
        /// <param name="objs">The objects to destroy.</param>
        public void Destroy(IEnumerable<GameObject> objs)
        {
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Populates the pool with the specified number of objects, so that they do not need instantiating later.
        /// </summary>
        /// <param name="quantity">The number of objects to populate it with.</param>
        /// <param name="method">The population mode.</param>
        public void Populate(int quantity, PopulateMethod method = PopulateMethod.Set)
        {
            int newObjCount;
            switch (method)
            {
                case PopulateMethod.Set: newObjCount = quantity - pooledObjects.Count; break;
                case PopulateMethod.Add: newObjCount = quantity; break;
                default: newObjCount = 0; break;
            }

            if (HasMaxSize) { newObjCount = Mathf.Min(newObjCount, maxSize - pooledObjects.Count); }
            if (newObjCount < 0) { newObjCount = 0; }

            for (int i = 0; i < newObjCount; i++)
            {
                GameObject newObj = CreateNewObject();
                newObj.SetActive(false);
                pooledObjects.Add(newObj);
            }
        }

        /// <summary>
        /// Destroys every object in the pool, both alive and pooled.
        /// </summary>
        public void Purge()
        {
            foreach (GameObject obj in pooledObjects) { Object.Destroy(obj); }
            foreach (GameObject obj in aliveObjects.Values) { Object.Destroy(obj); }
            pooledObjects.Clear();
            aliveObjects.Clear();
            componentCache.Clear();
        }

        /// <summary>
        /// Gets all active objects in the pool.
        /// </summary>
        /// <returns>The active objects.</returns>
        public IEnumerable<GameObject> GetAllActiveObjects()
        {
            return aliveObjects.Values
                .Where(x => x);
        }
        #endregion

        #region Callbacks
        private void Awake()
        {
            Initialized = false;

#if !UNITY_EDITOR
            if (_autoInitialize)
            {
                Initialize();
            }
#endif
        }

        private void OnSceneUnload(Scene scene)
        {
            CleanseInternal();

            if (repopulateOnSceneChange)
            {
                Populate(defaultSize);
            }
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChange(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.EnteredPlayMode)
            {
                instanceCounter = 0;
                Initialized = false;
                CleanseInternal();

                if (autoInitialize)
                {
                    Initialize();
                }
            }
        }
#endif
#endregion
    }
    
    /// <summary>
    /// Determines how many objects are needed when populating a pool.
    /// </summary>
    public enum PopulateMethod
    {
        /// <summary>If set is used, then populate will ensure the final population is the specified count.</summary>
        Set = 0,

        /// <summary>If add is used, then populate will add the specified count to the current population.</summary>
        Add = 1
    }
}
