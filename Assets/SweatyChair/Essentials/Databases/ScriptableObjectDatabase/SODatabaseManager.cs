using System;
using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Database manager that manages all scriptable object based databases.
	/// </summary>
	[DefaultExecutionOrder(-1000)]
	public static class SODatabaseManager
	{

		#region Variables

		private static Dictionary<Type, object> _databases = new Dictionary<Type, object>();

		public static SODatabaseSettings settings { get { return SODatabaseSettings.current; } }

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes the <see cref="T:SweatyChair.SODatabaseManager"/> class and all the databases in DatabaseSettings.
		/// </summary>
		static SODatabaseManager()
		{
			foreach (string dbName in settings.databaseNames) {
				Type dbType = Type.GetType(dbName);
				if (dbType == null)
					dbType = Type.GetType("SweatyChair." + dbName); // Try with SweatyChair namespace
				if (dbType == null) {
					Debug.LogErrorFormat("DatabaseManager - Invalid dbName={0}", dbName);
					continue;
				}
				object dbInstance = Activator.CreateInstance(dbType);
				Type dataType = dbType.BaseType.GetGenericArguments()[0];
				_databases.Add(dataType, dbInstance);
				System.Reflection.MethodInfo m = dbInstance.GetType().GetMethod("PostDatabaseCreated");
				if (m != null)
					m.Invoke(dbInstance, null);
			}
		}

		#endregion

		#region Init

		/// <summary>
		/// A empty function that's empty and simply executes the static constructor function
		/// </summary>
		public static void Init() { }

		#endregion

		#region Get

		/// <summary>
		/// Gets the database with given data type.
		/// </summary>
		/// <returns>The database with given data type.</returns>
		/// <typeparam name="T">GameData type.</typeparam>
		public static SODatabase<T> GetDatabase<T>() where T : SOData, new()
		{
			if (_databases.ContainsKey(typeof(T)))
				return (SODatabase<T>)_databases[typeof(T)];
			Debug.LogErrorFormat("[SODatabaseManager] : GetDatabase - No database for data type = {0}", typeof(T));
			return null;
		}

		/// <summary>
		/// Gets the data with given data type and an ID.
		/// </summary>
		/// <returns>The data with given data type and an ID.</returns>
		/// <param name="id">GameData ID.</param>
		/// <typeparam name="T">GameData type.</typeparam>
		public static T GetData<T>(int id) where T : SOData, new()
		{
			var database = GetDatabase<T>();
			if (database == null) {
				Debug.LogErrorFormat("[SODatabaseManager] : GetItems - No database for data type = {0}", typeof(T));
				return null;
			}
			return database.Get(id);
		}

		/// <summary>
		/// Gets data list with given data type.
		/// </summary>
		/// <returns>The data list.</returns>
		/// <typeparam name="T">GameData type.</typeparam>
		public static List<T> GetDatas<T>() where T : SOData, new()
		{
			var database = GetDatabase<T>();
			if (database != null)
				return database.GetAll();
			Debug.LogErrorFormat("[SODatabaseManager] : GetDatas - No database for data type = {0}", typeof(T));
			return null;
		}

		#endregion

		#region Query

		/// <summary>
		/// Queries the database with the provided predicate
		/// </summary>
		/// <returns>The data matching the predicate, or null if none were found</returns>
		/// <param name="predicate">The Search query.</param>
		/// <typeparam name="T">GameData type.</typeparam>
		public static T Query<T>(Func<T, bool> predicate) where T : SOData, new()
		{
			var database = GetDatabase<T>();
			if (database == null) {
				Debug.LogErrorFormat("[SODatabaseManager] : Query - No database for data type = {0}", typeof(T));
				return null;
			}
			return database.Query(predicate);
		}

		/// <summary>
		/// Queries the database with the provided predicate
		/// </summary>
		/// <returns>All data matching the predicate, or an empty enumerable if none were found</returns>
		/// <param name="predicate">The Search query.</param>
		/// <typeparam name="T">GameData type.</typeparam>
		public static IEnumerable<T> QueryMany<T>(Func<T, bool> predicate) where T : SOData, new()
		{
			var database = GetDatabase<T>();
			if (database == null) {
				Debug.LogErrorFormat("[SODatabaseManager] : QueryMany - No database for data type = {0}", typeof(T));
				return null;
			}
			return database.QueryMany(predicate);
		}

		#endregion

	}

}
