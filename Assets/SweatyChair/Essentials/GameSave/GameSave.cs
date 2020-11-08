using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace SweatyChair
{

	public enum SyncPolicy
	{
		// No sync, normally this is used when saving few variables at the same time
		NoSync,
		// Auto sync in a interval, this is the default behaviour and avoid player wasting too much brandwidth on syncing
		AutoSync,
		// Force to sync NOW, shouldn't use this for most of the time, only use this for extremely important vaiable
		ForceSync
	}

	/// <summary>
	/// A wrapper to save and load data to a persistent file, using Easy Save. This will also 
	/// </summary>
	public static class GameSave
	{

		// Events fired when any save executed
		public static event UnityAction<SyncPolicy> gameSaveSaved;

		public static bool isNew {
			get { return Get<bool>("IsNew"); }
			set { Set<bool>("IsNew", value); }
		}

		static GameSave()
		{
			// Set isNew
			if (!ES3.FileExists(ES3Settings.defaultSettings))
				isNew = true;
			else if (isNew) // Unset new
				isNew = false;
		}

		#region Get/Set

		/// <summary>
		/// Get the value and load into object. E.g. loading a saved Transform - GetInto<Transform>("SavedTransform", transform);
		/// Use this if really neccessary coz this may occupy large disk space.
		/// </summary>
		public static void GetInto<T>(string key, T obj) where T : class
		{
			ES3.LoadInto(key, obj, ES3Settings.defaultSettings);
		}

		public static T Get<T>(string key, T defaultValue = default)
		{
			//Debug.LogFormat("GameSave:Get<{0}>({1})", typeof(T), key);
			try {
				return ES3.Load(key, defaultValue, ES3Settings.defaultSettings);
			} catch (Exception e) {
				if (e is FormatException) {
					Debug.LogError("GameSave:Get - File format invalid, deleting the file now...");
					RestoreBackup();
				}
				return default(T);
			}
		}

		private static bool _ioExceptionWarningShown;

		public static void Set<T>(string key, object value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			//Debug.LogFormat("GameSave:Set({0},{1},{2})", key, StringUtils.ObjectToString(value), syncPolicy);
			//ES3.Save<T>(key, _gameSaveFilename, value);
			try {
				using (ES3Writer writer = ES3Writer.Create(ES3Settings.defaultSettings)) {
					writer.Write<DateTime>("Timestamp", DateTime.Now);
					writer.Write<T>(key, value);
					writer.Save();
				}
				gameSaveSaved?.Invoke(syncPolicy);
			} catch (System.IO.IOException e) {
				if (!_ioExceptionWarningShown) {
					new UI.Message {
						title = "Saving Failed".Localize(TermCategory.Message),
						content = "Please check if you have sufficient storage.".Localize(TermCategory.Message)
					}.Show();
					_ioExceptionWarningShown = true;
				}
			}
		}

		/// <summary>
		/// Delete the specified key, mostly used for debug only.
		/// </summary>
		public static bool HasKey(string key)
		{
			return ES3.KeyExists(key, ES3Settings.defaultSettings);
		}

		/// <summary>
		/// Delete the specified key, mostly used for debug only.
		/// </summary>
		public static void DeleteKey(string key, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			ES3.DeleteKey(key, ES3Settings.defaultSettings);
			gameSaveSaved?.Invoke(syncPolicy);
		}

		public static object GetObject(string key, object defaultValue = null)
		{
			return Get(key, defaultValue);
		}

		public static void SetObject(string key, object value = null, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<object>(key, value, syncPolicy);
		}

		#region Primitive GameData Types

		public static bool GetBool(string key, bool defaultValue = false)
		{
			return Get(key, defaultValue);
		}

		public static void SetBool(string key, bool value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<bool>(key, value, syncPolicy);
		}

		public static int GetInt(string key, int defaultValue = 0)
		{
			return Get(key, defaultValue);
		}

		public static void SetInt(string key, int value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<int>(key, value, syncPolicy);
		}

		public static void IncrementInt(string key, int value = 1, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			SetInt(key, GetInt(key) + value, syncPolicy);
		}

		public static float GetFloat(string key, float defaultValue = 0)
		{
			return Get(key, defaultValue);
		}

		public static void SetFloat(string key, float value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<float>(key, value, syncPolicy);
		}

		public static string GetString(string key, string defaultValue = "")
		{
			return Get(key, defaultValue);
		}

		public static void SetString(string key, string value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<string>(key, value, syncPolicy);
		}

		#endregion

		#region DateTime

		public static DateTime GetDateTime(string key, DateTime defaultValue = default(DateTime))
		{
			return Get(key, defaultValue);
		}

		public static void SetDateTime(string key, DateTime value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<DateTime>(key, value, syncPolicy);
		}

		#endregion

		#region Array

		public static T[] GetArray<T>(string key, T defaultValue = default, int defaultSize = 0)
		{
			T[] result = Get<T[]>(key);
			if (result == null) {
				result = new T[defaultSize];
				for (int i = 0; i < defaultSize; i++)
					result[i] = defaultValue;
			}
			return result;
		}

		public static int[] GetIntArray(string key, int defaultValue = 0, int defaultSize = 0)
		{
			return GetArray(key, defaultValue, defaultSize);
		}

		public static bool[] GetBoolArray(string key, bool defaultValue = false, int defaultSize = 0)
		{
			return GetArray(key, defaultValue, defaultSize);
		}

		public static string[] GetStringArray(string key, string defaultValue = "", int defaultSize = 0)
		{
			return GetArray(key, defaultValue, defaultSize);
		}

		public static void SetIntArray(string key, IList<int> value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<int[]>(key, value, syncPolicy);
		}

		public static void SetBoolArray(string key, IList<bool> value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<bool[]>(key, value, syncPolicy);
		}

		public static void SetStringArray(string key, IList<string> value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<string[]>(key, value, syncPolicy);
		}

		#endregion

		#region List

		public static List<T> GetList<T>(string key, T defaultValue = default, int defaultSize = 0)
		{
			List<T> result = Get<List<T>>(key);
			if (result == null) {
				result = new List<T>();
				for (int i = 0; i < defaultSize; i++)
					result.Add(defaultValue);
			}
			return result;
		}

		public static List<int> GetIntList(string key, int defaultValue = 0, int defaultSize = 0)
		{
			return GetList(key, defaultValue, defaultSize);
		}

		public static List<bool> GetBoolList(string key, bool defaultValue = false, int defaultSize = 0)
		{
			return GetList(key, defaultValue, defaultSize);
		}

		public static List<string> GetStringList(string key, string defaultValue = "", int defaultSize = 0)
		{
			return GetList(key, defaultValue, defaultSize);
		}

		public static void SetIntList(string key, List<int> value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<List<int>>(key, value, syncPolicy);
		}

		public static void SetBoolList(string key, List<bool> value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<List<bool>>(key, value, syncPolicy);
		}

		public static void SetStringList(string key, List<string> value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<List<string>>(key, value, syncPolicy);
		}

		// Add an unique ID to an ID list
		public static void AddIntList(string key, int value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			List<int> tmp = GetIntList(key);
			if (!tmp.Contains(value)) {
				tmp.Add(value);
				SetIntList(key, tmp, syncPolicy);
			}
		}

		// Remove an unique ID fron an ID list
		public static void RemoveIntList(string key, int value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			List<int> tmp = GetIntList(key);
			if (tmp.Contains(value)) {
				tmp.Remove(value);
				SetIntList(key, tmp, syncPolicy);
			}
		}

		#endregion List

		#region Dictionary

		public static Dictionary<int, int> GetIntDictionary(string key, int defaultValue = 0, int defaultSize = 0)
		{
			Dictionary<int, int> result = Get<Dictionary<int, int>>(key);
			if (result == null) {
				result = new Dictionary<int, int>();
				for (int i = 1; i <= defaultSize; i++)
					result.Add(i, defaultValue);
			}
			return result;
		}

		public static int GetIntDictionaryValue(string key, int dictKey, int defaultValue = 0)
		{
			Dictionary<int, int> dict = GetIntDictionary(key);
			int tmp = defaultValue;
			if (dict != null && dict.ContainsKey(dictKey))
				tmp = dict[dictKey];
			return tmp;
		}


		public static void SetIntDictionary(string key, Dictionary<int, int> value, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Set<Dictionary<int, int>>(key, value, syncPolicy);
		}

		/// <summary>
		/// Sets or adds ONE key-value to int dictionary.
		/// </summary>
		public static void SetIntDictionary(string key, int dictKey, int dictValue, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Dictionary<int, int> result = GetIntDictionary(key);
			result[dictKey] = dictValue;
			SetIntDictionary(key, result, syncPolicy);
		}

		public static void IncrementIntDictionary(string key, int dictKey, int inc = 1, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			try {
				Dictionary<int, int> dict = GetIntDictionary(key, 0, 0);
				int dictValue = inc;
				if (dict != null && dict.ContainsKey(dictKey))
					dictValue = dict[dictKey] + inc;
				SetIntDictionary(key, dictKey, dictValue, syncPolicy);
			} catch (Exception e) {
				Debug.LogWarningFormat("GameSaveManager:IncrementIntDictionary - Cannot find key={0}, error={1}", key, e);
			}
		}

		/// <summary>
		/// Remove ONE key-value from int dictionary, if exists.
		/// </summary>
		public static void RemoveIntDictionary(string key, int dictKey, SyncPolicy syncPolicy = SyncPolicy.AutoSync)
		{
			Dictionary<int, int> result = GetIntDictionary(key);
			if (result.ContainsKey(dictKey)) {
				result.Remove(dictKey);
				SetIntDictionary(key, result, syncPolicy);
			}
		}

		#endregion

		/// <summary>
		/// Restore the backup if any.
		/// </summary>
		public static void RestoreBackup()
		{
			ES3.RestoreBackup(ES3Settings.defaultSettings);
		}

		/// <summary>
		/// Reset the game save, use this with extra caution.
		/// </summary>
		public static void ResetSave()
		{
			ES3.DeleteFile(ES3Settings.defaultSettings);
			isNew = true;
		}

		#endregion

		#region Tools

		public static void Log()
		{
			if (ES3.FileExists(ES3Settings.defaultSettings)) {
				Debug.Log(ES3.LoadRawString(ES3Settings.defaultSettings));
			} else {
				Debug.Log("GameSaveManger:Log - save file not exists.");
			}
		}

		public static void LogBytes()
		{
			if (ES3.FileExists(ES3Settings.defaultSettings)) {
				DebugUtils.Log(ES3.LoadRawBytes(ES3Settings.defaultSettings));
			} else {
				Debug.Log("GameSaveManger:Log - save file not exists.");
			}
		}

		#endregion

		#region Debug

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Game Save/Print Game Save", false, 400)]
		public static void DebugLog()
		{
			Log();
		}

		[UnityEditor.MenuItem("Debug/Game Save/Force Save Now", false, 400)]
		public static void DebugMannualSave()
		{
			SetInt("dump", 0);
			Debug.LogFormat("Successfully saved at {0}", ES3Settings.defaultSettings.path);
		}

#endif
		#endregion

	}

}