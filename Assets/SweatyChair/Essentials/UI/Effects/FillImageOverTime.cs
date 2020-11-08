using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class FillImageOverTime : MonoBehaviour
{
	public float fillTime = 1f;
	[SerializeField][Range(0f, 1f)]
	private float _startFill = 0f;
	[SerializeField][Range(0f, 1f)]
	private float _endFill = 1f;
	public bool isPaused { get { return _isPaused; } set { _isPaused = value; } }
	private bool _isPaused = false;

	private Image _image;
	private UnityAction _callback;

	void Awake()
	{
		if (!_image)
			_image = GetComponent<Image> ();
	}

	public void StartFill()
	{
		StartCoroutine ("FillOverTime", fillTime);
	}

	public void StartFill(float fillTime)
	{
		StartCoroutine ("FillOverTime", fillTime);
	}

	public void StartFill(float fillTime, UnityAction callback)
	{
		_callback = callback;
		StartCoroutine ("FillOverTime", fillTime);
	}

	public void StopFill()
	{
		StopCoroutine ("FillOverTime");
	}

	public void Pause()
	{
		_isPaused = true;
	}

	public void Resume()
	{
		_isPaused = false;
	}

	private IEnumerator FillOverTime(float duration)
	{
		float elapsedTime = 0f;
		_image.fillAmount = _startFill;

		while (elapsedTime < duration) {
			while (_isPaused)
				yield return null;
			
			_image.fillAmount = Mathf.Clamp01(Mathf.Lerp (_startFill, _endFill, elapsedTime / duration));
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		_image.fillAmount = _endFill;
		if (_callback != null)
			_callback ();
		yield return null;
	}
}
