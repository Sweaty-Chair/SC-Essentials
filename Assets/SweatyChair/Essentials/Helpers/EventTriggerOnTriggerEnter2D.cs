using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

	/// <summary>
	/// Triggers event on trigger enter 2D.
	/// </summary>
	public class EventTriggerOnTriggerEnter2D : MonoBehaviour
	{

		[Tooltip("Match the object tag only, leave empty if no need.")]
		[SerializeField] private string _tag;
		[Tooltip("Match the object name only, leave empty if no need.")]
		[SerializeField] private string _objectName;
		[Tooltip("The event to trigger.")]
		[SerializeField] private UnityEvent _triggered;
		[Tooltip("Trigger only once, even the trigger enter fired multiple times.")]
		[SerializeField] private bool _triggerOnce = true;

		private float _lastTriggeredTime = -1;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!string.IsNullOrEmpty(_tag) && !collision.CompareTag(_tag))
				return;
			if (!string.IsNullOrEmpty(_objectName) && collision.name != _objectName)
				return;
			if (_triggerOnce && _lastTriggeredTime != -1)
				return;
			if (Time.unscaledTime - _lastTriggeredTime < 1) // Dont allow trigger with 1 second, to avoid colliders leave and enter issue
				return;
			_triggered?.Invoke();
			_lastTriggeredTime = Time.unscaledTime;
		}

	}

}