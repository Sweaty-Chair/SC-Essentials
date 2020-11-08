using UnityEngine;
using System;
using System.Collections.Generic;

namespace SweatyChair
{

	/// <summary>
	/// Database manager that concontrol all local databases.
	/// Ensure its excutation order is earlier than others which depend on database.
	/// </summary>
	public static class DatabaseManager
	{

		private static Dictionary<Type, object> _databases = new Dictionary<Type, object>();

		public static DatabaseSettings setting { get { return DatabaseSettings.current; } }

		/// <summary>
		/// Initializes the <see cref="T:SweatyChair.DatabaseManager"/> class and all the databases in DatabaseSettings.
		/// </summary>
		static DatabaseManager()
		{
			foreach (string dbName in setting.databaseNames) {
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

		/// <summary>
		/// A empty function that's empty and simply execute the static constructor function
		/// </summary>
		public static void Init()
		{
		}

		/// <summary>
		/// Gets the database with given data type.
		/// </summary>
		/// <returns>The database with given data type.</returns>
		/// <typeparam name="T">GameData type.</typeparam>
		public static Database<T> GetDatabase<T>() where T : BaseData, new()
		{
			if (_databases.ContainsKey(typeof(T)))
				return (Database<T>)_databases[typeof(T)];
			Debug.LogErrorFormat("DatabaseManager:GetDatabase - No database for data type={0}", typeof(T));
			return null;
		}

		/// <summary>
		/// Gets the data with given data type and an ID.
		/// </summary>
		/// <returns>The data with given data type and an ID.</returns>
		/// <param name="id">GameData ID.</param>
		/// <typeparam name="T">GameData type.</typeparam>
		public static T GetData<T>(int id) where T : BaseData, new()
		{
			var database = GetDatabase<T>();
			if (database == null) {
				Debug.LogErrorFormat("DatabaseManager:GetItems - No database for data type={0}", typeof(T));
				return null;
			}
			return database.Get(id);
		}

		/// <summary>
		/// Gets data list with given data type.
		/// </summary>
		/// <returns>The data list.</returns>
		/// <typeparam name="T">GameData type.</typeparam>
		public static List<T> GetDatas<T>() where T : BaseData, new()
		{
			var database = GetDatabase<T>();
			if (database != null)
				return database.GetAll();
			Debug.LogErrorFormat("DatabaseManager:GetDatas - No database for data type={0}", typeof(T));
			return null;
		}

	}

}