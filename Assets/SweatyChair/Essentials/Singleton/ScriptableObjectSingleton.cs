using System.Linq;
using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Scriptable object singleton, mostly used for settings where only ONE instance should be present in game.
	/// </summary>
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
#endif
	public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject, new()
	{

		private static T _instance;

		/// <summary>
		/// Find the settings file in Resources folder and asset it as current.
		/// </summary>
		public static T current {
			get {
				if (!_instance) {
					// Try load with exact class name as filename first
					_instance = Resources.Load<T>(typeof(T).Name);
					if (_instance == null) {
#if UNITY_EDITOR
						// Create an assets file in SweatyChair/Resources
						_instance = CreateInstance<T>();
						string assetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/SweatyChair/Resources/{0}.asset", typeof(T).Name));
						if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/SweatyChair/Resources")) // Create the Resources folder if not yet present
							UnityEditor.AssetDatabase.CreateFolder("Assets/SweatyChair", "Resources");
						Debug.LogFormat("Creating {0} asset at {1}", typeof(T).Name, assetPath);
						UnityEditor.AssetDatabase.CreateAsset(_instance, assetPath);
#endif
						if (_instance == null) {
							// Try find with object type, shouldn't come here unless the database is renamed
							_instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
							if (_instance == null) {
								// Create a new one if none can't find any
								Debug.LogErrorFormat("{0} - Not found setting file in Resources folder.", typeof(T));
								_instance = new T(); // Create a new one with default settings
							}
						}
					}
				}
				return _instance;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Opens the assets file from current.
		/// </summary>
		protected void OpenAssetsFile()
		{
			UnityEditor.Selection.activeObject = this;
		}
#endif

	}

}