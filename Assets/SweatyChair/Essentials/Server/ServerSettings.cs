using UnityEngine;

namespace SweatyChair
{

	[CreateAssetMenu(fileName = "ServerSettings", menuName = "Sweaty Chair/Settings/Server")]
	public class ServerSettings : ScriptableObjectSingleton<ServerSettings>
	{

		[Tooltip("The domain url of the server.")]
		public string domain = "http://api.sweatychair.com/";
		[Tooltip("The game ID.")]
		public int gameId = 5;
		[Tooltip("Get time from server and set to UnbiasedTime.")]
		public bool getTimeFromServer;
		[Tooltip("Set to offline mode and bypass all requests sending to server.")]
		public bool offlineMode;
		[Tooltip("Debug mode for logs of requests.")]
		public bool debugMode;

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Sweaty Chair/Settings/Server")]
		private static void OpenSettings()
		{
			current.OpenAssetsFile();
		}

#endif

	}

}