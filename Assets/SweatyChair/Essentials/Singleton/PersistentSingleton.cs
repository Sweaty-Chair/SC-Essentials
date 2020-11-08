namespace SweatyChair
{
	/// <summary>
	/// Singleton that persists across multiple scenes
	/// </summary>
	public class PersistentSingleton<T> : Singleton<T> where T : Singleton<T>
	{
		protected override void Awake()
		{
			base.Awake();
			if (instanceExists || instance == this) { // Skip if destroying
				if (transform.parent != null)
					transform.parent = null; // Make sure this is on root so DontDestroyOnLoad works
				DontDestroyOnLoad(gameObject);
			}
		}
	}
}
