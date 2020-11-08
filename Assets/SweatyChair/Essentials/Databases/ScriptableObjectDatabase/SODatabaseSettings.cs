using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A setting class that save which databases to load by DatabaseManager.
	/// </summary>
	[CreateAssetMenu(fileName = "SODatabaseSettings", menuName = "Sweaty Chair/Settings/SO Database")]
	public class SODatabaseSettings : ScriptableObjectSingleton<SODatabaseSettings>
	{
		public string[] databaseNames = new string[0];
		public bool debugMode;

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Sweaty Chair/Settings/SODatabase")]
		public static void OpenSettings()
		{
			current.OpenAssetsFile();
		}

#endif

	}

}
