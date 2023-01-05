using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gamepangin
{
    [Serializable]
    public abstract class TRepository<T> : IRepository where T : class, IRepository, new()
    {
        public const string PATH_IN_RESOURCES = "Repositories/";
        public const string DIRECTORY_ASSETS = "Assets/_Gamepangin/Resources/" + PATH_IN_RESOURCES;

        protected static T Instance;

        public string AssetDirectory => DIRECTORY_ASSETS;
        public abstract string RepositoryId { get; }

        public static T Get
        {
            get
            {
                if (Instance != null) return Instance;
                
                T repository = new T();
                string path = PathUtils.Combine(PATH_IN_RESOURCES, repository.RepositoryId);

                AssetRepository<T> assetRepository = Resources.Load<AssetRepository<T>>(path);
                
#if UNITY_EDITOR
                if (assetRepository == null)
                {
                    var newRepo = ScriptableObject.CreateInstance<AssetRepository<T>>();
                    AssetDatabase.CreateAsset(newRepo, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
                
                if (assetRepository != null)
                {
                    repository = assetRepository.Get();
                }
                
                Instance = repository;
                return Instance;
            }
        }
    }
}