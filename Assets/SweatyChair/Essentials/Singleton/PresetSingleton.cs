using UnityEngine;

namespace SweatyChair
{

	// A singleton controller that loads the pre-set prefab from Resources/Prefabs/Singletons/ and creates its singleton.
	// This different to Singleton class, which is either attached in scene, or created in runtime.
	public abstract class PresetSingleton<T> : MonoBehaviour where T : PresetSingleton<T>
	{

		[SerializeField] protected bool _dontDestroyOnLoad = true;
		protected const string PREFAB_RESOURCES_PATH = "Singletons/{0}";

		protected static T _instance;
		public static T instance {
			get {
				if (!Application.isPlaying || _isApplicationQuitting) { _instance = null; return null; }

				if (_instance == null) {
					_instance = FindObjectOfType<T>();

					if (FindObjectsOfType<T>().Length > 1) {
						Debug.LogErrorFormat("[PresetSingleton : {0}] Something went really wrong " +
						" - there should never be more than 1 singleton of this type!" +
						" Reopening the scene might fix it.", typeof(T).FullName);
						return _instance;
					}

					if (_instance == null) {
						GameObject singletonGO = Resources.Load<GameObject>(string.Format(PREFAB_RESOURCES_PATH, typeof(T).Name));
						if (singletonGO != null) {
							singletonGO = Instantiate(singletonGO);
							_instance = singletonGO.GetComponent<T>();
							_instance.Init();
						} else {
							Debug.LogErrorFormat("PresetSingleton<{0}>:instance - Prefab not found at: '{1}'.", typeof(T).FullName, string.Format(PREFAB_RESOURCES_PATH, typeof(T).Name));
						}
					} else {
						Debug.Log("[Singleton] Using instance already created: " +
						_instance.gameObject.name);
					}
				}

				return _instance;
			}
		}

		public static bool instanceExists => _instance != null;

		private static bool _isApplicationQuitting;

		protected virtual void Awake()
		{
			if (instanceExists && _instance != this) {
				Destroy(gameObject);
				return;
			}
			if (_dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}

		// Called at instantiate
		protected virtual void Init() { }

		protected virtual void OnApplicationQuit()
		{
			_isApplicationQuitting = true;
		}

	}

}