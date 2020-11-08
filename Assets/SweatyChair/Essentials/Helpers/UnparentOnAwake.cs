using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Unparent the current game object, used for SoundManager that need to be on root.
	/// </summary>
	public class UnparentOnAwake : MonoBehaviour
	{

		private void Awake()
		{
			transform.parent = null;
		}

	}

}