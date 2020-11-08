using UnityEngine.Events;

namespace SweatyChair
{

    /// <summary>
	/// A generic game manager that manage the game flow logic, such as start and end game.
	/// </summary>
    public static partial class GameManager
    {

		#region Game start

		public static event UnityAction gameStarted;

		public static bool isGameStarted { get; private set; }

		/// <summary>
		/// Start game, run all core logic in gameStarted callback.
		/// </summary>
		public static void StartGame()
		{
			PauseGame(false); // Just in case
			gameStarted?.Invoke();
			isGameStarted = true;
		}

		/// <summary>
		/// Set the game started boolean, used mostly for a count down to start a game and disable all other buttons/callbacks.
		/// </summary>
		public static void SetGameStarted()
		{
			isGameStarted = true;
		}

		#endregion

		#region Game pause

		public static event UnityAction<bool> gamePaused;

		public static bool isGamePaused { get; private set; }

        public static void PauseGame(bool isPaused = true)
		{
			gamePaused?.Invoke(isPaused);
			isGamePaused = isPaused;
		}

		#endregion

		#region Game restart

		public static event UnityAction gameRestarted;

		public static void RestartGame()
		{
			gameRestarted?.Invoke();
		}

		#endregion

		#region Game complete

		public static event UnityAction gameCompleted;

		public static void CompleteGame()
		{
			gameCompleted?.Invoke();
			isGameStarted = false;
		}

		#endregion

	}

}