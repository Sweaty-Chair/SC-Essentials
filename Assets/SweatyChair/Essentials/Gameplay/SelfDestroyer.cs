using System.Collections;
using UnityEngine;

public class SelfDestroyer : MonoBehaviour
{

	[SerializeField] private float _delay = 1;
	[SerializeField] private bool _ignoreTimeScale = false;

	private IEnumerator Start()
	{
		if (_ignoreTimeScale)
			yield return new WaitForSecondsRealtime(_delay);
		else
			yield return new WaitForSeconds(_delay);
		Destroy(gameObject);
	}

	public void SetDelay(float delay)
	{
		_delay = delay;
	}

}