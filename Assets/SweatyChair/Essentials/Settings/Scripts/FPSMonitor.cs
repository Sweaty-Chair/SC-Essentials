using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SweatyChair
{

	/// <summary>
	/// A simple FPS monitor that can access anywhere.
	/// Use SHOW_FPS in scripting symbol for easy UI.
	/// </summary>
	public static class FPSMonitor
	{

		private const int SAMPLE_COUNT = 10;
		private const float SAMPLE_INTERVAL = 0.3f;

		public static int medianFramesPerSec {
			get {
				if (_medianFramesPerSec <= 0)
					return framesPerSec;
				return _medianFramesPerSec > 0 ? _medianFramesPerSec : 0;
			}
		}

		private static int _medianFramesPerSec = 0;

		private static List<int> _sampleFPSList = new List<int>();

		private static IEnumerator _fpsEnumerator;

		public static int framesPerSec { get; private set; }

		public static float sampleDuration => SAMPLE_COUNT * SAMPLE_INTERVAL;

		public static void Start()
		{
			if (_fpsEnumerator != null)
				TimeManager.Stop(_fpsEnumerator);
			_fpsEnumerator = FPSCoroutine();
			TimeManager.Start(_fpsEnumerator);
		}

		public static IEnumerator FPSCoroutine()
		{
			while (_sampleFPSList.Count < SAMPLE_COUNT) {
				// Capture frame-per-second
				int lastFrameCount = Time.frameCount;
				float lastTime = Time.realtimeSinceStartup;
				yield return new WaitForSecondsRealtime(SAMPLE_INTERVAL);
				float timeSpan = Time.realtimeSinceStartup - lastTime;
				int frameCount = Time.frameCount - lastFrameCount;

				// Display it
				framesPerSec = Mathf.RoundToInt(frameCount / timeSpan);

				if (Time.timeSinceLevelLoad >= 3) {
#if SHOW_FPS
					Debug.Log(Time.timeSinceLevelLoad);
#endif
					_sampleFPSList.Add(framesPerSec);
				}

#if SHOW_FPS
				Debug.LogFormat ("FPS Now:{0} SmapleCount:{1}", framesPerSec, _sampleFPSList.Count);
#endif
			}

			int index = Mathf.RoundToInt(_sampleFPSList.Count / 2);
			if (index < _sampleFPSList.Count)
				_medianFramesPerSec = _sampleFPSList[index];

#if SHOW_FPS
			Debug.LogFormat("FPSMonitor:FPSCoroutine - _medianFramesPerSec={0}", _medianFramesPerSec);
#endif
		}

#if SHOW_FPS

		private void OnGUI ()
		{
			if (!_displayFPS) return;
			Rect rect = new Rect (0, 0, 80, 20);
			GUI.Label (rect, string.Format("FPS:{0}",framesPerSec));
		}

#endif
	}

}