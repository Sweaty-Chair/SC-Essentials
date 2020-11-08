using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on no social builds.
	/// </summary>
	public class DisableOnNoSocial : MonoBehaviour
	{
		private void Awake()
		{
#if NO_SOCIAL
		gameObject.SetActive(false);
#endif
		}
	}


}