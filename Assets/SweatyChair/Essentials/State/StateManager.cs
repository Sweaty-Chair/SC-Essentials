using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair.StateManagement
{

	public static class StateManager
	{

		public static event UnityAction<State> stateChanged;

		public static State currentState { get; private set; } = State.None;
		public static State lastState { get; private set; } = State.None;

		public static void Set(State newState)
		{
			if (currentState == newState)
				return;

			if (StateSettings.current.debugMode)
				Debug.LogFormat("StateManager:Set({0})", newState);

			lastState = currentState;
			currentState = newState;

			stateChanged?.Invoke(currentState);
		}

		public static void SetAsLast()
		{
			Set(lastState);
		}

		public static State Get()
		{
			return currentState;
		}

		// Returns true if the state matches current state
		public static bool Compare(State checkState)
		{
			return checkState == currentState;
		}

		public static bool Compare(params State[] checkStates)
		{
			foreach (State state in checkStates) {
				if (state == currentState)
					return true;
			}
			return false;
		}

		// Returns true if the state mask includes current state
		public static bool Compare(int checkStateMask)
		{
			return (checkStateMask & (1 << (int)currentState)) != 0;
		}

		// Returns true if the state matches last state
		public static bool CompareLast(State checkState)
		{
			return checkState == lastState;
		}

		// Returns true if the state mask includes current state
		public static bool CompareLast(int checkStateMask)
		{
			return (checkStateMask & (1 << (int)lastState)) != 0;
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/State/Print Current State", false, 100)]
		public static void PrintCurrentState()
		{
			Debug.Log(currentState);
		}

		[UnityEditor.MenuItem("Debug/State/Print Last State", false, 100)]
		public static void PrintLastState()
		{
			Debug.Log(lastState);
		}

		[UnityEditor.MenuItem("Debug/State/Reset State", false, 100)]
		public static void ResetState()
		{
			Set(State.None);
		}

#endif

	}

}