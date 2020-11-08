using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply destroys current game object on WebGL builds.
	/// </summary>
	public class DestroyOnWebGL : MonoBehaviour
	{

		private void Awake()
		{
#if UNITY_WEBGL
			Destroy(gameObject);
#endif
		}

	}

}