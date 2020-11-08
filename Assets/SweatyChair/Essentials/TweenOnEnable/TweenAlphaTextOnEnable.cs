using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TweenAlphaTextOnEnable : MonoBehaviour
{

	[SerializeField] private float _startAlpha = 0;
	[Range(0, float.MaxValue)]
	[SerializeField] private float _duration = 0.3f;
	[SerializeField] private float _delay = 0;
	[SerializeField] private LeanTweenType _tweenType = LeanTweenType.linear;
	[SerializeField] private LeanTweenType _loopType = LeanTweenType.once;
	[SerializeField] private bool _ignoreTimeScale = false;
	[SerializeField] private bool _disableOnComplete = false;

	private Text _text;
	private float _initAlpha;

	// Reference to our current tween id. DO NOT KEEP A REFERENCE TO THE LTDESCR BECAUSE IT IS POOLED AND MAY NOT HAVE THE SAME DATA WHEN WE CALL CANCEL
	private int _currentTweenId = -1;

	private void Awake()
	{
		_text = GetComponent<Text>();
		_initAlpha = _text.color.a;
	}

	[ContextMenu("Execute")]
	private void OnEnable()
	{
		Color newColor = _text.color;
		newColor.a = _startAlpha;
		_text.color = newColor;
		_currentTweenId = LeanTween.alphaText(GetComponent<RectTransform>(), _initAlpha, _duration)
				 .setDelay(_delay)
				 .setEase(_tweenType)
				 .setLoopType(_loopType)
				 .setIgnoreTimeScale(_ignoreTimeScale)
				 .setOnComplete(OnTweenComplete)
				 .uniqueId;
	}

	private void OnDisable()
	{
		if (_loopType != LeanTweenType.once)
			ForceCancelTween();
	}

	private void OnTweenComplete()
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

	public void ForceCancelTween()
	{
		// We now cancel our unique ID rather than the whole object, as doing it the other way would break other tweeners on this object
		LeanTween.cancel(_currentTweenId);

		// Set our alpha to the initial alpha, so we are correct next start
		Color newColor = _text.color;
		newColor.a = _startAlpha;
		_text.color = newColor;
	}

	#endregion

	#endregion

}