using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TweenCanvasGroupOnEnable : MonoBehaviour
{

	#region Variables

	[Header("Settings")]
	[Range(0, float.MaxValue)] [SerializeField] private float _duration = 0.3f;
	[SerializeField] private float _delay = 0;
	[SerializeField] private LeanTweenType _tweenType = LeanTweenType.linear;
	[SerializeField] private LeanTweenType _loopType = LeanTweenType.once;
	[SerializeField] private bool _ignoreTimeScale = false;

	[Range(0, 1)] [SerializeField] private float startAlpha;

	#endregion

	#region OnEnable / OnDisable

	private void OnEnable()
	{
		// Get our canvas group
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		// Store our final alpha value
		float finalAlpha = canvasGroup.alpha;
		// Set our current alpha to our start alpha
		canvasGroup.alpha = startAlpha;
		// Then leantween the alpha of our canvas to our desired alpha
		LeanTween.alphaCanvas(canvasGroup, finalAlpha, _duration)
			.setDelay(_delay)
			.setEase(_tweenType)
			.setLoopType(_loopType)
			.setIgnoreTimeScale(_ignoreTimeScale);
	}

	#endregion

}
