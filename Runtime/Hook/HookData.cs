using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    [CreateAssetMenu(order = 0, fileName = "New Hook", menuName = "Gamepangin/Hook")]
    public class HookData : DataDefinition<HookData>
    {
        [SerializeField, ReadOnly] private GameObject gameObject;

        public GameObject GameObject
        {
            get => gameObject;
            set => gameObject = value;
        }
    }
}
