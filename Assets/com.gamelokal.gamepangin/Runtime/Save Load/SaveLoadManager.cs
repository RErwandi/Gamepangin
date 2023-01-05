using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    public enum LoadMode
    {
        /// <summary>
        /// Immediately mode disables firing the OnLoad interface method when the
        /// SaveLoadSystem.Load() is executed. Instead, it will immediately load when subscribing.
        /// </summary>
        Immediately,

        /// <summary>
        /// OnLoad mode forces its loading whenever the SaveLoadSystem.Load() method is executed.
        /// </summary>
        OnLoad
    }
    
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        private struct Reference
        {
            public IGameSave reference;
            public int priority;
        }
        
        private struct Value
        {
            public object value;
            public bool isShared;
        }
        
        [Required]
        public DataStorage dataStorage;

        private Slots slots;
        
        private const int SLOT_MIN = 1;
        private const int SLOT_MAX = 9999;
        private const string DB_KEY_FORMAT = "data-{0:D4}-{1}";
        
        private Dictionary<string, Reference> subscribers= new();
        private Dictionary<string, Value> values = new();
        
        public int SlotLoaded { get; private set; } = -1;
        public bool IsGameLoaded => SlotLoaded > 0;
        public bool IsSaving { get; private set; } 
        public bool IsLoading { get; private set; }
        public bool IsDeleting { get; private set; }
        
        public event Action<int> EventBeforeSave;
        public event Action<int> EventAfterSave;

        public event Action<int> EventBeforeLoad;
        public event Action<int> EventAfterLoad;

        public event Action<int> EventBeforeDelete;
        public event Action<int> EventAfterDelete;

        protected override void Awake()
        {
            base.Awake();

            subscribers = new Dictionary<string, Reference>();
            values = new Dictionary<string, Value>();
            slots = new Slots();

            _ = Subscribe(slots, 100);
            _ = LoadItem(slots, 0);
        }

        public static async Task Subscribe(IGameSave reference, int priority = 0)
        {
            if (ApplicationManager.IsExiting) return;
            
            Instance.subscribers[reference.SaveId] = new Reference
            {
                reference = reference,
                priority = priority
            };
            
            switch (reference.LoadMode)
            {
                case LoadMode.Immediately:
                    if (Instance.values.TryGetValue(reference.SaveId, out Value value))
                    {
                        await reference.OnLoad(value.value);
                    }
                    else if (Instance.IsGameLoaded)
                    {
                        await Instance.LoadItem(reference, Instance.SlotLoaded);
                    }
                    break;
                
                case LoadMode.OnLoad:
                    if (reference.IsShared)
                    {
                        await Instance.LoadItem(reference, 0);
                    }
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static void Unsubscribe(IGameSave reference)
        {
            if (ApplicationManager.IsExiting) return;
            if (Instance.IsLoading) return;

            Instance.subscribers.Remove(reference.SaveId);
            Instance.values[reference.SaveId] = new Value
            {
                value = reference.SaveData,
                isShared = reference.IsShared
            };
        }
        
        public bool HasSave()
        {
            return slots.Count > 0;
        }

        public bool HasSaveAt(int slot)
        {
            return slots.ContainsKey(slot);
        }

        public async Task Save(int slot)
        {
            if (IsSaving || IsLoading || IsDeleting) return;
            if (slot == 0)
            {
                Debug.LogWarning("Can't save on slot 0 because it's reserved for slots");
                return;
            }

            EventBeforeSave?.Invoke(slot);

            IsSaving = true;

            foreach (KeyValuePair<string, Reference> item in subscribers)
            {
                if (item.Value.reference == null) continue;
                values[item.Value.reference.SaveId] = new Value
                {
                    value = item.Value.reference.SaveData,
                    isShared = item.Value.reference.IsShared
                };
            }

            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, Value> entry in values)
            {
                if (entry.Value.isShared) continue;
                keys.Add(entry.Key);
            }
            
            slots.Update(slot, keys.ToArray());

            foreach (KeyValuePair<string, Value> item in values)
            {
                string key = DatabaseKey(slot, item.Value.isShared, item.Key);
                await dataStorage.SetBlob(key, item.Value.value);
            }

            await dataStorage.Commit();
            IsSaving = false;

            EventAfterSave?.Invoke(slot);
        }
        
        public async Task Load(int slot, Action callback = null)
        {
            if (IsSaving || IsLoading || IsDeleting) return;
            if (slot == 0)
            {
                Debug.LogWarning("Can't load on slot 0 because it's reserved for slots");
                return;
            }

            if (!HasSaveAt(slot))
            {
                Debug.LogWarning($"Slot {slot} is empty!");
                return;
            }

            EventBeforeLoad?.Invoke(slot);
            
            IsLoading = true;
            SlotLoaded = slot;

            values.Clear();

            List<Reference> references = subscribers.Values.ToList();
            references.Sort((a, b) => b.priority.CompareTo(a.priority));

            for (int i = 0; i < references.Count; ++i)
            {
                IGameSave item = references[i].reference;
                if (item == null) continue;
                if (item.LoadMode == LoadMode.Immediately) continue;

                await LoadItem(references[i].reference, slot);
            }
            
            IsLoading = false;

            callback?.Invoke();
            EventAfterLoad?.Invoke(slot);
        }
        
        public async Task LoadLatest(Action callback = null)
        {
            int slot = slots.LatestSlot;
            
            if (slot < 0) return;
            await Load(slot, callback);
        }
        
        public async Task Delete(int slot)
        {
            if (IsSaving || IsLoading || IsDeleting) return;
            
            if (slot == 0)
            {
                Debug.LogWarning("Can't delete on slot 0 because it's reserved for slots");
                return;
            }

            EventBeforeDelete?.Invoke(slot);
            IsDeleting = true;

            if (slots.TryGetValue(slot, out Slots.Data data))
            {
                for (int i = data.keys.Length - 1; i >= 0; --i)
                {
                    string dataKey = DatabaseKey(slot, false, data.keys[i]);
                    await dataStorage.DeleteKey(dataKey);
                }

                slots.Remove(slot);

                string key = DatabaseKey(slot, slots.IsShared, slots.SaveId);
                await dataStorage.SetBlob(key, slots.SaveData);
            }

            await dataStorage.Commit();
            IsDeleting = false;
            
            EventAfterDelete?.Invoke(slot);
        }
        
        private async Task LoadItem(IGameSave reference, int slot)
        {
            string key = DatabaseKey(slot, reference.IsShared, reference.SaveId);

            object blob = await dataStorage.GetBlob(key, reference.SaveType, null);
            await reference.OnLoad(blob);
        }
        
        private static string DatabaseKey(int slot, bool isShared, string key)
        {
            slot = isShared ? 0 : Mathf.Clamp(slot, SLOT_MIN, SLOT_MAX);
            return string.Format(DB_KEY_FORMAT, slot, key);
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

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Gamepangin/Open Persistent Data folder", false, 270)]
        private static void OpenPersistentDataFolder()
        {
            string path = Application.persistentDataPath;
            UnityEditor.EditorUtility.RevealInFinder(path);
        }
        
#endif
    }
}