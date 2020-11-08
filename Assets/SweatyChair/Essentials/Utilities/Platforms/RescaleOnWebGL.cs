using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Rescales the current transform on WebGL builds.
	/// </summary>
	public class RescaleOnWebGL : MonoBehaviour
	{

		[Tooltip("Local scale to set on WebGL")]
		[SerializeField] Vector3 _localScale = Vector3.one;
		[Tooltip("Executes on start, or otherwise on awake")]
		[SerializeField] bool _onStart;

		private void Awake()
		{
#if UNITY_WEBGL
			if (!_onStart)
				Rescale();
#endif
		}

		private void Start()
		{
#if UNITY_WEBGL
			if (_onStart)
				Rescale();
#endif
		}

		private void Rescale()
		{
			transform.localScale = _localScale;
		}

	}

}