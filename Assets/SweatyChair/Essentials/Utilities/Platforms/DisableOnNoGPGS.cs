using UnityEngine;

namespace SweatyChair
{

	// Disables current game object on NO_GPGS (No Google Play Games Service) builds
	public class DisableOnNoGPGS : MonoBehaviour
	{
		private void Awake()
		{
#if NO_GPGS
		gameObject.SetActive(false);
#endif
		}
	}


}