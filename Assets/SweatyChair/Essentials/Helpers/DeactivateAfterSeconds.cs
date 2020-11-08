using UnityEngine;
using System.Collections;

public class DeactivateAfterSeconds : MonoBehaviour
{

	[SerializeField] private float _waitSeconds;
	[SerializeField] private bool _useTimeScale = false;

	void OnEnable()
	{
		StartCoroutine(DeactiavteRoutine());
	}

	private IEnumerator DeactiavteRoutine()
	{
		if (_useTimeScale)
			yield return new WaitForSeconds(_waitSeconds);
		else
			yield return new WaitForSecondsRealtime(_waitSeconds);
		gameObject.SetActive(false);
	}

}