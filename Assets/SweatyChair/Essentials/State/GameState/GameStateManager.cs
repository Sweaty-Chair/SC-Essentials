using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace SweatyChair.StateManagement
{

	public static class GameStateManager
	{

		public static event UnityAction<GameState> gameStateChanged;

		private const State ManagingState = State.Game;

		private static GameState _lastGameState = GameState.None;
		private static GameState _currentGameState = GameState.None;

		public static void AddChangeListener(UnityAction<GameState> callback)
		{
			gameStateChanged += callback;
		}

		public static void RemoveChangeListener(UnityAction<GameState> callback)
		{
			gameStateChanged -= callback;
		}

		public static void Set(GameState newState)
		{
			if (_currentGameState == newState)
				return;

			if (StateSettings.current.debugMode)
				Debug.LogFormat("GameStateManager:Set({0})", newState);

			_currentGameState = newState;

			gameStateChanged?.Invoke(_currentGameState);
		}

		public static void SetAsLast()
		{
			Set(_lastGameState);
		}

		private static IEnumerator _waitAndSetIenumerator;

		public static void WaitAndSet(GameState newState, float seconds)
		{
			StopWaitAndSet();
			if (seconds > 0) {
				_waitAndSetIenumerator = WaitAndSetCoroutine(newState, seconds);
				TimeManager.Start(_waitAndSetIenumerator);
			} else {
				Set(newState);
			}
		}

		private static IEnumerator WaitAndSetCoroutine(GameState newState, float seconds)
		{
			yield return new WaitForSecondsRealtime(seconds);
			Set(newState);
		}

		public static void StopWaitAndSet()
		{
			if (_waitAndSetIenumerator != null)
				TimeManager.Stop(_waitAndSetIenumerator);
		}

		public static GameState Get()
		{
			return _currentGameState;
		}

		/// <summary>
		/// Checks whether our global state is Game as well as checking for our substate match.
		/// If just checking sub state, use "Compare()" instead.
		/// </summary>
		/// <param name="checkState"></param>
		/// <returns></returns>
		public static bool CompareWithGlobalState(GameState checkState)
		{
			return StateManager.Compare(ManagingState) && Compare(checkState);
		}

		/// <summary>
		/// Checks whether our substate matches.
		/// If needing to check our global state as well. Use "CompareWithGlobalState()" instead.
		/// </summary>
		/// <param name="checkState"></param>
		/// <returns></returns>
		public static bool Compare(GameState checkState)
		{
			return checkState == _currentGameState;
		}

		public static bool Compare(params GameState[] checkStates)
		{
			foreach (GameState state in checkStates) {
				if (state == _currentGameState)
					return true;
			}
			return false;
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/State/Print Game State", false, 200)]
		public static void PrintState()
		{
			Debug.Log(_currentGameState);
		}

		[UnityEditor.MenuItem("Debug/State/Reset Game State", false, 200)]
		public static void ResetState()
		{
			Set(GameState.None);
		}

#endif

	}

}