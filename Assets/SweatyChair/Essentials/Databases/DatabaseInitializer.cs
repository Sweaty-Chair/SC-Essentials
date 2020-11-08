using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A simple class that simply initialize DatabaseManager.
	/// </summary>
	public class DatabaseInitializer : MonoBehaviour
	{

		private void Awake()
		{
			DatabaseManager.Init();
			Destroy(this);
		}

	}

}