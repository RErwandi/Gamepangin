using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gamepangin
{
    [Serializable]
    public class ScriptableObjectWithIdRepository : TRepository<ScriptableObjectWithIdRepository>
    {
        public override string RepositoryId => "ScriptableObject.assets";

        [SerializeField] private ScriptableObjectWithId[] scriptableObjects;

        public ScriptableObjectWithId[] ScriptableObjects
        {
            get => scriptableObjects;
            set => scriptableObjects = value;
        }

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode() => Instance = null;
#endif
    }
}