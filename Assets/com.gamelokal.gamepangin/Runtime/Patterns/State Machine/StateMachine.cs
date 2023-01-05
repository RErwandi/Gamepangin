using System;
using System.Linq;
using UnityEngine;

namespace Gamepangin
{
    public class StateMachine : MonoBehaviour
    {
        public string defaultState;
        public State[] states = Array.Empty<State>();
            
        public State CurrentState { get; private set; }
    
        private void Start()
        {
            foreach (var state in states)
            {
                if(state.gameObject.activeSelf)
                    state.gameObject.SetActive(false);
            }
                
            SetState(defaultState);
        }

        /// <summary>
        /// Change state. this will call OnStateExit on the current state before calling OnStateEnter on the target state.
        /// </summary>
        /// <param name="stateName">Name of the state</param>
        public void SetState(string stateName)
        {
            var newState = states.FirstOrDefault(o => o.StateName == stateName);
    
            if(newState != null)
            {
                if (CurrentState != null)
                {
                    CurrentState.onStateExit?.Invoke();
                    CurrentState.gameObject.SetActive(false);
                }
    
                newState.gameObject.SetActive(true);
                CurrentState = newState;
                CurrentState.onStateEnter?.Invoke();
            }
            else
                Debug.Log($"{gameObject.name} : Trying to set unknown state {stateName}");
        }
    }
}