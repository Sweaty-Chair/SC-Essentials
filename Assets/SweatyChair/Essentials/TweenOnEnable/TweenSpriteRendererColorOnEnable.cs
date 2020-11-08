using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class TweenSpriteRendererColorOnEnable : MonoBehaviour
{

	[Range(0, float.MaxValue)]
	[SerializeField] private float _duration = 0.3f;
	[SerializeField] private float _delay = 0;
	[SerializeField] private LeanTweenType _tweenType = LeanTweenType.linear;
	[SerializeField] private LeanTweenType _loopType = LeanTweenType.once;
	[SerializeField] private bool _ignoreTimeScale = false;

	[SerializeField] private Color _startColor;
	[SerializeField] private Color _finalColor;

	void OnEnable()
	{
		GetComponent<SpriteRenderer>().color = _startColor;
		LeanTween.color(gameObject, _finalColor, _duration)
			.setDelay(_delay)
			.setEase(_tweenType)
			.setLoopType(_loopType)
			.setIgnoreTimeScale(_ignoreTimeScale);
	}

}