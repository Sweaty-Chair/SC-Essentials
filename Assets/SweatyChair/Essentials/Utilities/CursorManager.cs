using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SweatyChair.StateManagement;

namespace SweatyChair
{

	/// <summary>
	/// Manager to simplify cursor state setting. Taking into account fullscreen type and other factors.
	/// </summary>
	public static class CursorManager
	{

		#region Variables

		public static event UnityAction<CursorLockMode> cursorLockModeChangeEvent;
		public static event UnityAction<bool> cursorVisibleChangeEvent;

		#region Cursor Locked States

		private static List<State> LOCKED_CURSOR_STATES = new List<State>() { };

		private static List<GameState> LOCKED_CURSOR_GAME_STATES = new List<GameState>() { };

		#endregion

		#endregion

		#region Constructor

		static CursorManager()
		{
			StateManager.stateChanged += OnStateChange;
			GameStateManager.gameStateChanged += OnGameStateChange;
		}

		#endregion

		#region State Change

		private static void OnStateChange(State state)
		{
			LockCursor(LOCKED_CURSOR_STATES.Contains(state));
		}

		private static void OnGameStateChange(GameState state)
		{
			if (StateManager.Compare(State.Game))
				LockCursor(LOCKED_CURSOR_GAME_STATES.Contains(state));
		}

		#endregion

		#region Set Cursor

		/// <summary>
		/// Sets both the lock mode and the visible state
		/// </summary>
		/// <param name="lockMode"></param>
		/// <param name="isVisible"></param>
		public static void SetCursor(CursorLockMode lockMode, bool isVisible)
		{
			SetCursorMode(lockMode);
			SetCursorVisible(isVisible);
		}

		/// <summary>
		/// Sets the cursor mode
		/// </summary>
		/// <param name="lockMode">Whether the cursor is 'Locked', 'Confined' or 'Free'</param>
		public static void SetCursorMode(CursorLockMode lockMode)
		{
			if (lockMode != Cursor.lockState) {                 //If our lock mode is different
				Cursor.lockState = lockMode;                    //Change the lock mode
				cursorLockModeChangeEvent?.Invoke(lockMode);    //Notify subscribers of the change
			}
		}

		/// <summary>
		/// Sets the Visibility of the users cursor
		/// </summary>
		/// <param name="isVisible">Whether the cursor will be visible or not</param>
		public static void SetCursorVisible(bool isVisible)
		{
			if (isVisible != Cursor.visible) {                  //If our cursor will change visible state
				Cursor.visible = isVisible;                     //Set our cursor to the state
				cursorVisibleChangeEvent?.Invoke(isVisible);    //Notify subscribers
			}
		}

		#endregion

		#region GetCursor

		public static bool IsCursorVisible()
		{
			return Cursor.visible;
		}

		public static bool IsCursorLocked()
		{
			return Cursor.lockState != CursorLockMode.None; //Our cursor is locked if it is not equal to none
		}

		public static CursorLockMode GetCursorLockMode()
		{
			return Cursor.lockState;
		}

		#endregion

		#region LockCursor

		/// <summary>
		/// Updates the cursor lock mode depending on the type of fullscreen the user is running under.
		/// Helps automate confining cursor in exclusive fullscreen while doing no locking when user is running borderless fullscreen,etc
		/// </summary>
		/// <param name="locked"></param>
		public static void LockCursor(bool locked)
		{
			//Debug.LogFormat("CursorManager:LockCursor({0})", locked);
#if UNITY_EDITOR
			SetCursorMode((locked) ? CursorLockMode.Locked : CursorLockMode.None);
			SetCursorVisible(!locked);
#else
			if ((Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)) {
				SetCursorMode((locked) ? CursorLockMode.Locked : CursorLockMode.Confined);  //If we are in exclusive fullscreen, we want to confine the cursor to the window, if we unlock the cursor
				SetCursorVisible(!locked);
			} else {
				SetCursorMode((locked) ? CursorLockMode.Locked : CursorLockMode.None);      //If we are in any other fullscreen or windowed mode, we want to allow the user to move the mouse outside of the window if unlocked
				SetCursorVisible(!locked);
			}
#endif
		}

		#endregion

		#region Debug Utilities

#if UNITY_EDITOR

        [UnityEditor.MenuItem("Debug/Cursor/Force Unlock Cursor")]
        private static void FORCE_UNLOCK_CURSOR_DEBUG()
        {
            LockCursor(false);
        }

        [UnityEditor.MenuItem("Debug/Cursor/Force Lock Cursor")]
        private static void FORCE_LOCK_CURSOR_DEBUG()
        {
            LockCursor(true);
        }

#endif

		#endregion

	}

}
