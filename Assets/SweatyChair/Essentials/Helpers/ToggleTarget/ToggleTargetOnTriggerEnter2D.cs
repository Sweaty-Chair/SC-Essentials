using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Toggles target object on trigger enter 2D.
	/// </summary>
	public class ToggleTargetOnTriggerEnter2D : MonoBehaviour
	{

		[Tooltip("Match the object tag only, leave empty if no need.")]
		[SerializeField] private string _tag;
		[Tooltip("Match the object name only, leave empty if no need.")]
		[SerializeField] private string _objectName;
		[Tooltip("The target object to toggle.")]
		[SerializeField] private GameObject _target;
		[Tooltip("Toggle on or off, default is to turn on.")]
		[SerializeField] private bool _toggle = true;
		[Tooltip("Toggle in a delay.")]
		[SerializeField] private float _delay = 0;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!string.IsNullOrEmpty(_tag) && !collision.CompareTag(_tag))
				return;
			if (!string.IsNullOrEmpty(_objectName) && collision.name != _objectName)
				return;
			Invoke("ToogleTarget", _delay);
		}

		private void ToogleTarget()
		{
			_target.SetActive(_toggle);
		}

	}

}