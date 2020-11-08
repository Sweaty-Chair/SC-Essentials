using UnityEngine;

namespace SweatyChair.StateManagement
{

	/// <summary>
	/// Activate target GameObject on matched state.
	/// </summary>
	public class ActivateOnState : MonoBehaviour
	{

		[Tooltip("State to activate")]
		[SerializeField] private State _state;
		[Tooltip("The target game object")]
		[SerializeField] private GameObject _targetGO;

		private void Awake()
		{
			StateManager.stateChanged += OnStateChanged;
		}

		private void OnDestroy()
		{
			StateManager.stateChanged -= OnStateChanged;
		}

		private void OnStateChanged(State state)
		{
			if (state == _state)
				_targetGO.SetActive(true);
		}

	}

}