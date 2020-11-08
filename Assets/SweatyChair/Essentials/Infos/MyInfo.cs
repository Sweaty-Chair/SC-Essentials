using UnityEngine;

namespace SweatyChair
{

	public static class MyInfo
	{

		#region Player Name

		private const string PREFS_PLAYER_NAME = "PlayerName";

		public static bool hasPlayerName => PlayerPrefs.HasKey(PREFS_PLAYER_NAME);

		public static string playerName {
			get {
				if (PlayerPrefs.HasKey(PREFS_PLAYER_NAME) && !string.IsNullOrEmpty(PlayerPrefs.GetString(PREFS_PLAYER_NAME))) // Use local saved name first, if any
					return PlayerPrefs.GetString(PREFS_PLAYER_NAME);
#if EASY_MOBILE
				// True if my name match the game server username
				if (EasyMobile.GameServices.IsInitialized() && !string.IsNullOrEmpty(EasyMobile.GameServices.LocalUser.userName))
					return EasyMobile.GameServices.LocalUser.userName;
#endif
				return RandomUtils.GetRandomName(12); // Nothing can use? just a random name
			}
		}

		public static void SetMyPlayerName(string pName)
		{
			PlayerPrefs.SetString(PREFS_PLAYER_NAME, pName);
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/MyInfo/Print Player Name")]
		private static void PrintPlayerName()
		{
			if (Application.isPlaying)
				DebugUtils.Log(playerName, "playerName");
			else
				DebugUtils.LogPlayerPrefs<string>(PREFS_PLAYER_NAME);
		}

#endif

		#endregion

	}

}