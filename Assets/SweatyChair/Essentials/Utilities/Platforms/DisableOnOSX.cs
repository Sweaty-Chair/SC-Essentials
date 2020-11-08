using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on OSX builds.
	/// </summary>
	public class DisableOnOSX : MonoBehaviour
	{
		private void Awake()
		{
#if UNITY_STANDALONE_OSX
		gameObject.SetActive(false);
#endif
		}
	}

}