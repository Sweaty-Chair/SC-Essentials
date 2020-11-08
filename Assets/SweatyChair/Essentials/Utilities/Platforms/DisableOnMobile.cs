using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on mobile builds.
	/// </summary>
	public class DisableOnMobile : MonoBehaviour
	{
		private void Start()
		{
#if UNITY_IOS || UNITY_ANDROID
			gameObject.SetActive(false);
#endif
		}
	}

}