using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TweenAnchorPosOnEnable : MonoBehaviour
{
    [SerializeField] private bool _ignoreTimeScale = false;
	[SerializeField] private Vector2 _startOffset = Vector2.zero;
	[Range(0, float.MaxValue)]
	[SerializeField] private float _speed = 0.3f;
	[SerializeField] private float _delay = 0;

	private RectTransform _rectTransform;
	private Vector2 _finalPosition;
	private IEnumerator _tweenMoveCoroutine;

	void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_finalPosition = _rectTransform.anchoredPosition;
	}

	void OnEnable()
	{
		if (_tweenMoveCoroutine != null)
			StopCoroutine(_tweenMoveCoroutine);
		_tweenMoveCoroutine = TweenMoveCoroutine();
		StartCoroutine(_tweenMoveCoroutine);
	}

	private IEnumerator TweenMoveCoroutine()
	{
		_rectTransform.anchoredPosition = _finalPosition + _startOffset;
		yield return new WaitForSecondsRealtime(_delay);
		while (_rectTransform.anchoredPosition != _finalPosition) {
			float horizontal = _rectTransform.anchoredPosition.x;
			float vertical = _rectTransform.anchoredPosition.y;
			horizontal = Mathf.MoveTowards(horizontal, _finalPosition.x, (_finalPosition.x - _startOffset.x) / _speed * (_ignoreTimeScale? Time.unscaledDeltaTime : Time.deltaTime));
			vertical = Mathf.MoveTowards(vertical, _finalPosition.y, (_finalPosition.y - _startOffset.y) / _speed * (_ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime));
			_rectTransform.anchoredPosition = new Vector2(horizontal, vertical);
			yield return null;
		}
	}

}