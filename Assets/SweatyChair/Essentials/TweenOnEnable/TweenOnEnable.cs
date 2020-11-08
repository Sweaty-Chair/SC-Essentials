using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A base class for tween, override this for different tween behaviours.
	/// </summary>
	public abstract class TweenOnEnable : MonoBehaviour
	{

		[Range(0, 30)]
		[SerializeField] protected float _duration = 0.3f;
		[SerializeField] protected float _delay = 0;
		[SerializeField] protected LeanTweenType _tweenType = LeanTweenType.linear;
		[SerializeField] protected LeanTweenType _loopType = LeanTweenType.once;
		[SerializeField] protected bool _ignoreTimeScale = false;
		[Tooltip("Disable the gameobject on tween complete")]
		[SerializeField] protected bool _disableOnComplete = false;

		// Reference to our current tween id. DO NOT KEEP A REFERENCE TO THE LTDESCR BECAUSE IT IS POOLED AND MAY NOT HAVE THE SAME DATA WHEN WE CALL CANCEL
		protected int _currentTweenId = -1;

		[ContextMenu("Execute")]
		private void OnEnable()
		{
			DoTween();
		}

		protected abstract void DoTween();

		protected virtual void OnDisable()
		{
			if (_loopType != LeanTweenType.once)
				ForceCancelTween();
		}

		protected virtual void OnTweenComplete()
		{
			if (_disableOnComplete)
				gameObject.SetActive(false);
		}

		#region Utility

		#region Set Values

		/// <summary>
		/// Sets tween delay. Does not affect the current tween if it is running. Disable then re-enable the object for the changes to apply
		/// </summary>
		/// <param name="newDelay"></param>
		public void SetTweenDelay(float newDelay)
		{
			_delay = newDelay;
		}

		/// <summary>
		/// Sets tween duration. Does not affect the current tween if it is running. Disable then re-enable the object for the changes to apply
		/// </summary>
		public void SetTweenDuration(float newDuration)
		{
			_duration = newDuration;
		}

		#endregion

		#region Cancel Tweens

		public void RefreshTween()
		{
			enabled = false;
			enabled = true;
		}

		public virtual void ForceCancelTween()
		{
			// We now cancel our unique ID rather than the whole object, as doing it the other way would break other tweeners on this object
			LeanTween.cancel(_currentTweenId);
		}

		#endregion

		#endregion

	}

}