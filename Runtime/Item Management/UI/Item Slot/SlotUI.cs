using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamepangin.UI
{
    [DisallowMultipleComponent]
    public abstract class SlotUI<T> : MonoBehaviour where T : class
    {
        protected T Data { get; private set; }
        
        protected virtual void Awake() => SetData(null);
        
        [SerializeField]
        private DataInfoUI<T>[] dataInfo;
        
        public bool TryGetInfoOfType<U>(out U info) where U : DataInfoBaseUI
        {
            return dataInfo.TryGetElementOfType(out info, true);
        }

        protected void SetData(T data)
        {
            Data = data;
            
            foreach (var info in dataInfo)
                info.SetData(data);
        }
    }
}
