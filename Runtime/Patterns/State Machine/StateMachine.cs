using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Erwandi.Gamepangin.Patterns
{
    [AddComponentMenu("Gamepangin/State Machine")]
    public class StateMachine : MonoBehaviour
    {
        [ValueDropdown("AvailableStates"), BoxGroup("State Machine"), Required]
        public string defaultState;
        [BoxGroup("State Machine")]
        public State[] states = new State[0];

        public State CurrentState { get; private set; }

        [Button(ButtonSizes.Large), BoxGroup("State Machine"), HideInPlayMode]
        private void AddNewState()
        {
            var newState = new GameObject($"State {states.Length}");
            var state = newState.AddComponent<State>();
            newState.transform.parent = transform;
            newState.transform.localPosition = Vector3.zero;
            newState.transform.localRotation = Quaternion.identity;
            newState.transform.localScale = Vector3.one;
            states = states.Concat(new[] { state }).ToArray();

            if (CurrentState == null)
                CurrentState = state;
        }

        [ValueDropdown("AvailableStates"), BoxGroup("Debug"), ShowInInspector, DisableInEditorMode]
        private string testState = "";

        [Button(ButtonSizes.Large), BoxGroup("Debug"), DisableInEditorMode]
        private void ChangeState()
        {
            SetState(testState);
        }

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
        /// Change current state to the new state
        /// </summary>
        /// <param name="stateName">New state name</param>
        public void SetState(string stateName)
        {
            var newState = states.FirstOrDefault(o => o.StateName == stateName);

            if(newState != null)
            {
                if (CurrentState != null)
                {
                    // Call Exit Actions
                    CurrentState.onStateExit?.Invoke();
                    // Then finally disable old state
                    CurrentState.gameObject.SetActive(false);
                }

                // Switch Active new state
                newState.gameObject.SetActive(true);

                // Then Set new current state
                CurrentState = newState;

                // Finally, call State enter
                CurrentState.onStateEnter?.Invoke();
            }
            else
                Debug.LogWarning($"{gameObject.name} : Trying to set unknown state {stateName}", gameObject);
        }

        private List<string> AvailableStates
        {
            get
            {
                return states.Select(state => state.StateName).ToList();
            }
        }
    }
}

