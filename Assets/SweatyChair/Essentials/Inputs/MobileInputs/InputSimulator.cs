using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

	/// <summary>
	/// A simulator that visually simulate a button press in Editor.
	/// (i.e. This only shows how a button is clicked while using keyboard, no controlling logic at all)
	/// Author: Richard
	/// </summary>
	public class InputSimulator : MonoBehaviour
	{

		[SerializeField] private string _buttonName = "Attack";
		[SerializeField] private UnityEvent pointerClick;

		private void Awake()
		{
#if !UNITY_EDITOR && !UNITY_STANDALONE
		Destroy(this);
#endif
		}

		private void Update()
		{
			if (Input.GetButtonUp(_buttonName)) {
				if (pointerClick != null)
					pointerClick.Invoke();
			}
		}

	}

}