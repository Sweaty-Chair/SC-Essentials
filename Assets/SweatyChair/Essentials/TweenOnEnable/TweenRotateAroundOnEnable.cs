using UnityEngine;

namespace SweatyChair
{

	public class TweenRotateAroundOnEnable : TweenOnEnable
	{

		[SerializeField] private Vector3 _rotateUp;
		[SerializeField] private float _degree;

        private Vector3 _initEulerAngles;

		private void Awake()
		{
			_initEulerAngles = transform.localEulerAngles;
		}

		protected override void DoTween()
		{
			_currentTweenId = LeanTween.rotateAroundLocal(gameObject, _rotateUp, _degree, _duration)
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