using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Remove the letterbox when switching windows mode to full screen mode, set the windows to manual screen width (if
	/// speicified) when switching full screen mode to windows mode. Standalone only.
	/// </summary>
	public class ScreenModeSwitcher : PersistentSingleton<ScreenModeSwitcher>
	{

#if UNITY_STANDALONE

		private const string PREFS_FULL_SCREEN_WIDTH = "FullScreenWidth";
		private const string PREFS_FULL_SCREEN_HEIGHT = "FullScreenHeight";

		private bool _fullScreen;

		private void Start()
		{
			_fullScreen = Screen.fullScreen;
		}

		private void Update()
		{
			if (_fullScreen != Screen.fullScreen) {
				if (_fullScreen) { // Full screen mode to window mode
					Debug.LogFormat("ScreenModeSwitch - Full screen mode changed to windows mode, Screen.width={0}, Screen.height={1}, Screen.currentResolution={2}", Screen.width, Screen.height, Screen.currentResolution);
					// Set resolution with manual width if specified, But only if our current resolution is larger than our manual width
					if (SettingSettings.current.manualScreenWidth > 0 && (Screen.width > SettingSettings.current.manualScreenWidth))
						Screen.SetResolution(SettingSettings.current.manualScreenWidth, Mathf.RoundToInt(1f * SettingSettings.current.manualScreenWidth * Screen.height / Screen.width), Screen.fullScreen);
					// Save the full screen ratio, because this will be overwritten in window mode if player resize the windows
					PlayerPrefs.SetInt(PREFS_FULL_SCREEN_WIDTH, Screen.width); // Save the screen ratio, so this can be used next time full screen
					PlayerPrefs.SetInt(PREFS_FULL_SCREEN_HEIGHT, Screen.height); // Save the screen ratio, so this can be used next time full screen
				} else { // Windows mode to full screen mode
					Debug.LogFormat("ScreenModeSwitch - Windows mode changed to full screen mode, Screen.width={0}, Screen.height={1}, Screen.currentResolution={2}", Screen.width, Screen.height, Screen.currentResolution);
					// Both Screen.width/height and Screen.currentResolution are set to the windows size, try get the full screen ratio from saved PlayerPrefs value or default it to 16:9
					int fullScreenWidth = PlayerPrefs.GetInt(PREFS_FULL_SCREEN_WIDTH, Screen.width);
					int fullScreenHeight = PlayerPrefs.GetInt(PREFS_FULL_SCREEN_HEIGHT, Screen.height);
					Debug.LogFormat("ScreenModeSwitch - fullScreenWidth={0}, fullScreenHeight={1}", fullScreenWidth, fullScreenHeight);
					if (SettingSettings.current.manualScreenWidth > 0) // Set to resolution relative to mannual width
						Screen.SetResolution(SettingSettings.current.manualScreenWidth, Mathf.RoundToInt(1f * SettingSettings.current.manualScreenWidth  * fullScreenHeight / fullScreenWidth), Screen.fullScreen);
					else // Set to resolution relative to current width
						Screen.SetResolution(fullScreenWidth, fullScreenHeight, Screen.fullScreen);
				}
				_fullScreen = Screen.fullScreen;
			}
		}

#endif

	}

}