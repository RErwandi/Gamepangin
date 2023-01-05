using System;
using UnityEngine;

namespace Gamepangin
{
    [Serializable]
    public abstract class TAssetRepository : ScriptableObject
    {
        public abstract string RepositoryId { get; }
        public abstract string AssetPath { get; }
    }
}