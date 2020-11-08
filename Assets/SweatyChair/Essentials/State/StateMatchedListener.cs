using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair.StateManagement
{

	[System.Serializable]
	public class StateMatchedEvent : UnityEvent<bool>
	{
	}

	/// <summary>
	/// Fire state matched event every time state changed.
	/// </summary>
	public class StateMatchedListener : MonoBehaviour
	{

		#region Variables

		[Header("Settings")]
		[SerializeField] private StateMatchedEvent _onStateMatched;
		[SerializeField] State _state;

		#endregion

		#region OnEnable / OnDisable

		public void OnEnable()
		{
			StateManager.stateChanged += OnStateChanged;
		}

		public void OnDisable()
		{
			StateManager.stateChanged -= OnStateChanged;
		}

		#endregion

		#region OnStateChange

		public void OnStateChanged(State state)
		{
			_onStateMatched?.Invoke(state == _state);
		}

		#endregion

	}

}