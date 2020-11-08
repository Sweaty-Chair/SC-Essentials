using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on non-Chinese builds.
	/// </summary>
	public class DisableOnNonCHS : MonoBehaviour
	{
		private void Awake()
		{
#if !CHS
		gameObject.SetActive(false);
#endif
		}
	}


}