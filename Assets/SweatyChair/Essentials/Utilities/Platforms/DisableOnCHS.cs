using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on CHS builds.
	/// </summary>
	public class DisableOnCHS : MonoBehaviour
	{
		private void Awake()
		{
#if CHS
		gameObject.SetActive(false);
#endif
		}
	}


}