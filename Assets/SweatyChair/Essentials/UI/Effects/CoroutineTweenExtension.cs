using UnityEngine.Events;
using UnityEngine.UI.Extensions.Tweens;

namespace UnityEngine.UI.CoroutineTween {

	internal struct EaseTypeTween : ITweenValue
	{
		public class EaseTypeTweenCallback : UnityEvent<float> { }

		private EaseTypeTweenCallback _target;
		private UnityEvent _completeTarget;

		private float _startFloat;
		private float _endFloat;
		private EasingFunction.Function _easeFunc;

		private float _duration;
		private bool _ignoreTimeScale;

		public float startFloat 
		{
			get { return _startFloat; }
			set { _startFloat = value; }
		}

		public float endFloat {
			get { return _endFloat; }
			set { _endFloat = value; }
		}

		public float duration {
			get { return _duration; }
			set { _duration = value; }
		}

		public bool ignoreTimeScale {
			get { return _ignoreTimeScale; }
			set { _ignoreTimeScale = value; }
		}

		public void TweenValue(float floatPercentage)
		{
			if (!ValidTarget() || _easeFunc == null) { return; }

			var newValue = _easeFunc(_startFloat, _endFloat, floatPercentage);
			_target.Invoke(newValue);
		}

		public void SetEasingFunc(EasingFunction.Ease easeType)
		{
			_easeFunc = EasingFunction.GetEasingFunction(easeType);
		}

		public void AddOnChangedCallback(UnityAction<float> callback)
		{
			if (_target == null) {
				_target = new EaseTypeTweenCallback();
			}
			_target.AddListener(callback);
		}

		public void AddOnCompleteCallback(UnityAction callback)
		{
			if (_completeTarget == null) {
				_completeTarget = new UnityEvent();
			}
			_completeTarget.AddListener(callback);
		}

		public bool GetIgnoreTimescale()
		{
			return _ignoreTimeScale;
		}

		public float GetDuration()
		{
			return _duration;
		}
		public bool ValidTarget()
		{
			return _target != null;
		}

		public void Finished()
		{
			if (_completeTarget == null) { return; }
			_completeTarget.Invoke();
		}
	}

}

