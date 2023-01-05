using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gamepangin
{
    public class StoragePlayerPrefs : DataStorage
    {
        public override Task DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            return Task.FromResult(1);
        }

        public override Task DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            return Task.FromResult(1);
        }

        public override Task<bool> HasKey(string key)
        {
            bool hasKey = PlayerPrefs.HasKey(key);
            return Task.FromResult(hasKey);
        }
        
        public override Task<object> GetBlob(string key, Type type, object value)
        {
            string json = PlayerPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(json)) value = JsonUtility.FromJson(json, type);

            return Task.FromResult(value);
        }

        public override Task SetBlob(string key, object value)
        {
            string json = JsonUtility.ToJson(value);

            PlayerPrefs.SetString(key, json);
            return Task.FromResult(1);
        }

        public override Task Commit()
        {
            return Task.FromResult(1);
        }
    }
}