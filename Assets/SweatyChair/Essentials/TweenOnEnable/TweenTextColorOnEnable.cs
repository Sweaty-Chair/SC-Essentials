using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TweenTextColourOnEnable : MonoBehaviour
{

	[Range(0, float.MaxValue)]
	[SerializeField] private float _duration = 0.3f;
	[SerializeField] private float _delay = 0;
	[SerializeField] private int _loopTimes;
	[SerializeField] private LeanTweenType _tweenType = LeanTweenType.linear;
	[SerializeField] private LeanTweenType _loopType = LeanTweenType.once;
	[SerializeField] private bool _ignoreTimeScale = false;

	[SerializeField] private Color _startColor;
	[SerializeField] private Color _finalColor;

	void OnEnable()
	{
		GetComponent<Text>().color = _startColor;
		LeanTween.textColor(GetComponent<RectTransform>(), _finalColor, _duration)
			.setDelay(_delay)
			.setLoopType(_tweenType)
			.setLoopCount(_loopTimes);
	}

}