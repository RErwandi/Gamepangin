using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    public class Timer : MonoBehaviour
    {
        public float duration = 5f;
        public bool startOnEnable;

        private float timer;
        public float CurrentTimer => timer;

        public UnityEvent onTimerStart = new();
        public UnityEvent onTimerFinished = new();
        public UnityEvent onTimerInterrupt = new();

        private void OnEnable()
        {
            if (startOnEnable)
            {
                StartTimer();
            }
            else
            {
                timer = 0f;
            }
        }

        private void Update()
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    onTimerFinished?.Invoke();
                }
                
                OnProcess();
            }
        }

        public virtual void OnProcess()
        {
            
        }

        public void StartTimer()
        {
            timer = duration;
            onTimerStart?.Invoke();
        }

        public void Interrupt()
        {
            if (timer > 0f)
            {
                timer = 0f;
                onTimerInterrupt?.Invoke();
            }
        }
    }
}