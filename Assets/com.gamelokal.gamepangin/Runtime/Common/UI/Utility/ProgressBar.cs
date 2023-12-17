using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gamepangin
{
    // <summary>
    // Add this bar to an object and link it to a bar (possibly the same object the script is on), and you'll be able to resize the bar object based on a current value, located between a min and max value.
    // See the HealthBar.cs script for a use case
    // </summary>
    public class ProgressBar : MonoBehaviour
	{
		public enum ProgressBarStates {Idle, Decreasing, Increasing, InDecreasingDelay, InIncreasingDelay }
		// the possible fill modes 
        public enum FillModes { LocalScale, FillAmount, Width, Height, Anchor }
        // the possible directions for the fill (for local scale and fill amount only)
        public enum BarDirections { LeftToRight, RightToLeft, UpToDown, DownToUp }
        // the possible timescales the bar can work on
        public enum TimeScales { UnscaledTime, Time }
        // the possible ways to animate the bar fill
        public enum BarFillModes { SpeedBased, FixedDuration }
        
        [TitleGroup("Bindings")]
        
        // the main foreground bar
        public Transform foregroundBar;
        // the delayed bar that will show when moving from a value to a new, lower value
        public Transform delayedBarDecreasing;
        // the delayed bar that will show when moving from a value to a new, higher value
        public Transform delayedBarIncreasing;


        [TitleGroup("Fill Settings")]
        
        // the local scale or fill amount value to reach when the value associated to the bar is at 0%
        [Range(0f,1f)]
        public float minimumBarFillValue;
        // the local scale or fill amount value to reach when the bar is full
        [Range(0f,1f)]
        public float maximumBarFillValue = 1f;
        // whether or not to initialize the value of the bar on start
        public bool setInitialFillValueOnStart = false;
        // the initial value of the bar
        [ShowIf("setInitialFillValueOnStart")]
        [Range(0f,1f)]
        public float initialFillValue;
        // the direction this bar moves to
        public BarDirections barDirection = BarDirections.LeftToRight;
        // the foreground bar's fill mode
        public FillModes fillMode = FillModes.LocalScale;
        // defines whether the bar will work on scaled or unscaled time (whether or not it'll keep moving if time is slowed down for example)
        public TimeScales timeScale = TimeScales.UnscaledTime;
        // the selected fill animation mode
        public BarFillModes barFillMode = BarFillModes.SpeedBased;

        [TitleGroup("Foreground Bar Settings")]
        
		// whether or not the foreground bar should lerp
		public bool lerpForegroundBar = true;
		// the speed at which to lerp the foreground bar
		[ShowIf("lerpForegroundBar")]
		public float lerpForegroundBarSpeedDecreasing = 15f;
		[ShowIf("lerpForegroundBar")]
		public float lerpForegroundBarSpeedIncreasing = 15f;
		[ShowIf("lerpForegroundBar")]
		public float lerpForegroundBarDurationDecreasing = 0.2f;
		// the duration each update of the foreground bar should take (only if in fixed duration bar fill mode)
		[ShowIf("lerpForegroundBar")]
		public float lerpForegroundBarDurationIncreasing = 0.2f;
		// the curve to use when animating the foreground bar fill
		[ShowIf("lerpForegroundBar")]
		public AnimationCurve lerpForegroundBarCurveDecreasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		[ShowIf("lerpForegroundBar")]
		public AnimationCurve lerpForegroundBarCurveIncreasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		
		[TitleGroup("Delayed Bar Decreasing")]
		
		// the delay before the delayed bar moves (in seconds)
		public float decreasingDelay = 1f;
		// whether or not the delayed bar's animation should lerp
		public bool lerpDecreasingDelayedBar = true;
		// the speed at which to lerp the delayed bar
		[ShowIf("lerpDecreasingDelayedBar")]
		public float lerpDecreasingDelayedBarSpeed = 15f;
		// the duration each update of the foreground bar should take (only if in fixed duration bar fill mode)
		[ShowIf("lerpDecreasingDelayedBar", true)]
		public float lerpDecreasingDelayedBarDuration = 0.2f;
		// the curve to use when animating the delayed bar fill
		[ShowIf("lerpDecreasingDelayedBar")]
		public AnimationCurve lerpDecreasingDelayedBarCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[TitleGroup("Delayed Bar Increasing")]
		
		// the delay before the delayed bar moves (in seconds)
		public float increasingDelay = 1f;
		// whether or not the delayed bar's animation should lerp
		public bool lerpIncreasingDelayedBar = true;

		[TitleGroup("Bump")]
		
		// whether or not the bar should "bump" when changing value
		public bool bumpScaleOnChange = true;
		// whether or not the bar should bump when its value increases
		public bool bumpOnIncrease;
		// whether or not the bar should bump when its value decreases
		public bool bumpOnDecrease;
        // the duration of the bump animation
        public float bumpDuration = 0.2f;
        // whether or not the bar should flash when bumping
        public bool changeColorWhenBumping = true;
        // the color to apply to the bar when bumping
        [ShowIf("changeColorWhenBumping")]
        public Color bumpColor = Color.white;
        // the curve to map the bump animation on
        public AnimationCurve bumpScaleAnimationCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
        // the curve to map the bump animation color animation on
        public AnimationCurve bumpColorAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        // whether or not the bar is bumping right now
        public bool Bumping { get; protected set; }

        [TitleGroup("Events")]
        
        // an event to trigger every time the bar bumps
        public UnityEvent onBump;
        // an event to trigger every time the bar starts decreasing
        public UnityEvent onBarMovementDecreasingStart;
        // an event to trigger every time the bar stops decreasing
        public UnityEvent onBarMovementDecreasingStop;
        // an event to trigger every time the bar starts increasing
        public UnityEvent onBarMovementIncreasingStart;
        // an event to trigger every time the bar stops increasing
        public UnityEvent onBarMovementIncreasingStop;

        [TitleGroup("Text")] 
        
        public Text percentageText;
        public string textPrefix;
        public string textSuffix;
        public float textValueMultiplier = 1f;
        public string textFormat = "{000}";

        [TitleGroup("Debug")]
        
        // the value the bar will move to if you press the DebugSet button
        [Range(0f, 1f)] 
        public float debugNewTargetValue;
        
        [TitleGroup("Debug Read Only")]
        
        // the current progress of the bar, ideally read only
        [Range(0f,1f)]
        public float barProgress;// the current progress of the bar, ideally read only
        [Range(0f,1f)]
        public float barTarget;
        [Range(0f,1f)]
        public float delayedBarIncreasingProgress;
        [Range(0f,1f)]
        public float delayedBarDecreasingProgress;

        protected bool initialized;
        protected Vector2 initialBarSize;
        protected Color initialColor;
        protected Vector3 initialScale;
        
        protected Image foregroundImage;
        protected Image delayedDecreasingImage;
        protected Image delayedIncreasingImage;
        
        protected Vector3 targetLocalScale = Vector3.one;
		protected float newPercent;
        protected float percentLastTimeBarWasUpdated;
		protected float lastUpdateTimestamp;
        
        protected float time;
        protected float deltaTime;
        protected int direction;
        protected Coroutine coroutine;
        protected bool coroutineShouldRun;
        protected bool actualUpdate;
        protected Vector2 anchorVector;

        protected float tmpDelayedBarDecreasingProgress;
        protected float tmpDelayedBarIncreasingProgress;
        protected ProgressBarStates currentState = ProgressBarStates.Idle;

        #region PUBLIC_API
        
        // <summary>
        // Updates the bar's values, using a normalized value
        // </summary>
        // <param name="normalizedValue"></param>
        public virtual void UpdateBar01(float normalizedValue) 
        {
	        UpdateBar(Mathf.Clamp01(normalizedValue), 0f, 1f);
        }
        
        // <summary>
        // Updates the bar's values based on the specified parameters
        // </summary>
        // <param name="currentValue">Current value.</param>
        // <param name="minValue">Minimum value.</param>
        // <param name="maxValue">Max value.</param>
        public virtual void UpdateBar(float currentValue,float minValue,float maxValue) 
        {
            if (!initialized)
            {
                Initialization();
            }
            
	        newPercent = MathHelper.Remap(currentValue, minValue, maxValue, minimumBarFillValue, maximumBarFillValue);
	        
	        actualUpdate = (barTarget != newPercent);
	        
	        if (!actualUpdate)
	        {
		        return;
	        }
	        
	        if (currentState != ProgressBarStates.Idle)
	        {
		        if ((currentState == ProgressBarStates.Decreasing) ||
		            (currentState == ProgressBarStates.InDecreasingDelay))
		        {
			        if (newPercent >= barTarget)
			        {
				        StopCoroutine(coroutine);
				        SetBar01(barTarget);
			        }
		        }
		        if ((currentState == ProgressBarStates.Increasing) ||
		            (currentState == ProgressBarStates.InIncreasingDelay))
		        {
			        if (newPercent <= barTarget)
			        {
				        StopCoroutine(coroutine);
				        SetBar01(barTarget);
			        }
		        }
	        }
	        
	        percentLastTimeBarWasUpdated = barProgress;
	        tmpDelayedBarDecreasingProgress = delayedBarDecreasingProgress;
	        tmpDelayedBarIncreasingProgress = delayedBarIncreasingProgress;
	        
	        barTarget = newPercent;
			
	        if ((newPercent != percentLastTimeBarWasUpdated) && !Bumping)
	        {
		        Bump();
	        }

	        DetermineDeltaTime();
	        lastUpdateTimestamp = time;
	        
		    DetermineDirection();
		    if (direction < 0)
		    {
			    onBarMovementDecreasingStart?.Invoke();
		    }
		    else
		    {
			    onBarMovementIncreasingStart?.Invoke();
		    }
		        
		    if (coroutine != null)
		    {
			    StopCoroutine(coroutine);
		    }
		    coroutineShouldRun = true;
		    
		    if (!this.gameObject.activeInHierarchy)
            {
                this.gameObject.SetActive(true);    
		    }

            if (this.gameObject.activeInHierarchy)
            {
                coroutine = StartCoroutine(UpdateBarsCo());
            }                

            UpdateText();
        }

        // <summary>
        // Sets the bar value to the one specified 
        // </summary>
        // <param name="currentValue"></param>
        // <param name="minValue"></param>
        // <param name="maxValue"></param>
        public virtual void SetBar(float currentValue, float minValue, float maxValue)
        {
	        float newPercent = MathHelper.Remap(currentValue, minValue, maxValue, 0f, 1f);
	        SetBar01(newPercent);
        }

        // <summary>
        // Sets the bar value to the normalized value set in parameter
        // </summary>
        // <param name="newPercent"></param>
        public virtual void SetBar01(float newPercent)
        {
            if (!initialized)
            {
                Initialization();
            }

            newPercent = MathHelper.Remap(newPercent, 0f, 1f, minimumBarFillValue, maximumBarFillValue);
	        barProgress = newPercent;
	        delayedBarDecreasingProgress = newPercent;
	        delayedBarIncreasingProgress = newPercent;
	        //_newPercent = newPercent;
	        barTarget = newPercent;
	        percentLastTimeBarWasUpdated = newPercent;
	        tmpDelayedBarDecreasingProgress = delayedBarDecreasingProgress;
	        tmpDelayedBarIncreasingProgress = delayedBarIncreasingProgress;
	        SetBarInternal(newPercent, foregroundBar, foregroundImage, initialBarSize);
	        SetBarInternal(newPercent, delayedBarDecreasing, delayedDecreasingImage, initialBarSize);
	        SetBarInternal(newPercent, delayedBarIncreasing, delayedIncreasingImage, initialBarSize);
	        UpdateText();
	        coroutineShouldRun = false;
	        currentState = ProgressBarStates.Idle;
        }
        
        #endregion PUBLIC_API

        #region START
        
        // <summary>
        // On start we store our image component
        // </summary>
        protected virtual void Start()
		{
            Initialization();
        }

        protected virtual void OnEnable()
        {
            if (!initialized)
            {
                return;
            }

            if (foregroundImage != null)
            {
	            foregroundImage.color = initialColor;    
            }
        }

        public virtual void Initialization()
        {
            initialScale = this.transform.localScale;

            if (foregroundBar != null)
            {
                foregroundImage = foregroundBar.GetComponent<Image>();
                initialBarSize = foregroundImage.rectTransform.sizeDelta;
            }
            if (delayedBarDecreasing != null)
            {
                delayedDecreasingImage = delayedBarDecreasing.GetComponent<Image>();
            }
            if (delayedBarIncreasing != null)
            {
                delayedIncreasingImage = delayedBarIncreasing.GetComponent<Image>();
            }
            initialized = true;

            if (foregroundImage != null)
            {
                initialColor = foregroundImage.color;
            }

            percentLastTimeBarWasUpdated = barProgress;

            if (setInitialFillValueOnStart)
            {
                SetBar01(initialFillValue);
            }
        }
        
        #endregion START

        #region TESTS

        // <summary>
        // This test method, called via the inspector button of the same name, lets you test what happens when you update the bar to a certain value
        // </summary>
        protected virtual void DebugUpdateBar()
        {
	        this.UpdateBar01(debugNewTargetValue);
        }
        
        // <summary>
        // Test method
        // </summary>
        protected virtual void DebugSetBar()
        {
	        this.SetBar01(debugNewTargetValue);
        }

        // <summary>
        // Test method
        // </summary>
        public virtual void Plus10Percent()
        {
	        float newProgress = barTarget + 0.1f;
	        newProgress = Mathf.Clamp(newProgress, 0f, 1f);
	        UpdateBar01(newProgress);
        }
        
        // <summary>
        // Test method
        // </summary>
        public virtual void Minus10Percent()
        {
	        float newProgress = barTarget - 0.1f;
	        newProgress = Mathf.Clamp(newProgress, 0f, 1f);
	        UpdateBar01(newProgress);
        }


        #endregion TESTS

        
        
        protected virtual void UpdateText()
        {
	        if (percentageText == null)
	        {
		        return;
	        }

	        percentageText.text = textPrefix + (barTarget * textValueMultiplier).ToString(textFormat) + textSuffix;
        }
        
		// <summary>
		// On Update we update our bars
		// </summary>
		protected virtual IEnumerator UpdateBarsCo()
		{
			while (coroutineShouldRun)
			{
				DetermineDeltaTime();
				DetermineDirection();
				UpdateBars();
				yield return null;
			}

			currentState = ProgressBarStates.Idle;
			yield break;
		}
		
        protected virtual void DetermineDeltaTime()
        {
	        deltaTime = (timeScale == TimeScales.Time) ? Time.deltaTime : Time.unscaledDeltaTime;
	        time = (timeScale == TimeScales.Time) ? Time.time : Time.unscaledTime;
        }

        protected virtual void DetermineDirection()
        {
		    direction = (newPercent > percentLastTimeBarWasUpdated) ? 1 : -1;
        }

		// <summary>
		// Updates the foreground bar's scale
		// </summary>
		protected virtual void UpdateBars()
		{
			float newFill;
			float newFillDelayed;
			float t1, t2 = 0f;
			
			// if the value is decreasing
			if (direction < 0)
			{
				newFill = ComputeNewFill(lerpForegroundBar, lerpForegroundBarSpeedDecreasing, lerpForegroundBarDurationDecreasing, lerpForegroundBarCurveDecreasing, 0f, percentLastTimeBarWasUpdated, out t1);
				SetBarInternal(newFill, foregroundBar, foregroundImage, initialBarSize);
				SetBarInternal(newFill, delayedBarIncreasing, delayedIncreasingImage, initialBarSize);

				barProgress = newFill;
				delayedBarIncreasingProgress = newFill;

				currentState = ProgressBarStates.Decreasing;
				
				if (time - lastUpdateTimestamp > decreasingDelay)
				{
					newFillDelayed = ComputeNewFill(lerpDecreasingDelayedBar, lerpDecreasingDelayedBarSpeed, lerpDecreasingDelayedBarDuration, lerpDecreasingDelayedBarCurve, decreasingDelay,tmpDelayedBarDecreasingProgress, out t2);
					SetBarInternal(newFillDelayed, delayedBarDecreasing, delayedDecreasingImage, initialBarSize);

					delayedBarDecreasingProgress = newFillDelayed;
					currentState = ProgressBarStates.InDecreasingDelay;
				}
			}
			else // if the value is increasing
			{
				newFill = ComputeNewFill(lerpForegroundBar, lerpForegroundBarSpeedIncreasing, lerpForegroundBarDurationIncreasing, lerpForegroundBarCurveIncreasing, 0f, tmpDelayedBarIncreasingProgress, out t1);
				SetBarInternal(newFill, delayedBarIncreasing, delayedIncreasingImage, initialBarSize);
				
				delayedBarIncreasingProgress = newFill;
				currentState = ProgressBarStates.Increasing;

				if (delayedBarIncreasing == null)
				{
					newFill = ComputeNewFill(lerpForegroundBar, lerpForegroundBarSpeedIncreasing, lerpForegroundBarDurationIncreasing, lerpForegroundBarCurveIncreasing, 0f, percentLastTimeBarWasUpdated, out t2);
					SetBarInternal(newFill, delayedBarDecreasing, delayedDecreasingImage, initialBarSize);
					SetBarInternal(newFill, foregroundBar, foregroundImage, initialBarSize);
					
					barProgress = newFill;	
					delayedBarDecreasingProgress = newFill;
					currentState = ProgressBarStates.InDecreasingDelay;
				}
				else
				{
					if (time - lastUpdateTimestamp > increasingDelay)
					{
						newFillDelayed = ComputeNewFill(lerpIncreasingDelayedBar, lerpForegroundBarSpeedIncreasing, lerpForegroundBarDurationIncreasing, lerpForegroundBarCurveIncreasing, increasingDelay, tmpDelayedBarDecreasingProgress, out t2);
					
						SetBarInternal(newFillDelayed, delayedBarDecreasing, delayedDecreasingImage, initialBarSize);
						SetBarInternal(newFillDelayed, foregroundBar, foregroundImage, initialBarSize);
					
						barProgress = newFillDelayed;	
						delayedBarDecreasingProgress = newFillDelayed;
						currentState = ProgressBarStates.InDecreasingDelay;
					}
				}
			}
			
			if ((t1 >= 1f) && (t2 >= 1f))
			{
				coroutineShouldRun = false;
				if (direction > 0)
				{
					onBarMovementIncreasingStop?.Invoke();
				}
				else
				{
					onBarMovementDecreasingStop?.Invoke();
				}
			}
		}

		protected virtual float ComputeNewFill(bool lerpBar, float barSpeed, float barDuration, AnimationCurve barCurve, float delay, float lastPercent, out float t)
		{
			float newFill = 0f;
			t = 0f;
			if (lerpBar)
			{
				float delta = 0f;
				float timeSpent = time - lastUpdateTimestamp - delay;
				float speed = barSpeed;
				if (speed == 0f) { speed = 1f; }
				
				float duration = (barFillMode == BarFillModes.FixedDuration) ? barDuration : (Mathf.Abs(newPercent - lastPercent)) / speed;
				
				delta = MathHelper.Remap(timeSpent, 0f, duration, 0f, 1f);
				delta = Mathf.Clamp(delta, 0f, 1f);
				t = delta;
				if (t < 1f)
				{
					delta = barCurve.Evaluate(delta);
					newFill = Mathf.LerpUnclamped(lastPercent, newPercent, delta);	
				}
				else
				{
					newFill = newPercent;
				}
			}
			else
			{
				newFill = newPercent;
			}

			newFill = Mathf.Clamp( newFill, 0f, 1f);

			return newFill;
		}

		protected virtual void SetBarInternal(float newAmount, Transform bar, Image image, Vector2 initialSize)
		{
			if (bar == null)
			{
                return;
			}
			
			switch (fillMode)
            {
                case FillModes.LocalScale:
                    targetLocalScale = Vector3.one;
                    switch (barDirection)
                    {
                        case BarDirections.LeftToRight:
                            targetLocalScale.x = newAmount;
                            break;
                        case BarDirections.RightToLeft:
                            targetLocalScale.x = 1f - newAmount;
                            break;
                        case BarDirections.DownToUp:
                            targetLocalScale.y = newAmount;
                            break;
                        case BarDirections.UpToDown:
                            targetLocalScale.y = 1f - newAmount;
                            break;
                    }

                    bar.localScale = targetLocalScale;
                    break;

                case FillModes.Width:
                    if (image == null)
                    {
                        return;
                    }
                    float newSizeX = MathHelper.Remap(newAmount, 0f, 1f, 0, initialSize.x);
                    image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSizeX);
                    break;

                case FillModes.Height:
                    if (image == null)
                    {
                        return;
                    }
                    float newSizeY = MathHelper.Remap(newAmount, 0f, 1f, 0, initialSize.y);
                    image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSizeY);
                    break;

                case FillModes.FillAmount:
                    if (image == null)
                    {
                        return;
                    }
                    image.fillAmount = newAmount;
                    break;
                case FillModes.Anchor:
	                if (image == null)
	                {
		                return;
	                }
	                switch (barDirection)
	                {
		                case BarDirections.LeftToRight:
			                anchorVector.x = 0f;
			                anchorVector.y = 0f;
			                image.rectTransform.anchorMin = anchorVector;
			                anchorVector.x = newAmount;
			                anchorVector.y = 1f;
			                image.rectTransform.anchorMax = anchorVector;
			                break;
		                case BarDirections.RightToLeft:
			                anchorVector.x = newAmount;
			                anchorVector.y = 0f;
			                image.rectTransform.anchorMin = anchorVector;
			                anchorVector.x = 1f;
			                anchorVector.y = 1f;
			                image.rectTransform.anchorMax = anchorVector;
			                break;
		                case BarDirections.DownToUp:
			                anchorVector.x = 0f;
			                anchorVector.y = 0f;
			                image.rectTransform.anchorMin = anchorVector;
			                anchorVector.x = 1f;
			                anchorVector.y = newAmount;
			                image.rectTransform.anchorMax = anchorVector;
			                break;
		                case BarDirections.UpToDown:
			                anchorVector.x = 0f;
			                anchorVector.y = newAmount;
			                image.rectTransform.anchorMin = anchorVector;
			                anchorVector.x = 1f;
			                anchorVector.y = 1f;
			                image.rectTransform.anchorMax = anchorVector;
			                break;
	                }
	                break;
            }
		}

		#region  Bump

		// <summary>
		// Triggers a camera bump
		// </summary>
		public virtual void Bump()
		{
			bool shouldBump = false;

			if (!initialized)
			{
				return;
			}
			
			DetermineDirection();
			
			if (bumpOnIncrease && (direction > 0))
			{
				shouldBump = true;
			}
			
			if (bumpOnDecrease && (direction < 0))
			{
				shouldBump = true;
			}
			
			if (bumpScaleOnChange)
			{
				shouldBump = true;
			}

			if (!shouldBump)
			{
				return;
			}
			
			if (this.gameObject.activeInHierarchy)
			{
				StartCoroutine(BumpCoroutine());
			}

			onBump?.Invoke();
		}

		// <summary>
		// A coroutine that (usually quickly) changes the scale of the bar 
		// </summary>
		// <returns>The coroutine.</returns>
		protected virtual IEnumerator BumpCoroutine()
		{
			float journey = 0f;

			Bumping = true;

			while (journey <= bumpDuration)
			{
				journey = journey + deltaTime;
				float percent = Mathf.Clamp01(journey / bumpDuration);
				float curvePercent = bumpScaleAnimationCurve.Evaluate(percent);
				float colorCurvePercent = bumpColorAnimationCurve.Evaluate(percent);
				this.transform.localScale = curvePercent * initialScale;

				if (changeColorWhenBumping && (foregroundImage != null))
				{
					foregroundImage.color = Color.Lerp(initialColor, bumpColor, colorCurvePercent);
				}

				yield return null;
			}
			if (changeColorWhenBumping && (foregroundImage != null))
			{
				foregroundImage.color = initialColor;
			}
			Bumping = false;
			yield return null;
		}

		#endregion Bump

		#region ShowHide

		// <summary>
		// A simple method you can call to show the bar (set active true)
		// </summary>
		public virtual void ShowBar()
		{
			this.gameObject.SetActive(true);
		}

		// <summary>
		// Hides (SetActive false) the progress bar object, after an optional delay
		// </summary>
		// <param name="delay"></param>
		public virtual void HideBar(float delay)
		{
			if (delay <= 0)
			{
				this.gameObject.SetActive(false);
			}
			else if (this.gameObject.activeInHierarchy)
			{
				StartCoroutine(HideBarCo(delay));
			}
		}

		// <summary>
		// An internal coroutine used to handle the disabling of the progress bar after a delay
		// </summary>
		// <param name="delay"></param>
		// <returns></returns>
		protected virtual IEnumerator HideBarCo(float delay)
		{
			yield return new WaitForSeconds(delay);
			this.gameObject.SetActive(false);
		}

		#endregion ShowHide
		
	}
}