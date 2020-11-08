using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using SweatyChair.UI;
#if EASY_MOBILE
using EasyMobile;
#endif

namespace SweatyChair
{

	/// <summary>
	/// Loads and saves game save to game services online. Also checking the remote save is newer and prompt the user for overriding.
	/// </summary>
	public class GameSaveSync : PersistentSingleton<GameSaveSync>
	{

		/// <summary>
		/// Event fires when progress is synced from game services. This fires after game launch and most likely after all initialization, so make sure all scripts using GameSave register this event and update their variables accordingly.
		/// </summary>
		public static event UnityAction gameSaveUpdated;

#if EASY_MOBILE

		[Tooltip("The message content where a newer version of game save is found online and asking to replace.")]
		[SerializeField] string _replaceWithServerSaveContent = "Do you want to load previous progress with level {0}?";
		[Tooltip("The data key to used in above sentence.")]
		[SerializeField] string _replaceServerSaveContentKey = "Level";
		[Tooltip("Reload the scene immediate after replacing the server save.")]
		[SerializeField] bool _reloadSceneAfterReplace = true;
		[Tooltip("Debug mode and do not skip sync in Editor.")]
		[SerializeField] bool _debugMode = false;

		private SavedGame _mySavedGame;
		private bool _readSavedGameAfterOpen = true; // Only read saved game on first open

		private void Start()
		{
			GameSave.gameSaveSaved += SaveOnline;
			if (GameServices.IsInitialized()) // Already logined in, simply open saved game now
				OpenSavedGame();
			else // Open saved game after login
				GameServices.UserLoginSucceeded += OpenSavedGame;
		}

		private void OpenSavedGame()
		{
			if (_debugMode)
				Debug.LogFormat("GameSave:OpenSavedGame");
			GameServices.SavedGames.OpenWithAutomaticConflictResolution("SavedGame", OnSavedGameOpened);
		}

		// Open saved game callback
		private void OnSavedGameOpened(SavedGame savedGame, string error)
		{
			if (string.IsNullOrEmpty(error)) {
				Debug.LogFormat("GameSaveSync:OnSavedGameOpened - Saved game opened successfully, savedGame.ModificationDate={0}", savedGame.ModificationDate);
				_mySavedGame = savedGame;
				if (_readSavedGameAfterOpen) {
					ReadSavedGame();
					_readSavedGameAfterOpen = false;
				}
			} else {
				Debug.Log("Open saved game failed with error: " + error);
			}
		}

		public static void OnGameSaveUpdated() // Public for debug
		{
			gameSaveUpdated?.Invoke();
		}

		#region Saved Game Read

		// Retrieves the binary data associated with the specified saved game
		private void ReadSavedGame()
		{
			if (_mySavedGame.IsOpen) {
				// The saved game is open and ready for reading
				GameServices.SavedGames.ReadSavedGameData(
					_mySavedGame,
					(SavedGame game, byte[] data, string error) => {
						if (string.IsNullOrEmpty(error)) {
							Debug.Log("Saved game data has been retrieved successfully!");
							// Here you can process the data as you wish.
							if (data.Length > 0) {
								// Data processing
								CheckSavedGame(data);
							} else {
								Debug.Log("The saved game has no data!");
							}
						} else {
							Debug.Log("Reading saved game data failed with error: " + error);
						}
					}
				);
			} else {
				// The saved game is not open. You can optionally open it here and repeat the process.
				Debug.Log("You must open the saved game before reading its data.");
				OpenSavedGame();
			}
		}

		private byte[] _serverBytes;

		private void CheckSavedGame(byte[] bytes)
		{
			ES3File localFile = new ES3File(ES3Settings.defaultSettings);

			// Note: Cannot use ES3File directly on encrypted bytes, otherwise will throw error, i.e. ES3File serverFile = new ES3File(bytes, ES3Settings.defaultSettings);
			// Need to save the bytes as a tmp file first and read it
			string tmpPath = ES3Settings.defaultSettings.path.Replace(".es3", ".tmp");
			var tmpSetting = new ES3Settings(tmpPath, ES3Settings.defaultSettings);
			ES3.SaveRaw(bytes, tmpPath); // Need to save the bytes without encyrption here
			ES3File serverFile = new ES3File(tmpSetting); // Load the bytes again from the tmp file with encryption

			DateTime localFileDateTime = localFile.Load<DateTime>("Timestamp");
			DateTime serverFileDateTime = serverFile.Load<DateTime>("Timestamp");

			if (_debugMode) {
				Debug.LogFormat("GameSave:CheckServerSave - localFileDateTime={0}, serverFileDateTime={1}", localFileDateTime, serverFileDateTime);
				Debug.LogFormat("GameSave:CheckServerSave - localFile={0}", localFile.LoadRawString());
				Debug.LogFormat("GameSave:CheckServerSave - serverFile={0}", serverFile.LoadRawString());
			}

			if (GameSave.isNew || serverFileDateTime > localFileDateTime) {

				_serverBytes = bytes;

				if (!string.IsNullOrEmpty(_replaceWithServerSaveContent))
					AskReplaceWithServerSavedGame();
				else // Directly replace with server save without asking
					OnConfirmReplaceWithServerSavedGame();

			}

		}

		private void AskReplaceWithServerSavedGame()
		{
			ES3File serverFile = new ES3File(_serverBytes);
			DateTime serverFileDateTime = serverFile.Load<DateTime>("Timestamp");

			new Message {
				title = LocalizeUtils.Get(TermCategory.Message, "Game Progress"),
				content = LocalizeUtils.GetFormat(TermCategory.Message, _replaceWithServerSaveContent, GameSave.Get<int>(_replaceServerSaveContentKey)) + "\n" + LocalizeUtils.GetFormat(TermCategory.Message, "(Saved at {0})", serverFileDateTime),
				confirmCallback = OnConfirmReplaceWithServerSavedGame,
				cancelCallback = AskOverrideServerSavedGame
			}.Show();
		}

		private void OnConfirmReplaceWithServerSavedGame()
		{
			ES3.CreateBackup(ES3Settings.defaultSettings);
			ES3.SaveRaw(_serverBytes, ES3Settings.defaultSettings.path);
			_serverBytes = new byte[0]; // Reset to save memory
			if (GameSave.isNew) // Server save shouldn't be isNew=true, just to make sure isNew=false in case
				GameSave.isNew = false;
			if (_debugMode)
				Debug.Log("GameSaveSync:OnConfirmReplaceWithServerSavedGame - Coins=" + GameSave.Get<int>("Coins"));
			Invoke("InvokeGameSaveUpdated", 0.5f); // Give 0.5s delay
			if (_reloadSceneAfterReplace) Invoke("ReloadCurrentScene", 1);
		}

		private void ReloadCurrentScene()
		{
			GameSceneManager.ReloadActiveScene();
		}

		private void AskOverrideServerSavedGame()
		{
			new Message {
				title = LocalizeUtils.Get(TermCategory.Message, "Override Progress"),
				content = LocalizeUtils.Get(TermCategory.Message, "Sure to continue playing and overwrite the previous progress?"),
				confirmCallback = OnConfirmOverrideServerSavedGame,
				cancelCallback = AskReplaceWithServerSavedGame
			}.Show();
		}

		private void OnConfirmOverrideServerSavedGame()
		{
			_serverBytes = new byte[0]; // Reset the save memory
		}

		private void InvokeGameSaveUpdated()
		{
			OnGameSaveUpdated();
		}

		#endregion

		#region Saved Game Write

		private float _lastSaveOnlineTime = -10;

		private void SaveOnline(SyncPolicy SyncPolicy) // non-static to force creating an instance
		{
			if (SyncPolicy == SyncPolicy.NoSync) // Skip if force no sync
				return;

			if (!GameServices.IsInitialized()) // Skip if not yet signed in of lost connection
				return;

			if (_mySavedGame == null) { // Don't save/override to server if never loaded
				Debug.LogError("No saved game is opened.");
				return;
			}

			if (_debugMode)
				Debug.LogFormat("GameSave:SaveOnline - file={0}", ES3.LoadRawString(ES3Settings.defaultSettings));

			StopAllCoroutines();

			if (SyncPolicy == SyncPolicy.AutoSync && Time.unscaledTime - _lastSaveOnlineTime < 30) { // Have a sync within last 30 seconds, schedule it and return
				Debug.Log("GameSave:SaveOnline - Sync already run in last 30 seconds, wait for it a bit now.");
				StartCoroutine(WaitAndSaveOnlineCoroutine(Mathf.CeilToInt(30 - Time.unscaledTime + _lastSaveOnlineTime), SyncPolicy));
			} else {
				try {
					WriteSavedGame();
					_lastSaveOnlineTime = Time.unscaledTime;
				} catch (Exception e) {
					Debug.Log("GameSave:SaveOnline - Error: " + e);
				}
			}

		}

		// Updates the given binary data to the specified saved game
		private void WriteSavedGame()
		{
			if (_mySavedGame.IsOpen) {
				if (_debugMode)
					Debug.LogFormat("GameSave:WriteSavedGame");
				// The saved game is open and ready for writing
				GameServices.SavedGames.WriteSavedGameData(
					_mySavedGame,
					ES3.LoadRawBytes(ES3Settings.defaultSettings),
					(SavedGame updatedSavedGame, string error) => {
						if (string.IsNullOrEmpty(error)) {
							Debug.Log("Saved game data has been written successfully!");
							OpenSavedGame();
						} else {
							Debug.Log("Writing saved game data failed with error: " + error);
						}
					}
				);
			} else {
				// The saved game is not open. You can optionally open it here and repeat the process.
				Debug.Log("You must open the saved game before writing to it.");
				OpenSavedGame();
			}
		}

		private IEnumerator WaitAndSaveOnlineCoroutine(int waitSeconds, SyncPolicy SyncPolicy)
		{
			yield return new WaitForSeconds(waitSeconds);
			SaveOnline(SyncPolicy);
		}

		#endregion

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Game Save/Invoke Game Save Updated Event")]
		public static void DebugOnGameSaveUpdated()
		{
			DebugUtils.CheckPlaying(OnGameSaveUpdated);
		}

		[UnityEditor.MenuItem("Debug/Game Save/Simulate Server Save Downloaded")]
		public static void DebugSimulateServerSaveDownloaded()
		{
			DebugUtils.CheckPlaying(() => {
				if (instanceExists) {
					string tmpPath = ES3Settings.defaultSettings.path.Replace(".es3", ".tmp");
					var tmpSetting = new ES3Settings(tmpPath, ES3Settings.defaultSettings);
					ES3.CopyFile(ES3Settings.defaultSettings, tmpSetting);
					ES3.Save("Timestamp", DateTime.Now, tmpSetting);
					ES3.Save("UnlockedLevel", UnityEngine.Random.Range(1, 10), tmpSetting);
					ES3.Save("Coins", UnityEngine.Random.Range(1, 999), tmpSetting);
					ES3.Save("Gems", UnityEngine.Random.Range(1, 999), tmpSetting);
					Debug.Log(ES3.Load<System.DateTime>("Timestamp", ES3Settings.defaultSettings));
					Debug.Log(ES3.Load<System.DateTime>("Timestamp", tmpSetting));
					byte[] bytes = ES3.LoadRawBytes(tmpSetting);
					instance.CheckSavedGame(bytes);
				}
			});
		}

		[UnityEditor.MenuItem("Debug/Game Save/Delete Game Save")]
		public static void DebugReset()
		{
			GameSave.ResetSave();
			OnGameSaveUpdated();
		}

#endif

#endif

	}

}