using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    public class State : MonoBehaviour
    {
        public string StateName => gameObject.name;

        public UnityEvent onStateEnter;
        public UnityEvent onStateExit;
    }
}