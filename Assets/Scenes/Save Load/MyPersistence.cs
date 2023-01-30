using System;
using System.Threading.Tasks;
using Gamepangin;
using Sirenix.OdinInspector;
using UnityEngine;

public class MyPersistence : MonoBehaviour, IGameSave
{
    public MyState state;
    
    public string SaveId => "My Persistence";
    public bool IsShared => false;
    public Type SaveType => typeof(MyState);
    public object SaveData => state;
    public LoadMode LoadMode => LoadMode.OnLoad;

    private void OnEnable()
    {
        _ = SaveLoadManager.Subscribe(this);
    }

    private void OnDisable()
    {
        SaveLoadManager.Unsubscribe(this);
    }

    [Button]
    private async Task SaveGame(int slot)
    {
        Debug.Log($"Saving game...");
        await SaveLoadManager.Instance.Save(slot);
        Debug.Log($"Game saved!");
    }

    [Button]
    private async Task LoadGame(int slot)
    {
        Debug.Log($"Loading game...");
        await SaveLoadManager.Instance.Load(slot);
        Debug.Log($"Game loaded!");
    }

    [Button]
    private void DeleteSlot(int slot)
    {
        _ = SaveLoadManager.Instance.Delete(slot);
        Debug.Log($"Slot deleted");
    }

    public Task OnLoad(object value)
    {
        var loaded = (MyState)value;
        
        state.myName = loaded.myName;
        state.myAge = loaded.myAge;
        state.isMarried = loaded.isMarried;
        
        return Task.FromResult(1);
    }
}
