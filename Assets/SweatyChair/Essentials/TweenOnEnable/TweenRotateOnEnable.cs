using UnityEngine;

namespace SweatyChair
{

	public class TweenRotateOnEnable : TweenOnEnable
	{

		[SerializeField] private Vector3 _startOffset;

		private Vector3 _initEulerAngles;

		private void Awake()
		{
			_initEulerAngles = transform.localEulerAngles;
		}

		protected override void DoTween()
		{
			transform.localEulerAngles = _initEulerAngles + _startOffset;
			_currentTweenId = LeanTween.rotateLocal(gameObject, _initEulerAngles, _duration)
				.setDelay(_delay)
				.setEase(_tweenType)
				.setLoopType(_loopType)
				.setIgnoreTimeScale(_ignoreTimeScale)
				.setOnComplete(OnTweenComplete)
				.uniqueId;
		}

		public override void ForceCancelTween()
		{
			base.ForceCancelTween();
			// Set our position to the final rotation, so we are correct next start
			transform.localEulerAngles = _initEulerAngles;
		}
	}

}