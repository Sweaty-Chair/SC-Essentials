using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on no-IAP builds.
	/// </summary>
	public class DisableOnNoIAP : MonoBehaviour
	{
		private void Awake()
		{
#if NO_IAP
		gameObject.SetActive(false);
#endif
		}
	}


}