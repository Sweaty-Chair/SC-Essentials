using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A setting class that save which databases to load by DatabaseManager.
	/// </summary>
	[CreateAssetMenu(fileName = "DatabaseSettings", menuName = "Sweaty Chair/Settings/Database")]
	public class DatabaseSettings : ScriptableObjectSingleton<DatabaseSettings>
	{
		public string[] databaseNames = new string[0];
		public bool debugMode;

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Sweaty Chair/Settings/Database %#d")]
		public static void OpenSettings()
		{
			current.OpenAssetsFile();
		}

#endif

	}

}