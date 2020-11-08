using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SweatyChair
{

	public abstract class SODatabase<T> where T : SOData
	{

		#region Variables

		// The directory name in resource, override this for different directory to save loading time
		protected virtual string _resourceDir => string.Empty;

		protected Dictionary<int, T> _dataDict { get; private set; }

		#endregion

		#region Constructor / Initialization

		protected SODatabase()
		{
			if (SODatabaseSettings.current.debugMode)
				Debug.LogFormat("Loading SODatabase {0}...", typeof(T).FullName);

			// Load all of our data from Resources
			T[] allData = Resources.LoadAll<T>(_resourceDir).Where(item => item.load).ToArray();
			// Then go through all of our data, and attempt to parse it all into our dictionary
			InitializeDataDict(allData);

			if (SODatabaseSettings.current.debugMode)
				Debug.LogFormat("Finished Loading SODatabase {0}...", typeof(T).FullName);
		}

		/// <summary>
		/// Read csv database from Resource or in a asset bundle.
		/// </summary>
		/// <param name="csv">TextAsset from csv file</param>
		protected void InitializeDataDict(T[] datas)
		{
			_dataDict = new Dictionary<int, T>();

			// Go through and add all of our data to our database
			for (int i = 0; i < datas.Length; i++) {

				// Check if our data is null
				if (datas[i] == null)
					continue;

				// Then we just add our data to our dict.
				// We dont create a duplicate of the data, as we might want to make changes to our data at runtime, for easier editing
				if (_dataDict.ContainsKey(datas[i].id))
					Debug.LogErrorFormat("[{0}] : Duplicate data id = '{0}'", GetType(), datas[i].id);

				// Then add to our dict
				_dataDict[datas[i].id] = datas[i];
			}

			PostDatabaseRead();
		}

		/// <summary>
		/// Post progess after a database read, do e.g. cache the item totals.
		/// </summary>
		protected virtual void PostDatabaseRead()
		{

		}

		#endregion

		#region Get

		public T Get(int id)
		{
			if (!_dataDict.ContainsKey(id)) {
				Debug.LogErrorFormat("[{0}] : Get - Invalid id = {1}", GetType(), id);
				return null;
			}

			return _dataDict[id];
		}

		public List<T> GetAll()
		{
			return new List<T>(_dataDict.Values);
		}

		#endregion

		#region Query

		public T Query(Func<T, bool> predicate)
		{
			return _dataDict.Values.FirstOrDefault(predicate);
		}

		public IEnumerable<T> QueryMany(Func<T, bool> predicate)
		{
			return _dataDict.Values.Where(predicate);
		}

		#endregion

		#region Utility

		public override string ToString()
		{
			return string.Format("[Database]: type = '{0}', itemCount = '{1}'", GetType(), _dataDict.Count);
		}

		public virtual void PrintDatabase()
		{
			if (_dataDict == null || _dataDict.Count == 0) {
				Debug.LogFormat("[{0}] : PrintDatabase - Haven't create database or database has not been initialized.", GetType());
				return;
			}
			DebugUtils.Log(_dataDict);
		}

		#endregion

	}

}