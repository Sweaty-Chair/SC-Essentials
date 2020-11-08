using UnityEngine;
using SweatyChair.StateManagement;

namespace SweatyChair.UI
{

	/// <summary>
	/// A simple panel that show on specific state.
	/// </summary>
	public class SimpleStatePanel : Panel
	{

		[SerializeField] private State _state;

		protected override void OnStateChanged(State state)
		{
			Toggle(state == _state);
		}

	}

}