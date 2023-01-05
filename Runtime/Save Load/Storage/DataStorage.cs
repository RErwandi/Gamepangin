using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gamepangin
{
    public abstract class DataStorage : MonoBehaviour
    {
        public abstract Task DeleteAll();
        public abstract Task DeleteKey(string key);
        public abstract Task<bool> HasKey(string key);
        public abstract Task<object> GetBlob(string key, Type type, object value);
        public abstract Task SetBlob(string key, object value);
        public abstract Task Commit();
    }   
}
