using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Repositions the current transform on WebGL builds.
	/// </summary>
	public class RepositionOnWebGL : MonoBehaviour
	{

		[Tooltip("Local position to set on WebGL")]
		[SerializeField] Vector3 _localPosition;
		[Tooltip("Executes on start, or otherwise on awake")]
		[SerializeField] bool _onStart;

		private void Awake()
		{
#if UNITY_WEBGL
			if (!_onStart)
				Reposition();
#endif
		}

		private void Start()
		{
#if UNITY_WEBGL
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