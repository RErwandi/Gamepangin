using UnityEngine;

namespace Gamepangin
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManagerSound : MonoBehaviour
    {
        public string id;
        public AudioManagerTracks track;
        public bool persistent;
        public AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
}