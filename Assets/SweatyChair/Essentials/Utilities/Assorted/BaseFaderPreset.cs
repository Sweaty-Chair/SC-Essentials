using UnityEngine;

namespace SweatyChair
{

	public abstract class BaseFaderPreset : ScriptableObject
	{
		#region Variables

		[Header("Easing Settings")]
		[SerializeField] protected EasingFunction.Ease _fadeEaseType = EasingFunction.Ease.EaseOutQuad;
		[SerializeField] protected float _fadeAnimationTime = 1;

		// Public getters
		public EasingFunction.Ease faseEaseType => _fadeEaseType;
		public float fadeAnimationTime => _fadeAnimationTime;

		#endregion
	}

}