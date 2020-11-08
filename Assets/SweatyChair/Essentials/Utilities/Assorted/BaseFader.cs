using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI.CoroutineTween;
using UnityEngine.UI.Extensions.Tweens;

namespace SweatyChair
{

	public abstract class BaseFader : MonoBehaviour
	{

		#region Variables

		[Header("Preset")]
		[SerializeField] protected BaseFaderPreset _faderPreset;
		protected static Type _faderPresetType => typeof(BaseFader);
		[Space()]


		[Header("Default Values")]
		[FormerlySerializedAs("_hoverEaseType")] [SerializeField] protected EasingFunction.Ease _fadeEaseType = EasingFunction.Ease.EaseOutQuad;
		[FormerlySerializedAs("_hoverAnimationTime")] [SerializeField] protected float _fadeAnimationTime = 1;

		public EasingFunction.Ease fadeEaseType { get { return _fadeEaseType; } }
		public float fadeAnimationTime { get { return _fadeAnimationTime; } }

		public float targetValue { get; private set; } = -1;
		public bool isFadingOut { get { return targetValue == 0; } }
		public abstract float currentValue { get; }
		public abstract bool isFadedOut { get; }

		#region Tween Variables

		[NonSerialized] private TweenRunner<EaseTypeTween> m_AnimCurveTweenRunner;
		public bool _isTweening { get; private set; }

		private TweenRunner<EaseTypeTween> animCurveTweenRunner {
			get {
				if (m_AnimCurveTweenRunner == null) {
					m_AnimCurveTweenRunner = new TweenRunner<EaseTypeTween>();
					m_AnimCurveTweenRunner.Init(this);
				}
				return m_AnimCurveTweenRunner;
			}
		}

		#endregion

		#endregion

		#region OnValidate

#if UNITY_EDITOR

		protected virtual void OnValidate()
		{
			ApplyPreset();
		}

#endif

		#endregion

		#region Awake

		protected virtual void Awake()
		{
			ApplyPreset();
		}

		#endregion

		#region Presets

		protected virtual void ApplyPreset()
		{
			if (_faderPreset != null) {
				// Apply our preset values
				_fadeEaseType = _faderPreset.faseEaseType;
				_fadeAnimationTime = _faderPreset.fadeAnimationTime;
			}
		}

		public virtual void SetPreset(BaseFaderPreset preset)
		{
			// Set and apply our preset
			_faderPreset = preset;

			ApplyPreset();
		}

		#endregion

		#region FadeIn / Fade Out

		#region Fade

		public void Fade(bool fadeOut)
		{
			Fade(fadeOut, _fadeAnimationTime, _fadeEaseType);
		}
		public void Fade(bool fadeOut, bool isInstant)
		{
			Fade(fadeOut, (isInstant) ? 0 : _fadeAnimationTime, _fadeEaseType);
		}
		public void Fade(bool fadeOut, float fadeDuration, EasingFunction.Ease easeType, bool ignoreTimeScale = true)
		{
			TweenFade(fadeOut, fadeDuration, easeType, ignoreTimeScale);
		}

		#endregion

		#region Fade In

		public void FadeIn()
		{
			FadeIn(_fadeAnimationTime, _fadeEaseType);
		}
		public void FadeIn(bool isInstant)
		{
			FadeIn((isInstant) ? 0 : _fadeAnimationTime, _fadeEaseType);
		}
		public void FadeIn(float fadeDuration, EasingFunction.Ease easeType, bool ignoreTimeScale = true)
		{
			TweenFade(false, fadeDuration, easeType, ignoreTimeScale);
		}

		#endregion

		#region Fade Out

		public void FadeOut()
		{
			FadeOut(_fadeAnimationTime, _fadeEaseType);
		}
		public void FadeOut(bool isInstant)
		{
			FadeOut((isInstant) ? 0 : _fadeAnimationTime, _fadeEaseType);
		}
		public void FadeOut(float fadeDuration, EasingFunction.Ease easeType, bool ignoreTimeScale = true)
		{
			TweenFade(true, fadeDuration, easeType, ignoreTimeScale);
		}

		#endregion

		#endregion

		#region Fade Tween

		protected void TweenFade(bool fadeOut, float tweenTime, EasingFunction.Ease easeType, bool ignoreTimeScale)
		{
			// If we are not active and enabled, we immediately set our alpha
			if (!isActiveAndEnabled) {
				OnTweenUpdate((fadeOut) ? 0 : 1);
				return;
			}

			//Max our time to be more than 0
			tweenTime = Mathf.Max(tweenTime, 0);
			targetValue = (fadeOut) ? 0 : 1;

			//Start our tween
			var animTween = new EaseTypeTween {
				duration = tweenTime,
				startFloat = currentValue,
				endFloat = targetValue,
				ignoreTimeScale = true
			};

			animTween.SetEasingFunc(easeType);
			animTween.AddOnChangedCallback(OnTweenUpdate);
			animTween.AddOnCompleteCallback(OnTweenComplete);
			animCurveTweenRunner.StartTween(animTween);

			_isTweening = true;
		}

		protected virtual void OnTweenUpdate(float alphaAmnt)
		{
		}

		protected virtual void OnTweenComplete()
		{
			OnTweenUpdate(targetValue);

			_isTweening = false;
		}

		#endregion

	}

}
