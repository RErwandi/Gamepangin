using UnityEngine;
using UnityEngine.Events;

namespace Erwandi.Gamepangin.Utilities
{
    public class Timer : MonoBehaviour
    {
        public bool startOnEnable;

        [Range(0,999)]
        public uint hours = 0;
        [Range(0,59)]
        public uint minutes = 1;
        [Range(0,59)]
        public uint seconds = 30;
        [Range(0,999)]
        public uint milliseconds = 0;

        public uint CurrentHours => (uint)Mathf.Floor(timer / 3600);
        public uint CurrentMinutes => (uint)Mathf.Floor(timer / 60) % 60;
        public uint CurrentSeconds => (uint)Mathf.Floor(timer) % 60;
        public uint CurrentMilliseconds => (uint)((timer % 1.0f) * 1000);
        
        public UnityEvent onTimerStart;
        public UnityEvent onTimerFinished;
        public UnityEvent onTimerInterrupt;

        private float timer = 0.0f;

        public bool IsRunning => timer > 0.0f;

        public void OnEnable()
        {
            if (startOnEnable)
                StartTimer();
            else
                timer = 0.0f;
        }

        public void StartTimer()
        {
            timer = hours * 3600 + minutes * 60 + seconds + milliseconds * 0.001f;
            onTimerStart?.Invoke();
        }
        
        public void StartTimer(uint hours, uint minutes, uint seconds, uint milliseconds)
        {
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.milliseconds = milliseconds;
            
            timer = hours * 3600 + minutes * 60 + seconds + milliseconds * 0.001f;
            onTimerStart?.Invoke();
        }

        public void Update()
        {
            if(timer > 0.0f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0.0f)
                {
                    timer = 0.0f;
                    onTimerFinished?.Invoke();
                }
            }
        }

        public void Interrupt(GameObject instigator = null)
        {
            if (!(timer > 0.0f)) return;
            
            timer = 0.0f;
            onTimerInterrupt?.Invoke();
        }
    }
}