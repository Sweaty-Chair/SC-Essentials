using UnityEngine;
using System.Collections.Generic;

namespace SweatyChair
{

	/// <summary>
	/// A base abstract class of a local database instance.
	/// Each database should have a csv with same name.
	/// T is the class type of data this database contains.
	/// </summary>
	public abstract class Database<T> where T : BaseData, new()
	{

		// Database csv filename, same to the script filename by defaut
        protected virtual string filename => GetType().Name;

		// GameData dictionary this database contains
		protected Dictionary<int, T> _dataDict { get; private set; }

		protected Database()
		{
			if (DatabaseSettings.current.debugMode)
				Debug.LogFormat("Loading database {0}...", filename);
			TextAsset csv = Resources.Load<TextAsset>(filename);
			if (csv == null) {
				Debug.LogErrorFormat("{0} - Can't find database file, filename={1}", GetType(), filename);
				return;
			}
			ReadDatabase(csv);
		}

		/// <summary>
		/// Read csv database from Resource or in a asset bundle.
		/// </summary>
		/// <param name="csv">TextAsset from csv file</param>
		protected void ReadDatabase(TextAsset csv)
		{
			_dataDict = new Dictionary<int, T>();
			ES3Spreadsheet sheet = new ES3Spreadsheet();
			sheet.LoadRaw(csv.text);
			string[] cells = new string[sheet.ColumnCount];
			for (int row = 1; row < sheet.RowCount; row++) { // Skip the first heading row
				for (int col = 0; col < sheet.ColumnCount; col++)
					cells[col] = sheet.GetCell<string>(col, row);
				ReadDatabaseRow(cells);
			}
			PostDatabaseRead();
		}

		/// <summary>
		/// Create data list based on csv database column.
		/// </summary>
		/// <param name="cells">CSV cells</param>
		protected virtual void ReadDatabaseRow(string[] cells)
		{
			T data = new T();
			if (!data.FeedData(cells))
				return;
			if (data == null) {
				Debug.LogErrorFormat("{0}:ReadDatabaseColumn - Invalid column", GetType());
				return;
			}
			if (_dataDict.ContainsKey(data.id))
				Debug.LogErrorFormat("{0}:ReadDatabaseColumn - Duplicated id={1}", GetType(), data.id);
			_dataDict[data.id] = data;
		}

		/// <summary>
		/// Post progess after a database read, do e.g. cache the item totals.
		/// </summary>
		protected virtual void PostDatabaseRead()
		{
		}

		public T Get(int id)
		{
			if (!_dataDict.ContainsKey(id)) {
				Debug.LogErrorFormat("{0}:GetData - Invalid id={1}", GetType(), id);
				return null;
			}
			return _dataDict[id];
		}

		public List<T> GetAll()
		{
			return new List<T>(_dataDict.Values);
		}

		public override string ToString()
		{
			return string.Format("[Database: type={0}, itemCount={1}]", GetType(), _dataDict.Count);
		}

		public List<string> GetColumnNames()
		{
			var fileds = typeof(T).GetFields();

			List<string> names = new List<string>();
			foreach (var filed in fileds) {
				names.Add(filed.Name);
			}

			return names;
		}

		public List<string> GetColumnValues(string columnName)
		{
			List<string> fildNames = GetColumnNames();
			List<string> columnValues = new List<string>();
			if (fildNames.Contains(columnName)) {
				List<T> allData = DatabaseManager.GetDatabase<T>().GetAll();
				foreach (T data in allData) {
					columnValues.Add(typeof(T).GetField(columnName).GetValue(data).ToString());
				}
				return columnValues;
			} else {
				Debug.LogErrorFormat("{0}:GetColumnValues - does not contain a field name with [{1}]", GetType(), columnName);
				return null;
			}
		}

		/// <summary>
		/// For DEBUG, print the database items.
		/// </summary>
		public virtual void PrintDatabase()
		{
			if (_dataDict == null || _dataDict.Count == 0) {
				Debug.LogFormat("{0}:PrintDatabase - Haven't create database or database has not been initialized.", GetType());
				return;
			}
			DebugUtils.Log(_dataDict);
		}

	}
}