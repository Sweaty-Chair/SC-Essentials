using UnityEditor;
using UnityEngine;

namespace SweatyChair
{

	public class TimeScaleManagerPlayback : EditorWindow
	{

		#region Variables

		private const int _minTimescale = 0;
		private const int _maxTimescale = 5;

		#endregion

		#region Show Window

		[MenuItem("Window/Sweaty Chair/TimeScale Playback Controller")]
		public static void ShowWindow()
		{
			TimeScaleManagerPlayback w = GetWindow<TimeScaleManagerPlayback>();
			w.Show();
		}

		private void OnEnable()
		{
			titleContent = new GUIContent("Playback", EditorGUIUtility.FindTexture("Animation.Play"));
			minSize = new Vector2(175, 45);
			maxSize = minSize;
		}

		private void OnDisable()
		{
			TimeScaleManager.SetTimescaleModifier(1);   //Reset our timescale on enable
		}

		#endregion

		#region On GUI

		private void OnGUI()
		{
			if (Application.isPlaying) {                //If Application is playing, display our controls, otherwise draw our error message
				DrawPlaybackControls();
			} else {
				DrawGameNeedsToBePlayingMessage();
			}
		}

		private void DrawGameNeedsToBePlayingMessage()
		{
			EditorGUILayout.HelpBox("Game needs to be running to use this tool.", MessageType.Error, true);
		}

		private void DrawPlaybackControls()
		{
			EditorGUILayout.BeginVertical();

			TimeScaleManager.SetTimescaleModifier(EditorGUILayout.Slider(TimeScaleManager.timeScaleModifier, _minTimescale, _maxTimescale));

			//Button Area
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button(EditorGUIUtility.FindTexture("Animation.FirstKey"))) {
				TimeScaleManager.SetTimescaleModifier(_minTimescale);  //Set Speed to 0
			}

			if (GUILayout.Button(EditorGUIUtility.FindTexture("Animation.PrevKey"))) {
				TimeScaleManager.SetTimescaleModifier(Mathf.Clamp(TimeScaleManager.timeScaleModifier - 0.25f, _minTimescale, _maxTimescale));  //Step Speed backwards by .25 times
																																			   //TimeScaleManager.SetTimescale(ref playbackControlID, 99999, Mathf.Clamp(TimeScaleManager.timeScale - , _minTimescale, _maxTimescale));
			}

			if (GUILayout.Button(EditorGUIUtility.FindTexture("Animation.Play"))) {
				TimeScaleManager.SetTimescaleModifier(1);  //Set Speed to 1
			}

			if (GUILayout.Button(EditorGUIUtility.FindTexture("Animation.NextKey"))) {
				TimeScaleManager.SetTimescaleModifier(Mathf.Clamp(TimeScaleManager.timeScaleModifier + 0.25f, _minTimescale, _maxTimescale));  //Step Speed backwards by .25 times
																																			   //TimeScaleManager.SetTimescale(ref playbackControlID, 99999, Mathf.Clamp(TimeScaleManager.timeScale + 0.25f, _minTimescale, _maxTimescale));
			}

			if (GUILayout.Button(EditorGUIUtility.FindTexture("Animation.LastKey"))) {
				TimeScaleManager.SetTimescaleModifier(_maxTimescale);  //Set speed to 2
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}

		#endregion

	}

}
