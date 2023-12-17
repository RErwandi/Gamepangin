#pragma warning disable CS0414

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gamepangin
{
	[CreateAssetMenu(order = 0, fileName = "New Audio Clip", menuName = "Gamepangin/Audio/Clip Data")]
    public class AudioClipData : DataDefinition<AudioClipData>
    {
	    [Title("Sound")]
	    [Tooltip("the sound clip to play")]
	    [HideIf("useRandomSounds")]
	    public AudioClip sound;
	    [Tooltip("Use randomized sound")]
        public bool useRandomSounds;
        [Tooltip("an array to pick a random sfx from")]
        [ShowIf("useRandomSounds")]
        public AudioClip[] randomSounds;
        public AudioClip Sound => !useRandomSounds ? sound : randomSounds[Random.Range(0, randomSounds.Length)];
        [Title("Sound Options")]
        [Tooltip("the min/max volume to play the sound at")]
        [MinMaxSlider(0f, 2f, true)]
        public Vector2 volume = new(1f, 1f);
        public float Volume => Random.Range(volume.x, volume.y);
        [MinMaxSlider(0f, 2f, true)]
        public Vector2 pitch = new(1f, 1f);
        public float Pitch => Random.Range(pitch.x, pitch.y);
        [Tooltip("the track on which to play the sound. Pick the one that matches the nature of your sound")]
		public AudioManagerTracks audioTrack;
		[Tooltip("whether or not this sound should loop")]
		public bool loop;
		[Tooltip("whether or not this sound should continue playing when transitioning to another scene")]
		public bool persistent;
		[Tooltip("whether or not this sound should play if the same sound clip is already playing")]
		public bool doNotPlayIfClipAlreadyPlaying;
		[Tooltip("if this is true, this sound won't be recycled if it's not done playing")]
		public bool doNotAutoRecycleIfNotDonePlaying;
		
		[FoldoutGroup("Fade")]
		[Tooltip("whether or not to fade this sound in when playing it")]
		public bool fade;
		[Tooltip("if fading, the volume at which to start the fade")]
		[FoldoutGroup("Fade")]
		public float fadeInitialVolume;
		[Tooltip("if fading, the duration of the fade, in seconds")]
		[FoldoutGroup("Fade")]
		public float fadeDuration = 1f;
		[Tooltip("if fading, the tween over which to fade the sound ")]
		[FoldoutGroup("Fade")]
		public Ease fadeEase = Ease.InOutQuart;
		
		[FoldoutGroup("Solo")]
		[Tooltip("whether or not this sound should play in solo mode over its destination track. If yes, all other sounds on that track will be muted when this sound starts playing")]
		public bool soloSingleTrack;
		[FoldoutGroup("Solo")]
		[Tooltip("whether or not this sound should play in solo mode over all other tracks. If yes, all other tracks will be muted when this sound starts playing")]
		public bool soloAllTracks;
		[FoldoutGroup("Solo")]
		[Tooltip("if in any of the above solo modes, AutoUnSoloOnEnd will unmute the track(s) automatically once that sound stops playing")]
		public bool autoUnSoloOnEnd;
		
		[FoldoutGroup("Spatial Settings")]
		[Tooltip("Pans a playing sound in a stereo way (left or right). This only applies to sounds that are Mono or Stereo.")]
		[Range(-1f,1f)]
		public float panStereo;
		[FoldoutGroup("Spatial Settings")]
		[Tooltip("Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D.")]
		[Range(0f,1f)]
		public float spatialBlend;
		
		[FoldoutGroup("Effects")]
		[Tooltip("Bypass effects (Applied from filter components or global listener filters).")]
		public bool bypassEffects;
		[FoldoutGroup("Effects")]
		[Tooltip("When set global effects on the AudioListener will not be applied to the audio signal generated by the AudioSource. Does not apply if the AudioSource is playing into a mixer group.")]
		public bool bypassListenerEffects;
		[FoldoutGroup("Effects")]
		[Tooltip("When set doesn't route the signal from an AudioSource into the global reverb associated with reverb zones.")]
		public bool bypassReverbZones;
		[FoldoutGroup("Effects")]
		[Tooltip("Sets the priority of the AudioSource.")]
		[Range(0, 256)]
		public int priority = 128;
		[FoldoutGroup("Effects")]
		[Tooltip("The amount by which the signal from the AudioSource will be mixed into the global reverb associated with the Reverb Zones.")]
		[Range(0f,1.1f)]
		public float reverbZoneMix = 1f;
		
		[FoldoutGroup("3D Sound Settings")]
		[Tooltip("Sets the Doppler scale for this AudioSource.")]
		[Range(0f,5f)]
		public float dopplerLevel = 1f;
		[FoldoutGroup("3D Sound Settings")]
		[Tooltip("Sets the spread angle (in degrees) of a 3d stereo or multichannel sound in speaker space.")]
		[Range(0,360)]
		public int spread;
		[FoldoutGroup("3D Sound Settings")]
		[Tooltip("Sets/Gets how the AudioSource attenuates over distance.")]
		public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
		[FoldoutGroup("3D Sound Settings")]
		[Tooltip("Within the Min distance the AudioSource will cease to grow louder in volume.")]
		public float minDistance = 1f;
		[FoldoutGroup("3D Sound Settings")]
		[Tooltip("(Logarithmic rolloff) MaxDistance is the distance a sound stops attenuating at.")]
		public float maxDistance = 500f;
		
		#if UNITY_EDITOR
	    
	    private GameObject previewAudioGo;
	    private AudioManagerSound previewAudioSound;
	    private bool editorStillPlaying;

	    [SerializeField, ProgressBar(0f, 1f, DrawValueLabel = false), HideLabel] 
	    private float previewProgress;
	    
	    [HideIf(nameof(editorStillPlaying))]
	    [Button(ButtonSizes.Large), HorizontalGroup]
	    private void PreviewAudio()
	    {
		    if(previewAudioGo == null)
			    previewAudioGo = new GameObject("OneShotAudio");
            
		    previewAudioSound = previewAudioGo.AddComponent<AudioManagerSound>();
		    previewAudioSound.audioSource = previewAudioSound.gameObject.AddComponent<AudioSource>();
		    previewAudioSound.audioSource.clip = Sound;
		    previewAudioSound.audioSource.pitch = Pitch;
		    previewAudioSound.audioSource.volume = Volume;

		    previewAudioSound.audioSource.Play();

		    EditorApplication.update += DestroyAudioPreview;
		    Selection.selectionChanged += StopPreview;
	    }

	    [ShowIf(nameof(editorStillPlaying))]
	    [Button(ButtonSizes.Large), HorizontalGroup]
	    private void StopPreview()
	    {
		    previewProgress = 0f;
		    editorStillPlaying = false;
		    DestroyImmediate(previewAudioGo);
		    previewAudioGo = null;
            
		    EditorApplication.update -= DestroyAudioPreview;
		    Selection.selectionChanged -= StopPreview;
	    }

	    private void DestroyAudioPreview()
        {
            if (previewAudioSound != null && !previewAudioSound.audioSource.isPlaying)
            {
                // Destroy the temporary GameObject after the audio clip has finished playing
                DestroyImmediate(previewAudioGo);
                previewAudioGo = null;
                editorStillPlaying = false;
                previewProgress = 0f;

                // Unsubscribe from the update to avoid unnecessary checks
                EditorApplication.update -= DestroyAudioPreview;
            }
            else
            {
                editorStillPlaying = true;
                if (previewAudioSound != null)
                {
                    previewProgress = previewAudioSound.audioSource.time / previewAudioSound.audioSource.clip.length;
                }
            }
        }
#endif
    }
}