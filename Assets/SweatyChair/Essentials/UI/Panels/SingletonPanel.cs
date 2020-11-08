namespace SweatyChair.UI
{

	/// <summary>
	/// Panel using singleton approach.
	/// NOTE: This is for simple projects and prototype, always try using PanelManager.Get<XxxPanel>() to access the panel.
	/// </summary>
	/// <typeparam name="T">Panel</typeparam>
	public abstract class SingletonPanel<T> : Panel where T : SingletonPanel<T>
	{

		private static T _instance;

		public static T instance {
			get {
				if (_instance == null)
					_instance = PanelManager.Get<T>();
				if (_instance == null) // Use FindObjectOfType (more expansive) if it's not the root panels. Notes that this can only find the activated panels.
					_instance = FindObjectOfType<T>();
				//if (_instance == null)
					//Debug.LogErrorFormat("SingletonPanel:s_Instance - Cannot find the instance for {0}, please make sure it's added into the scene.", typeof(T));
				return _instance;
			}
		}

		public static bool instanceExists => _instance != null;

		/// <summary>
		/// A initialization method run on Awake.
		/// NOTE: Remember to put base.Init() if it needs to check state.
		/// </summary>
		public override void Init()
		{
			base.Init();
			_instance = (T)this;
		}

	}

}