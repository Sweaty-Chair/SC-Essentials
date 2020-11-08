#if CROSS_PLATFORM_INPUT

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SweatyChair.InputSystem
{
	public class CallEventOnButtonDown : MonoBehaviour

	{
		#region Class

		[System.Serializable]
		private class ButtonEvent : UnityEvent<bool> { }

		#endregion

		#region Event

		[Header("Settings")]
		[SerializeField] private string _listeningButton;

		[Header("Events")]
		[FormerlySerializedAs("buttonChangeEvent")]
		[SerializeField] private ButtonEvent _buttonChanged;
		[Space()]
		[FormerlySerializedAs("inverseButtonChangeEvent")]
		[SerializeField] private ButtonEvent _inverseButtonChanged;

		#endregion

		#region OnEnable / OnDisable

		private void OnEnable()
		{
			InputManager.AddListenerToButtonValue(_listeningButton, OnAxisChange);
		}

		private void OnDisable()
		{
			InputManager.RemoveListenerFromButtonValue(_listeningButton, OnAxisChange);
		}

		#endregion

		#region Event Callbacks

		private void OnAxisChange(bool buttonValue)
		{
			_buttonChanged?.Invoke(buttonValue);
			_inverseButtonChanged?.Invoke(!buttonValue);
		}

		#endregion
	}

}

#endif