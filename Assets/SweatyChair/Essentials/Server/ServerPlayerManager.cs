using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace SweatyChair
{

	public static class ServerPlayerManager
	{

		public const int MAX_CHR_PLAYER_NAME = 12;
		public const int MIN_CHR_PLAYER_NAME = 3;

		private const string PREFS_PLAYER_ID = "ServerPlayerId";
		private const string PREFS_PLAYER_NAME = "ServerPlayerName";
		private const string PREFS_POSTED_GAME_SERVICES_PLAYER_INFO = "PostedPlayGameCenterId";
		private const string PREFS_POSTED_FACEBOOK_INFO = "PostedFacebookId";

		public static event UnityAction registerSucceed;
		public static event UnityAction<string> nameChangeEvent;

		public static int playerId { get; private set; }

		public static string playerName { get; private set; }

		public static bool isRegistered => playerId > 0;

		public static bool hasPlayerName => !string.IsNullOrEmpty(playerName);

		static ServerPlayerManager()
		{
			playerId = PlayerPrefs.GetInt(PREFS_PLAYER_ID);
			playerName = PlayerPrefs.GetString(PREFS_PLAYER_NAME);

#if EASY_MOBILE
			EasyMobile.GameServices.UserLoginSucceeded += CheckGameServicesUpdatedPlayer;
#endif
#if FACEBOOK
			GameFacebookManager.loginStatusUpdatedEvent += CheckFacebookUpdatedPlayer;
#endif

			if (playerId == 0)
				PostNewPlayer();
		}

		private static void SetPlayerId(int id)
		{
			playerId = id;
			PlayerPrefs.SetInt(PREFS_PLAYER_ID, playerId);
		}

		private static void SetPlayerName(string name)
		{
			playerName = name;
			PlayerPrefs.SetString(PREFS_PLAYER_NAME, playerName);
		}

		private static void PostNewPlayer()
		{
			var dict = new Dictionary<string, string> { { "udid", UnityEngine.SystemInfo.deviceUniqueIdentifier } };
			dict = AddAccounts(dict);

			// Game info
			dict["game_id"] = "{gameId}";
#if UNITY_IOS
			dict["channel"] = "1";
#elif UNITY_ANDROID
			dict["channel"] = "2";
#if TAPTAP
			dict["channel"] = "11";
#endif
#endif
			dict["version"] = StringUtils.ParseToInt(Application.version).ToString();

			ServerManager.Post(
				"players",
				dict,
				OnPostNewPlayerComplete
			);
		}

		private static Dictionary<string, string> AddAccounts(Dictionary<string, string> dict)
		{
#if EASY_MOBILE
			if (EasyMobile.GameServices.IsInitialized()) {
#if UNITY_IOS
				dict["gamecenter_id"] = EasyMobile.GameServices.LocalUser.id;
#elif UNITY_ANDROID
				dict["playgames_id"] = EasyMobile.GameServices.LocalUser.id;
				SetPostedGameServicesId();
#endif
				if (string.IsNullOrEmpty(playerName)) {
					SetPlayerName(EasyMobile.GameServices.LocalUser.userName);
					dict["name"] = playerName;
				}
			}
#endif
#if FACEBOOK
			if (GameFacebookManager.isLoggedIn) {
			dict["facebook_id"] = GameFacebookState.myId;
			SetPostedFacebookUpdatedPlayer();
			if (string.IsNullOrEmpty(playerName)) {
			SetPlayerName(GameFacebookState.myName);
			dict["name"] = playerName;
			}
			}
#endif
			return dict;
		}

		private static void OnPostNewPlayerComplete(Hashtable ht)
		{
			SetPlayerId(System.Convert.ToInt32(ht["id"]));
			Debug.LogFormat("ServerPlayerManager:OnPostNewPlayerComplete - Successfully registered as playerId={0}", playerId);
			registerSucceed?.Invoke();
		}

		private static void CheckGameServicesUpdatedPlayer()
		{
			if (PlayerPrefs.GetInt(PREFS_POSTED_GAME_SERVICES_PLAYER_INFO) != 1)
				PostUpdatedPlayer();
		}

		private static void SetPostedGameServicesId()
		{
			PlayerPrefs.SetInt(PREFS_POSTED_GAME_SERVICES_PLAYER_INFO, 1);
		}

		private static void CheckFacebookUpdatedPlayer()
		{
#if FACEBOOK
			if (!GameFacebookManager.isLoggedIn)
				return;
#endif
			if (PlayerPrefs.GetInt(PREFS_POSTED_FACEBOOK_INFO) == 1)
				return;
			PostUpdatedPlayer();
		}

		private static void SetPostedFacebookUpdatedPlayer()
		{
			PlayerPrefs.SetInt(PREFS_POSTED_FACEBOOK_INFO, 1);
		}

		private static void PostUpdatedPlayer()
		{
			var dict = new Dictionary<string, string>();
			dict = AddAccounts(dict);
			ServerManager.Post(
				string.Format("players/{0}", playerId),
				dict
			);
		}

		public static void ChangePlayerName(string name)
		{
			SetPlayerName(name);
			var dict = new Dictionary<string, string> { { "name", playerName } };
			ServerManager.Post(
				string.Format("players/{0}", playerId),
				dict
			);

			nameChangeEvent?.Invoke(name);
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Server/Player/Print Parameters")]
		private static void PrintParameters()
		{
			if (Application.isPlaying) {
				DebugUtils.Log(playerId, "playerId");
				DebugUtils.Log(playerName, "playerName");
			} else {
				DebugUtils.LogPlayerPrefs<int>(PREFS_PLAYER_ID);
				DebugUtils.LogPlayerPrefs<string>(PREFS_PLAYER_NAME);
			}
		}

		[UnityEditor.MenuItem("Debug/Server/Player/Reset PlayerPrefs")]
		private static void ResetPlayerPrefs()
		{
			PlayerPrefs.DeleteKey(PREFS_PLAYER_ID);
			PlayerPrefs.DeleteKey(PREFS_POSTED_GAME_SERVICES_PLAYER_INFO);
			PlayerPrefs.DeleteKey(PREFS_POSTED_FACEBOOK_INFO);
			Debug.Log("ServerPlayerManager:ResetPlayerPrefs - Successfully reset");
		}

#endif

	}

}