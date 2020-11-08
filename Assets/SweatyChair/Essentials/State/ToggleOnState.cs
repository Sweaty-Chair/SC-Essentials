using UnityEngine;

namespace SweatyChair.StateManagement
{

	/// <summary>
	/// Toggles the current game object only for a state.
	/// </summary>
	public class ToggleOnState : MonoBehaviour
	{

		[Tooltip("State to trigger on/off")]
		[SerializeField] State _state;
		[Tooltip("Sets to active or inactive")]
		[SerializeField] bool _active;

		private void Awake()
		{
			StateManager.stateChanged += OnStateChanged;
		}

		private void OnStateChanged(State state)
		{
			if (state == _state)
				gameObject.SetActive(_active);
		}

	}

}