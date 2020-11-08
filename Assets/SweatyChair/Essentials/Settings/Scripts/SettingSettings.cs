using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A Settings class for common game settings, sucha as graphic etc.
	/// </summary>
	[CreateAssetMenu(fileName = "SettingSettings", menuName = "Sweaty Chair/Settings/Setting")]
	public class SettingSettings : ScriptableObjectSingleton<SettingSettings>
	{

		[Header("Graphics")]
		[Tooltip("Default framerate to target when opening app for the first time")]
		public int defaultMaxFramerate = -1;

		[Tooltip("Toggle for using render texture mapping")]
		public SettingManager.GraphicsSetting desultGraphicsSetting = SettingManager.GraphicsSetting.Best;
		[Tooltip("Default Fullscreen mode when starting the app for the first time")]
		public FullScreenMode defaultFullscreenMode = FullScreenMode.FullScreenWindow;

		[Tooltip("Manual screen width in Windows, 0 to disable, used for standalone only")]
		public int manualScreenWidthWin;
		[Tooltip("Manual screen width in macOS, 0 to disable, used for standalone only")]
		public int manualScreenWidthOsx = 1920;

		public int manualScreenWidth =>
#if UNITY_STANDALONE_WIN
				manualScreenWidthWin;
#elif UNITY_STANDALONE_OSX
				manualScreenWidthOsx;
#else
				0;
#endif

		[Header("Others")]
		public string facebookPageURL = "https://www.facebook.com/sweatychair";

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Sweaty Chair/Settings/Setting")]
		private static void OpenSettings()
		{
			current.OpenAssetsFile();
		}
#endif

	}

}