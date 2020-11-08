using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply destroys current game object on non-WebGL builds.
	/// </summary>
	public class DestroyOnNonWebGL : MonoBehaviour
	{

		private void Awake()
		{
#if !UNITY_WEBGL
			Destroy(gameObject);
#endif
		}

	}

}