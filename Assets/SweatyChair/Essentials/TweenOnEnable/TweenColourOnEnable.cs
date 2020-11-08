using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenColourOnEnable : MonoBehaviour {

	#region Variables

	[SerializeField] private Color _startColour = Color.white;

	[Range(0, float.MaxValue)]
	[SerializeField] private float _duration = 0.3f;
	[SerializeField] private float _delay = 0;
	[SerializeField] private LeanTweenType _tweenType = LeanTweenType.linear;
	[SerializeField] private LeanTweenType _loopType = LeanTweenType.once;
	[SerializeField] private bool _ignoreTimeScale = false;

	private Graphic _graphicComponent;
	private Color _originalGraphicColour;

	private int _leanTweenID;

	#endregion

	#region OnEnable / OnDisable

	private void OnEnable() {
		if (_graphicComponent == null) { _graphicComponent = GetComponent<Graphic>(); }
		if (_graphicComponent == null) { Debug.Log("Cannot tween colour on non-existent graphic component"); return; }

		_originalGraphicColour = _graphicComponent.color;

		//Set our initial colour to our start colour
		_graphicComponent.color = _startColour;

		_leanTweenID = LeanTween.color(GetComponent<RectTransform>(), _originalGraphicColour, _duration)
			.setRecursive(false)
			.setDelay(_delay)
			.setEase(_tweenType)
			.setLoopType(_loopType)
			.setIgnoreTimeScale(_ignoreTimeScale)
			.uniqueId;
	}

	private void OnDisable() {
		//Cancel our running leantween
		LeanTween.cancel(_leanTweenID);

		//Reset the colour to initial colour. (For if we delete this component)
		_graphicComponent.color = _originalGraphicColour;

	}

	#endregion

}
