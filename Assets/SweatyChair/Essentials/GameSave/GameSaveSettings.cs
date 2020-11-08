using UnityEngine;

namespace SweatyChair
{

	[CreateAssetMenu(fileName = "GameSaveSettings", menuName = "Sweaty Chair/Settings/GameSave")]
	public class GameSaveSettings : ScriptableObjectSingleton<GameSaveSettings>
	{

		public ES3.EncryptionType encryptionType = ES3.EncryptionType.AES;
		public string password = "T8y3m_4#!BYt#8WZ";

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Sweaty Chair/Settings/GameSave")]
		private static void OpenSettings()
		{
			current.OpenAssetsFile();
		}

#endif

	}

}