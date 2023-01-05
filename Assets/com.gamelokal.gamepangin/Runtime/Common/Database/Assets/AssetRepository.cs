using System;
using UnityEngine;

namespace Gamepangin
{
    [Serializable]
    public abstract class AssetRepository<T> : TAssetRepository where T : class, IRepository, new()
    {
        [SerializeReference] private IRepository repository = new T();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string AssetPath => repository.AssetDirectory;

        public override string RepositoryId => repository.RepositoryId;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T Get()
        {
            return repository as T;
        }
    }
}