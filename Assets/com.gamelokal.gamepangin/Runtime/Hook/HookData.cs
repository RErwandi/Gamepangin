using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    public class HookData<T> : DataDefinition<HookData<T>>
    {
        [SerializeField, ReadOnly] private T reference;

        public T Reference
        {
            get => reference;
            set => reference = value;
        }
    }
}
