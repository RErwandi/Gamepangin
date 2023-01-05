using System;
using System.Collections;
using System.Collections.Generic;
using Gamepangin;
using UnityEngine;
using UnityEngine.UI;

public class AudioManagerDemo : MonoBehaviour
{
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public Slider uiSlider;
    public Toggle masterToggle;
    public Toggle sfxToggle;
    public Toggle musicToggle;
    public Toggle uiToggle;

    private void OnEnable()
    {
        masterSlider.onValueChanged.AddListener(ChangeMasterTrackVolume);
        musicSlider.onValueChanged.AddListener(ChangeMusicTrackVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxTrackVolume);
        uiSlider.onValueChanged.AddListener(ChangeUiTrackVolume);
        
        masterToggle.onValueChanged.AddListener(ToggleMaster);
        sfxToggle.onValueChanged.AddListener(ToggleSfx);
        musicToggle.onValueChanged.AddListener(ToggleMusic);
        uiToggle.onValueChanged.AddListener(ToggleUi);
    }

    private void OnDisable()
    {
        masterSlider.onValueChanged.RemoveListener(ChangeMasterTrackVolume);
        musicSlider.onValueChanged.RemoveListener(ChangeMusicTrackVolume);
        sfxSlider.onValueChanged.RemoveListener(ChangeSfxTrackVolume);
        uiSlider.onValueChanged.RemoveListener(ChangeUiTrackVolume);
        
        masterToggle.onValueChanged.RemoveListener(ToggleMaster);
        musicToggle.onValueChanged.RemoveListener(ToggleMusic);
        sfxToggle.onValueChanged.RemoveListener(ToggleSfx);
        uiToggle.onValueChanged.RemoveListener(ToggleUi);
    }

    private void ChangeMasterTrackVolume(float value)
    {
        AudioManager.Instance.SetTrackVolume(AudioManagerTracks.Master, value);
    }
    
    private void ChangeMusicTrackVolume(float value)
    {
        AudioManager.Instance.SetTrackVolume(AudioManagerTracks.Music, value);
    }
    
    private void ChangeSfxTrackVolume(float value)
    {
        AudioManager.Instance.SetTrackVolume(AudioManagerTracks.Sfx, value);
    }
    
    private void ChangeUiTrackVolume(float value)
    {
        AudioManager.Instance.SetTrackVolume(AudioManagerTracks.UI, value);
    }

    private void ToggleMaster(bool isOn)
    {
        if (!isOn)
        {
            AudioManager.Instance.MuteTrack(AudioManagerTracks.Master);
        }
        else
        {
            AudioManager.Instance.UnmuteTrack(AudioManagerTracks.Master);
        }

        masterSlider.value = AudioManager.Instance.state.masterVolume;
    }
    
    private void ToggleMusic(bool isOn)
    {
        if (!isOn)
        {
            AudioManager.Instance.MuteTrack(AudioManagerTracks.Music);
        }
        else
        {
            AudioManager.Instance.UnmuteTrack(AudioManagerTracks.Music);
        }
        
        musicSlider.value = AudioManager.Instance.state.musicVolume;
    }
    
    private void ToggleSfx(bool isOn)
    {
        if (!isOn)
        {
            AudioManager.Instance.MuteTrack(AudioManagerTracks.Sfx);
        }
        else
        {
            AudioManager.Instance.UnmuteTrack(AudioManagerTracks.Sfx);
        }
        
        sfxSlider.value = AudioManager.Instance.state.sfxVolume;
    }
    
    private void ToggleUi(bool isOn)
    {
        if (!isOn)
        {
            AudioManager.Instance.MuteTrack(AudioManagerTracks.UI);
        }
        else
        {
            AudioManager.Instance.UnmuteTrack(AudioManagerTracks.UI);
        }
        
        uiSlider.value = AudioManager.Instance.state.uiVolume;
    }

    public void PlayClip(string id)
    {
        AudioManager.Instance.PlaySound(id);
    }

    public void StopClip(string id)
    {
        AudioManager.Instance.StopSound(id);
    }

    public void StopTrack(string trackId)
    {
        Enum.TryParse(trackId, out AudioManagerTracks track);
        AudioManager.Instance.StopTrack(track);
    }
}
