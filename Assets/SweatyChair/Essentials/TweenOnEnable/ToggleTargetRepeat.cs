using UnityEngine;

public class ToggleTargetRepeat : MonoBehaviour
{

	public float delay;
	public float onDuration;
	public float offDuration;

	public GameObject target;

	private void Start()
	{
		Invoke("ToggleOn", delay);
	}

	private void ToggleOn()
	{
		target.SetActive(true);
		Invoke("ToggleOff", onDuration);
	}

	private void ToggleOff()
	{
		target.SetActive(false);
		Invoke("ToggleOn", offDuration);
	}

}