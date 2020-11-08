using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Repositions the current game object on CHS builds.
	/// </summary>
	public class RepositionOnCHS : MonoBehaviour
	{

		[Tooltip("Local position to set on CHS")]
		[SerializeField] Vector3 _localPosition;
		[Tooltip("Executes on start, or otherwise on awake")]
		[SerializeField] bool _onStart;

		private void Awake()
		{
#if CHS
            if (!_onStart)
                transform.localPosition = _localPosition;
#endif
		}

		private void Start()
		{
#if CHS
			if (_onStart)
                transform.localPosition = _localPosition;
#endif
		}

	}

}