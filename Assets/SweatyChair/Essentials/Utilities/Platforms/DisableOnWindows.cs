using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on Windows builds.
	/// </summary>
	public class DisableOnWindows : MonoBehaviour
	{
		private void Awake()
		{
#if UNITY_STANDALONE_WIN
		gameObject.SetActive(false);
#endif
		}
	}

}