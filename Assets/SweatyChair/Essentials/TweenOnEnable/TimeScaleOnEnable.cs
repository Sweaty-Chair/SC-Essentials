using UnityEngine;

namespace SweatyChair
{

	public class TimeScaleOnEnable : MonoBehaviour
    {

		[Header("Settings")]
        [SerializeField] private float _targetTimeScale = 0;
        [SerializeField] private float _lastTimeScale = 1;
		[Space]
        [SerializeField] private bool _usePrevTimeScale = true;


        private void OnEnable()
		{
            if (_usePrevTimeScale)
    			_lastTimeScale = Time.timeScale;

			Time.timeScale = _targetTimeScale;
		}

		private void OnDisable()
		{
			Time.timeScale = _lastTimeScale;
		}

	}

}