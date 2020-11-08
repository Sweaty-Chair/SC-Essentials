using UnityEngine;
using System.Collections.Generic;

namespace SweatyChair.StateManagement
{

	/// <summary>
	/// A script simply set the state on enable.
	/// </summary>
	public class StateInitializer : MonoBehaviour
	{

		[SerializeField] private State _state;
		[SerializeField] private List<State> _ignoredStateList;

		private void OnEnable()
		{
			if (!_ignoredStateList.Contains(StateManager.currentState))
				StateManager.Set(_state);
			Destroy(gameObject);
		}

	}

}