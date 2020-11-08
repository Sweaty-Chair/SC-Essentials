using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Tween move the object from/to a position on enable.
	/// </summary>
	public class TweenMoveOnEnable : TweenOnEnable
	{

		[Tooltip("Offset from original position BEFOE the tween, used for moving object TO its original position.")]
		[SerializeField] private Vector3 _startOffset = Vector3.zero;
		[Tooltip("Offset from original position AFTER the tween, used for moving object FROM its original position.")]
		[SerializeField] private Vector3 _endOffset = Vector3.zero;

		private Vector3 _initLocalPosition;

		private void Awake()
		{
			_initLocalPosition = transform.localPosition;
		}

		protected override void DoTween()
		{
			transform.localPosition = _initLocalPosition + _startOffset;
			_currentTweenId = LeanTween.moveLocal(gameObject, _initLocalPosition + _endOffset, _duration)
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
			// Set position back to the original position
			transform.localPosition = _initLocalPosition;
		}

	}

}