using UnityEngine;
using UnityEngine.Events;
using SweatyChair.StateManagement;

namespace SweatyChair
{

	public static class GameSpeedManager
	{

		public const int SUBSCRIBED_MAX_SPEED = 4;
		public const int REGULAR_MAX_SPEED = 2;
		public const float GAME_SPEED_MULTIPLIER = 2f;
		public const int DEFAULT_GAME_SPEED = 1;

		public static event UnityAction<float> gameSpeedChangedEvent;

		private static float _speedLevel = DEFAULT_GAME_SPEED;

		public static float maxGameSpeed
		{
			get { return REGULAR_MAX_SPEED; }
		}

		public static void ResetGameSpeed()
		{
			SetSpeed(DEFAULT_GAME_SPEED);
		}

		public static void CycleSpeed()
		{
			if (_speedLevel >= maxGameSpeed)
				SetSpeed(DEFAULT_GAME_SPEED);
			else
				SetSpeed(_speedLevel *= GAME_SPEED_MULTIPLIER);
		}

		private static void SetSpeed(float newSpeed)
		{
			//Debug.LogFormat("GameSpeedManager:SetSpeed - newSpeed={0}", newSpeed);
			_speedLevel = newSpeed;
			Time.timeScale = _speedLevel;
			if (gameSpeedChangedEvent != null)
				gameSpeedChangedEvent(_speedLevel);
		}

		public static void SetToCurrentSpeed()
		{
			SetSpeed(_speedLevel);
		}

		public static void Pause()
		{
			//Debug.Log("GameSpeedManager:PauseGame");
			Time.timeScale = 0;
		}

		public static void Resume()
		{
			//Debug.LogFormat("GameSpeedManager:ResumeGame - StateManager.Get()={0}", StateManager.Get());
			if (StateManager.Compare(State.Game))
				SetToCurrentSpeed();
			else
				Time.timeScale = 1;
		}

	}

}