using System;
using System.Threading.Tasks;
using Gamepangin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoPlayer : MonoBehaviour, IGameSave
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Toggle toggle;
    
    private PlayerState state;
    
    public string SaveId => "My Demo Player";
    public bool IsShared => false;
    public Type SaveType => typeof(PlayerState);
    public object SaveData => state;
    public LoadMode LoadMode => LoadMode.OnLoad;

    private void OnEnable()
    {
        _ = SaveLoadManager.Subscribe(this);
        SaveLoadManager.Instance.EventAfterLoad += AfterLoad;
    }
    
    private void OnDisable()
    {
        if (ApplicationManager.IsExiting) return;
        
        SaveLoadManager.Unsubscribe(this);
        SaveLoadManager.Instance.EventAfterLoad -= AfterLoad;
    }

    public void SaveGame()
    {
        _ = SaveLoadManager.Instance.Save(1);
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        _ = SaveLoadManager.Instance.Load(1);
        Debug.Log("Game Loaded");
    }

    public void UpdateToggleState(bool isOn)
    {
        state.toggle = isOn;
    }

    public void UpdateInputState(string text)
    {
        state.input = text;
    }

    public void UpdateSliderState(float value)
    {
        state.slider = value;
    }

    private void AfterLoad(int slot)
    {
        toggle.isOn = state.toggle;
        slider.value = state.slider;
        inputField.text = state.input;
    }

    public Task OnLoad(object value)
    {
        var loaded = (PlayerState)value;
        
        state.input = loaded.input;
        state.slider = loaded.slider;
        state.toggle = loaded.toggle;
        
        return Task.FromResult(1);
    }
}
