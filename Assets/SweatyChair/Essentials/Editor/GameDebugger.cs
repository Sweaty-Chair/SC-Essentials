using UnityEngine;
using UnityEditor;
using SweatyChair;

public static class GameDebugger
{

	[MenuItem("Debug/Reset Game", false, 1)]
	private static void ResetGame()
	{
		DebugUtils.CheckNotPlaying(() => {
			PlayerPrefs.DeleteAll();
			FileUtils.EmptyDirectory(Application.persistentDataPath);
			Debug.Log("All PlayerPrefs and files in Application.persistentDataPath are deleted!");
		});
	}

	[MenuItem("Debug/Persistent GameData Path/Clear", false, 2)]
	private static void ClearPersistentDataPathFolder()
	{
		FileUtils.EmptyDirectory(Application.persistentDataPath);
	}

	[MenuItem("Debug/Persistent GameData Path/Open", false, 3)]
	private static void OpenPersistentDataPathFolder()
	{
		EditorUtility.RevealInFinder(Application.persistentDataPath);
	}

	// An easy access playground for simple code testing
	[MenuItem("Debug/Test", false, 9999)]
	private static void Test()
	{
		//string tmpPath = ES3Settings.defaultSettings.path.Replace(".es3", ".tmp");
		//var tmpSetting = new ES3Settings(tmpPath, ES3Settings.defaultSettings);
		//ES3.CopyFile(ES3Settings.defaultSettings, tmpSetting);
		//ES3.Save("Timestamp", System.DateTime.Now, tmpSetting);
		//ES3.Save("UnlockedLevel", UnityEngine.Random.Range(1, 10), tmpSetting);
		//ES3.Save("Coins", UnityEngine.Random.Range(1, 999), tmpSetting);
		//ES3.Save("Gems", UnityEngine.Random.Range(1, 999), tmpSetting);

		//Debug.Log(ES3.Load<System.DateTime>("Timestamp", ES3Settings.defaultSettings));
		//Debug.Log(ES3.Load<System.DateTime>("Timestamp", tmpSetting));

		//byte[] bytes = ES3.LoadRawBytes(tmpSetting);
		//instance.CheckSavedGame(bytes);
	}

}