using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on offline builds.
	/// </summary>
	public class DisableOnOffline : MonoBehaviour
	{
		private void Awake()
		{
#if OFFLINE
		gameObject.SetActive(false);
#endif
		}
	}


}