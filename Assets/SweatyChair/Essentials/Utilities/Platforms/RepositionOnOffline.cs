using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Repositions the current transform on offline builds.
	/// </summary>
	public class RepositionOnOffline : MonoBehaviour
	{

		[Tooltip("Local position to set on WebGL")]
		[SerializeField] Vector3 _localPosition;
		[Tooltip("Executes on start, or otherwise on awake")]
		[SerializeField] bool _onStart;

		private void Awake()
		{
#if NO_ADS
			if (!_onStart)
				Reposition();
#endif
		}

		private void Start()
		{
#if NO_ADS
			if (_onStart)
				Reposition();
#endif
		}

		private void Reposition()
		{
			transform.localPosition = _localPosition;
		}

	}

}