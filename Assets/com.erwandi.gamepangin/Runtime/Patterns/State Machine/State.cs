using UnityEngine;
using UnityEngine.Events;

namespace Erwandi.Gamepangin.Patterns {
    public class State : MonoBehaviour
    {
        public string StateName => gameObject.name;

        public UnityEvent onStateEnter;
        public UnityEvent onStateExit;
    }
}