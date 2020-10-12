using UnityEngine;

namespace Erwandi.Gamepangin.Patterns
{
	/// <summary>
	/// Singleton pattern implementation which derived from MonoBehaviour.
	/// You can override ShouldNotDestroyOnLoad if you want singleton not persist between scene changes.
	/// </summary>
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;
		private static bool _applicationIsQuitting;
		
		public static T Instance
		{
			get
			{
				if (_applicationIsQuitting)
				{
					Debug.LogError($"{typeof(T)} Singleton can't be used when application is quitting.");
					return null;
				}
				
				if (_instance != null) return _instance;
				
				_instance = (T) FindObjectOfType(typeof(T));

				if (FindObjectsOfType(typeof(T)).Length > 1)
				{
					return _instance;
				}

				if (_instance == null)
				{
					CreateInstance();
				}

				return _instance;
			}
		}
		
		protected virtual void Awake()
		{
			// Move singleton to singleton service parent
			gameObject.transform.SetParent(SingletonService.Parent);
			
			if (_instance == null)
			{
				//If I am the first instance, make me the Singleton
				_instance = this as T;
				if (ShouldNotDestroyOnLoad())
				{
					DontDestroyOnLoad(gameObject);
				}
			}
			else
			{
				//If a Singleton already exists and you find
				//another reference in scene, destroy it!
				if(this != _instance)
				{
					Destroy(gameObject);
				}
			}
		}
		
		/// <summary>
		/// Create the Singleton Instance.
		/// </summary>
		private static void CreateInstance()
		{
			var singleton = new GameObject();
			_instance = singleton.AddComponent<T>();
			singleton.name = $"{typeof(T).FullName}";
		}

		/// <summary>
		/// Override this if you want the singleton does not persist between scene changes.
		/// </summary>
		/// <returns></returns>
		protected virtual bool ShouldNotDestroyOnLoad() { return true; }

		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed,
		/// it will create a buggy ghost object that will stay on the Editor scene
		/// even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		protected virtual void OnDestroy ()
		{
            _instance = null;
		}

		protected void OnApplicationQuit()
		{
			_instance = null;
			_applicationIsQuitting = true;
		}
	}
}