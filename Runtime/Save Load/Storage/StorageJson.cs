using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    public class StorageJson : DataStorage
    {
        public string filename = "savefile.json";
        private static Dictionary<string, StoreType> data;
        
#if UNITY_EDITOR
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode() => data = null;
#endif

        public bool useEncryption;
        [ShowIf("useEncryption")]
        public string encryptionKey = "Gamepangin";
        
        public override Task DeleteAll()
        {
            Data.Clear();
            return Task.FromResult(1);
        }

        public override Task DeleteKey(string key)
        {
            Data.Remove(key);
            return Task.FromResult(1);
        }

        public override Task<bool>HasKey(string key)
        {
            bool hasKey = Data.ContainsKey(key);
            return Task.FromResult(hasKey);
        }

        // GETTERS: -------------------------------------------------------------------------------

        public override Task<object> GetBlob(string key, Type type, object value)
        {
            if (!Data.TryGetValue(key, out StoreType storeType)) return Task.FromResult(value);
            if (storeType is not StoreString storeString) return Task.FromResult(value);
            
            string json = storeString.Value;
                
            if (!string.IsNullOrEmpty(json)) value = JsonUtility.FromJson(json, type);
            return Task.FromResult(value);
        }

        // SETTERS: -------------------------------------------------------------------------------

        public override Task SetBlob(string key, object value)
        {
            string json = JsonUtility.ToJson(value);
            
            Data[key] = new StoreString(key, json);
            return Task.FromResult(1);
        }

        public override Task Commit()
        {
            string path = Path.Combine(Application.persistentDataPath, filename);
            
            try
            {
                string directory = Path.GetDirectoryName(path) ?? string.Empty;
                Directory.CreateDirectory(directory);
                
                Block content = new Block(data);
                string json = JsonUtility.ToJson(content, true);
                
                if(useEncryption)
                    json = Encrypt(json);

                using FileStream stream = new FileStream(path, FileMode.Create);
                using StreamWriter writer = new StreamWriter(stream);
                
                writer.Write(json);
            }
            catch (Exception exception) 
            {
                Debug.LogError($"Error trying to save data: {exception}");
            }
            
            return Task.FromResult(1);
        }
        
        private Dictionary<string, StoreType> Data
        {
            get
            {
                if (data != null) return data;
                
                data = new Dictionary<string, StoreType>();
                Block content = null;
                    
                string path = Path.Combine(Application.persistentDataPath, filename);

                if (File.Exists(path)) 
                {
                    try 
                    {
                        string json;
                        using (FileStream stream = new FileStream(path, FileMode.Open))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                json = reader.ReadToEnd();
                            }
                        }
                        
                        if(useEncryption)
                            json = Decrypt(json);
                        
                        content = JsonUtility.FromJson<Block>(json);
                    }
                    catch (Exception exception) 
                    {
                        Debug.LogError($"Error trying to load data: {exception}");
                    }
                }

                foreach (StoreType value in content?.Values ?? Array.Empty<StoreType>())
                {
                    data[value.Key] = value;
                }

                return data;
            }
        }
        
        private string Encrypt(string input)
        {
            var output = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                int secretIndex = i % encryptionKey.Length;
                output.Append(input[i] ^ encryptionKey[secretIndex]);
            }
            
            return output.ToString();
        }
        
        private string Decrypt(string input)
        {
            var output = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                int secretIndex = i % encryptionKey.Length;
                output.Append(input[i] ^ encryptionKey[secretIndex]);
            }
            
            return output.ToString();
        }
        
        [Serializable]
        private class Block
        {
            [SerializeReference] private StoreType[] values;

            public StoreType[] Values => values;

            public Block(Dictionary<string, StoreType> data)
            {
                values = new StoreType[data.Count];
                int index = 0;
                
                foreach (KeyValuePair<string, StoreType> entry in data)
                {
                    values[index] = entry.Value;
                    index += 1;
                }
            }
        }
        
        [Serializable]
        private abstract class StoreType
        {
            [SerializeField] private string key;

            public string Key => key;

            protected StoreType(string key)
            {
                this.key = key;
            }
        }
        
        [Serializable]
        private class StoreString : StoreType
        {
            [SerializeField] private string value;

            public string Value => value;
            
            public StoreString(string key, string value) : base(key)
            {
                this.value = value;
            }
        }
    }
}