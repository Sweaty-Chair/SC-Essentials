using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on no-Ads builds.
	/// </summary>
	public class DisableOnNoAds : MonoBehaviour
	{
		private void Awake()
		{
#if NO_ADS
		gameObject.SetActive(false);
#endif
		}
	}


}