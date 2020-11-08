using UnityEngine;
using UnityEngine.Serialization;

namespace SweatyChair
{

	public class TweenScaleOnEnable : TweenOnEnable
	{

		[SerializeField] private Vector3 _startScaler = Vector3.one;

		[Tooltip("Invert the tween, scale to the scaler instead")]
		[SerializeField] private bool _isScaleTo;
		[Tooltip("Reset the scale on tween complete")]
		[FormerlySerializedAs("_resetOnFinish")]
		[SerializeField] private bool _resetOnComplete;
		[Tooltip("Should the initial cache of the local scale be reinitialise on OnEnable?")]
		[SerializeField] private bool _initOnEnable;

		private Vector3 _initLocalScale;

		private void Awake()
		{
			_initLocalScale = transform.localScale;
		}

		protected override void DoTween()
		{
			if (_initOnEnable)
				_initLocalScale = transform.localScale;
			if (!_isScaleTo)
				transform.localScale = Vector3.Scale(_initLocalScale, _startScaler);

			Vector3 targetSize = _isScaleTo ? _startScaler : _initLocalScale;

			_currentTweenId = LeanTween.scale(gameObject, targetSize, _duration)
					 .setEase(_tweenType)
					 .setLoopType(_loopType)
					 .setIgnoreTimeScale(_ignoreTimeScale)
					 .setOnComplete(OnTweenComplete)
					 .setDelay(_delay)
					 .uniqueId;
		}

		protected override void OnTweenComplete()
		{
			base.OnTweenComplete();
			if (_resetOnComplete)
				transform.localScale = _initLocalScale;
		}

		public override void ForceCancelTween()
		{
			base.ForceCancelTween();
			// Set our position to the final rotation, so we are correct next start
			transform.localScale = _initLocalScale;
		}

	}

}