using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	// A helper class that tracks a 'Return' key is input in InputField and do a callback
	[RequireComponent(typeof(InputField))]
	public class InputFieldReturnTracker : MonoBehaviour
	{

		[SerializeField] private InputField[] _checkNonEmptyInputFields;
		[SerializeField] private UnityEvent _onReturnEntered;

		private InputField _inputField;

		private void Awake()
		{
			_inputField = GetComponent<InputField>();
		}

		private void Update()
		{
			// Monitor if the esc key pressed
			if (_inputField.text.Length > 0 && (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))) {
				foreach (InputField inputField in _checkNonEmptyInputFields) {
					if (string.IsNullOrEmpty(inputField.text))
						return;
				}
				_onReturnEntered?.Invoke();
			}
		}

	}

}