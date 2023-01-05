using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Gamepangin
{
    public class AudioManager : Singleton<AudioManager>
    {
        public GameObject audioSourcePrefab;
        public AudioMixer audioMixer;
        public AudioMixerGroup masterAudioMixerGroup;
        public AudioMixerGroup musicAudioMixerGroup;
        public AudioMixerGroup sfxAudioMixerGroup;
        public AudioMixerGroup uiAudioMixerGroup;
        public float mixerValuesMultiplier = 20;

        public AudioManagerState state;
        
        private GameObjectPool pool;
        private int defaultPoolSize = 10;
        
        private List<AudioManagerSound> sounds = new();

        protected override void Awake()
        {
            base.Awake();
            InitializeAudioManager();
        }

        private void InitializeAudioManager()
        {
            if (pool == null)
            {
                pool = gameObject.AddComponent<GameObjectPool>();
            }

            pool.Prefab = audioSourcePrefab;
            pool.Preload = defaultPoolSize;
            pool.PreloadAll();
        }

        public virtual void PlaySound(string id)
        {
            var clipData = GetClipFromDatabase(id);
            PlaySound(clipData);
        }

        private AudioClipData GetClipFromDatabase(string id)
        {
            var allClips = Database.Get<AudioRepository>().Clips;
            foreach (var clip in allClips)
            {
                if (clip.id.ToString().Equals(id))
                {
                    return clip;
                }
            }

            return null;
        }

        [Button]
        public virtual void PlaySound(AudioClipData clipData, Vector3 location = default)
        {
            var audioSourceGo = pool.Spawn(transform);
            var audioSound = audioSourceGo.GetComponent<AudioManagerSound>();
            var clip = clipData;
            audioSound.id = clipData.id.ToString();

            bool alreadyIn = false;
            foreach (var audioManagerSound in sounds)
            {
                if (audioManagerSound.audioSource == audioSound.audioSource)
                {
                    alreadyIn = true;
                }
            }

            if (!alreadyIn)
            {
                sounds.Add(audioSound);
            }
            
            var clipSound = clip.sound;
            audioSound.persistent = clip.persistent;
            audioSound.track = clip.audioTrack;
            audioSound.audioSource.transform.position = location;
            audioSound.audioSource.clip = clipSound;
            audioSound.audioSource.pitch = clip.Pitch;
            audioSound.audioSource.volume = clip.Volume;
            audioSound.audioSource.spatialBlend = clip.spatialBlend;
            audioSound.audioSource.panStereo = clip.panStereo;
            audioSound.audioSource.loop = clip.loop;
            audioSound.audioSource.bypassEffects = clip.bypassEffects;
            audioSound.audioSource.bypassListenerEffects = clip.bypassListenerEffects;
            audioSound.audioSource.bypassReverbZones = clip.bypassReverbZones;
            audioSound.audioSource.priority = clip.priority;
            audioSound.audioSource.reverbZoneMix = clip.reverbZoneMix;
            audioSound.audioSource.dopplerLevel = clip.dopplerLevel;
            audioSound.audioSource.spread = clip.spread;
            audioSound.audioSource.rolloffMode = clip.rolloffMode;
            audioSound.audioSource.minDistance = clip.minDistance;
            audioSound.audioSource.maxDistance = clip.maxDistance;

            audioSound.audioSource.outputAudioMixerGroup = clip.audioTrack switch
            {
                AudioManagerTracks.Master => masterAudioMixerGroup,
                AudioManagerTracks.Music => musicAudioMixerGroup,
                AudioManagerTracks.Sfx => sfxAudioMixerGroup,
                AudioManagerTracks.UI => uiAudioMixerGroup,
                _ => audioSound.audioSource.outputAudioMixerGroup
            };

            audioSound.audioSource.Play();

            if (!clip.loop && !clip.doNotAutoRecycleIfNotDonePlaying)
            {
                pool.Despawn(audioSourceGo, clipSound.length);
            }
        }

        public virtual void PauseSound(string id)
        {
            var audioSource = FindById(id);
            if(audioSource)
                audioSource.Pause();
        }

        public virtual void ResumeSound(string id)
        {
            var audioSource = FindById(id);
            if(audioSource)
                audioSource.Play();
        }

        public virtual void StopSound(string id)
        {
            var audioSource = FindById(id);
            if(audioSource)
                audioSource.Stop();
        }

        public virtual void MuteTrack(AudioManagerTracks track)
        {
            ControlTrack(track, ControlTrackModes.Mute);
        }
        
        public virtual void UnmuteTrack(AudioManagerTracks track)
        {
            ControlTrack(track, ControlTrackModes.Unmute);
        }

        public virtual void PauseTrack(AudioManagerTracks track)
        {
            foreach (var sound in sounds.Where(sound => sound.track == track))
            {
                sound.audioSource.Pause();
            }
        }

        public virtual void PlayTrack(AudioManagerTracks track)
        {
            foreach (var sound in sounds.Where(sound => sound.track == track))
            {
                sound.audioSource.Play();
            }
        }
        
        public virtual void StopTrack(AudioManagerTracks track)
        {
            foreach (var sound in sounds.Where(sound => sound.track == track))
            {
                sound.audioSource.Stop();
            }
        }
        
        public virtual void FreeTrack(AudioManagerTracks track)
        {
            foreach (var sound in sounds)
            {
                if (sound.track == track)
                {
                    sound.audioSource.Stop();
                    Pool.Despawn(sound.gameObject);
                }
            }
        }
        
        public virtual bool HasSoundsPlaying(AudioManagerTracks track)
        {
            return sounds.Any(sound => sound.track == track && sound.audioSource.isPlaying);
        }
        
        public virtual bool IsMuted(AudioManagerTracks track)
        {
            switch (track)
            {
                case AudioManagerTracks.Master:
                    return state.masterOn; 
                case AudioManagerTracks.Music:
                    return state.musicOn;
                case AudioManagerTracks.Sfx:
                    return state.sfxOn;
                case AudioManagerTracks.UI:
                    return state.uiOn;
            }
            return false;
        }

        public virtual void SetTrackVolume(AudioManagerTracks track, float volume)
        {
            if (volume <= 0f)
            {
                volume = AudioManagerSettings.MIN_VOLUME;
            }
            
            switch (track)
            {
                case AudioManagerTracks.Master:
                    state.masterVolume = volume;
                    audioMixer.SetFloat(AudioManagerSettings.MASTER_VOLUME_PARAMS, NormalizedToMixerVolume(volume));
                    break;
                case AudioManagerTracks.Music:
                    state.musicVolume = volume;
                    audioMixer.SetFloat(AudioManagerSettings.MUSIC_VOLUME_PARAMS, NormalizedToMixerVolume(volume));
                    break;
                case AudioManagerTracks.UI:
                    state.uiVolume = volume;
                    audioMixer.SetFloat(AudioManagerSettings.UI_VOLUME_PARAMS, NormalizedToMixerVolume(volume));
                    break;
                case AudioManagerTracks.Sfx:
                    state.sfxVolume = volume;
                    audioMixer.SetFloat(AudioManagerSettings.SFX_VOLUME_PARAMS, NormalizedToMixerVolume(volume));
                    break;
            }
        }

        protected float GetTrackVolume(AudioManagerTracks track)
        {
            float volume = 1f;
            switch (track)
            {
                case AudioManagerTracks.Master:
                    audioMixer.GetFloat(AudioManagerSettings.MASTER_VOLUME_PARAMS, out volume);
                    break;
                case AudioManagerTracks.Music:
                    audioMixer.GetFloat(AudioManagerSettings.MUSIC_VOLUME_PARAMS, out volume);
                    break;
                case AudioManagerTracks.Sfx:
                    audioMixer.GetFloat(AudioManagerSettings.SFX_VOLUME_PARAMS, out volume);
                    break;
                case AudioManagerTracks.UI:
                    audioMixer.GetFloat(AudioManagerSettings.UI_VOLUME_PARAMS, out volume);
                    break;
            }

            return MixerVolumeToNormalized(volume);
        }
        
        public enum ControlTrackModes {Mute, Unmute, SetVolume}
        protected void ControlTrack(AudioManagerTracks track, ControlTrackModes mode, float volume = 1f)
        {
            string target;
            var savedVolume = 0f;

            switch (track)
            {
                case AudioManagerTracks.Master:
                    target = AudioManagerSettings.MASTER_VOLUME_PARAMS;
                    if (mode == ControlTrackModes.Mute)
                    {
                        audioMixer.GetFloat(target, out state.masterSavedVolume);
                        state.masterOn = false;
                    }
                    else if (mode == ControlTrackModes.Unmute)
                    {
                        savedVolume = state.masterSavedVolume;
                        state.masterOn = true;
                    }
                    break;
                case AudioManagerTracks.Music:
                    target = AudioManagerSettings.MUSIC_VOLUME_PARAMS;
                    if (mode == ControlTrackModes.Mute)
                    {
                        audioMixer.GetFloat(target, out state.musicSavedVolume);
                        state.musicOn = false;
                    }
                    else if (mode == ControlTrackModes.Unmute)
                    {
                        savedVolume = state.musicSavedVolume;
                        state.musicOn = true;
                    }
                    break;
                case AudioManagerTracks.Sfx:
                    target = AudioManagerSettings.SFX_VOLUME_PARAMS;
                    if (mode == ControlTrackModes.Mute)
                    {
                        audioMixer.GetFloat(target, out state.sfxSavedVolume);
                        state.sfxOn = false;
                    }
                    else if (mode == ControlTrackModes.Unmute)
                    {
                        savedVolume = state.sfxSavedVolume;
                        state.sfxOn = true;
                    }
                    break;
                case AudioManagerTracks.UI:
                    target = AudioManagerSettings.UI_VOLUME_PARAMS;
                    if (mode == ControlTrackModes.Mute)
                    {
                        audioMixer.GetFloat(target, out state.uiSavedVolume);
                        state.uiOn = false;
                    }
                    else if (mode == ControlTrackModes.Unmute)
                    {
                        savedVolume = state.uiSavedVolume;
                        state.uiOn = true;
                    }
                    break;
            }

            switch (mode)
            {
                case ControlTrackModes.Mute:
                    SetTrackVolume(track, AudioManagerSettings.MIN_VOLUME);
                    break;
                case ControlTrackModes.Unmute:
                    SetTrackVolume(track, MixerVolumeToNormalized(savedVolume));
                    break;
                case ControlTrackModes.SetVolume:
                    SetTrackVolume(track, volume);
                    break;
            }
        }
        
        public virtual void PauseAllSounds()
        {
            foreach (var sound in sounds)
            {
                sound.audioSource.Pause();
            }    
        }
        
        public virtual void PlayAllSounds()
        {
            foreach (var sound in sounds)
            {
                sound.audioSource.Play();
            }    
        }
        
        public virtual void StopAllSounds()
        {
            foreach (var sound in sounds)
            {
                sound.audioSource.Stop();
            }
        }
        
        public virtual void FreeAllSounds()
        {
            foreach (var sound in sounds)
            {
                pool.Despawn(sound.gameObject);
            }
        }
        
        public virtual void FreeAllSoundsButPersistent()
        {
            foreach (var sound in sounds.Where(sound => !sound.persistent))
            {
                pool.Despawn(sound.gameObject);
            }
        }
        
        public virtual void FreeAllLoopingSounds()
        {
            foreach (var sound in sounds)
            {
                if (sound.audioSource.loop)
                {
                    pool.Despawn(sound.gameObject);
                }
            }
        }

        protected float NormalizedToMixerVolume(float normalizedVolume)
        {
            return Mathf.Log10(normalizedVolume) * mixerValuesMultiplier;
        }

        protected float MixerVolumeToNormalized(float mixerVolume)
        {
            return (float)Math.Pow(10, (mixerVolume / mixerValuesMultiplier));
        }

        protected AudioSource FindById(string id)
        {
            foreach (var sound in sounds.Where(sound => sound.id == id))
            {
                return sound.audioSource;
            }
            
            Debug.LogWarning($"Audio clip with id [{id}] doesnt found in Audio Database.");
            return null;
        }
    }
}