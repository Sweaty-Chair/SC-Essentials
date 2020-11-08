using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on WebGL builds.
	/// </summary>
	public class DisableOnWebGL : MonoBehaviour
	{
		private void Awake()
		{
#if UNITY_WEBGL
		gameObject.SetActive(false);
#endif
		}
	}


}