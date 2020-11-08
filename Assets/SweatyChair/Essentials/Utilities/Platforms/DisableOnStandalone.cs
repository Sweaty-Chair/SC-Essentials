using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on standalone platforms.
	/// </summary>
	public class DisableOnStandalone : MonoBehaviour
	{
		private void Awake()
		{
#if UNITY_STANDALONE
		gameObject.SetActive(false);
#endif
		}
	}

}