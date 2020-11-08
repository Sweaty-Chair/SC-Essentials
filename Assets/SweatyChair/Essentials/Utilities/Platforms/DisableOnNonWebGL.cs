using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on non-WebGL builds.
	/// </summary>
	public class DisableOnNonWebGL : MonoBehaviour
	{
		private void Awake()
		{
#if !UNITY_WEBGL
		gameObject.SetActive(false);
#endif
		}
	}


}