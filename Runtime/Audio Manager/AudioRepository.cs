using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gamepangin
{
    [Serializable]
    public class AudioRepository : TRepository<AudioRepository>
    {
        public override string RepositoryId => "Audio.assets";

        [SerializeField] private AudioClipData[] clips;

        public AudioClipData[] Clips
        {
            get => clips;
            set => clips = value;
        }

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode() => Instance = null;
#endif
    }
}