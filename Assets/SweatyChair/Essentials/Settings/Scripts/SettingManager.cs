using SweatyChair.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

	/// <summary>
	/// A master manager that contols in-game settings.
	/// </summary>
	public static partial class SettingManager
	{

		private const string PREFS_MUSIC_ENABLED = "MusicEnabled";
		private const string PREFS_SOUND_ENABLED = "SoundEnabled";
		private const string PREFS_FACEBOOK_PAGE_PAGE_OPENED = "FacebookPageOpened";

		public static event UnityAction<bool> uiToggled;

		static SettingManager()
		{
			InitGraphics();
			InitMusic();
			InitSound();
		}

		// Empty function to call the static constructor only
		public static void Init() { }

		public static void ToggleUI()
		{
			ToggleUI(!PanelManager.IsShown<SettingPanel>());
		}

		public static void ToggleUI(bool doShow)
		{
			uiToggled?.Invoke(doShow);
		}

		#region Music

		public static event UnityAction<bool> musicToggled;

		public static bool isMusicEnabled { get; private set; }

		public static void InitMusic()
		{
			ToggleMusic(PlayerPrefs.GetInt(PREFS_MUSIC_ENABLED, 1) == 1); // Default on
		}

		public static void ToggleMusic()
		{
			ToggleMusic(!isMusicEnabled);
		}

		public static void ToggleMusic(bool enabled)
		{
			isMusicEnabled = enabled;
			PlayerPrefs.SetInt(PREFS_MUSIC_ENABLED, isMusicEnabled ? 1 : 0);
			musicToggled?.Invoke(isMusicEnabled);
			SetMusic();
		}

		public static void MuteMusic()
		{
			SoundManager.MuteMusic(true);
		}

		public static void SetMusic()
		{
			SoundManager.MuteMusic(!isMusicEnabled);
		}

		#endregion

		#region Sound

		public static event UnityAction<bool> soundToggled;

		public static bool isSoundEnabled { get; private set; }

		public static void InitSound()
		{
			isSoundEnabled = PlayerPrefs.GetInt(PREFS_SOUND_ENABLED, 1) == 1; // Default on
			ToggleSound(isSoundEnabled);
		}

		public static void ToggleSound()
		{
			ToggleSound(!isSoundEnabled);
		}

		public static void ToggleSound(bool enabled)
		{
			isSoundEnabled = enabled;
			PlayerPrefs.SetInt(PREFS_SOUND_ENABLED, isSoundEnabled ? 1 : 0);
			soundToggled?.Invoke(isSoundEnabled);
			SetSound();
		}

		public static void SetSound()
		{
			SoundManager.MuteSFX(!isSoundEnabled);
		}

		#endregion

		#region Graphics Setting

		public enum GraphicsSetting
		{
			Fastest = 0,
			Fast = 1,
			Good = 2,
			Best = 3
		}

		private const string PREFS_GRAPHICS_SETTING = "GraphicsSetting";
		private const string PREFS_AUTO_CALIBRATION_COMPLETED = "AutoCalibrationCompleted";
		private const string PREFS_FULLSCREEN_MODE = "FullscreenMode";
		private const string PREFS_TARGET_DISPLAY = "UnitySelectMonitor"; // Internal Unity Magic string, Check SetTargetDisplay for more info
		private const string PREFS_VSYNC_MODE = "VsyncMode";
		private const string PREFS_MAX_FRAMERATE = "maxFramerate";

		private static int FALLBACK_FPS_GOOD = 15; // If FPS lower than 15, set graphics to Good
		private static int FALLBACK_FPS_FAST = 5; // If FPS lower than 5, set graphics to Fast 

		private static int MIN_FRAMERATE = 5;

		public static event UnityAction<GraphicsSetting> graphicsQualityChanged; // Called when graphics quality is changed manuually or by auto calibrate
		public static event UnityAction autoCalibrateStarted; // Called when the auto calibrate started, use this for disable camera, show fader, etc
		public static event UnityAction autoCalibrateCompleted; // Called when the auto calibrate completed, use this for re-enable camera, hide fader, etc

		public static event UnityAction<FullScreenMode> fullScreenModeChanged; // Called when our fullscreen mode is changed
		public static event UnityAction<int> targetDisplayChanged; // Called when our target display is changed
		public static event UnityAction<bool> vsyncChanged; // Called when our vsync setting changes

		public static event UnityAction<int> maxTargetFramerateChanged;
		public static event UnityAction<int> targetFramerateChanged;

		public static GraphicsSetting currentGraphicsSetting = GraphicsSetting.Best;

		public static FullScreenMode currentFullscreenMode { get { return Screen.fullScreenMode; } }
		public static int currentTargetDisplay { get { return PlayerPrefs.GetInt(PREFS_TARGET_DISPLAY, 0); } }          // Default target display should be 0 (main monitor) no matter what
		public static bool currentVsyncOn { get { return (QualitySettings.vSyncCount == 0) ? false : true; } }

		public static int currentMaxTargetFramerate = -1;  // Target highest possible platform framerate by default
		public static int currentTargetFramerate = -1;  // Target framerate we are attempting to hit. Cache this so if our max framerate increases we can immediately hit it

		public static float screenRatio { get; private set; } // Note that this can be changed for standalone resizable windows, do not rely on it

		public static bool isAutoCalibrationCompleted => PlayerPrefs.HasKey(PREFS_AUTO_CALIBRATION_COMPLETED);

		public static void InitGraphics()
		{
			// Fullscreen
			Screen.fullScreen = true;
			LoadFullscreenMode();

			// Graphics Settings
			int graphicIndex = PlayerPrefs.GetInt(PREFS_GRAPHICS_SETTING, (int)SettingSettings.current.desultGraphicsSetting);
			currentGraphicsSetting = (GraphicsSetting)Mathf.Clamp(graphicIndex, 0, EnumUtils.GetCount<GraphicsSetting>() - 1);
			screenRatio = (float)Screen.width / Screen.height;

			// Framerate
			LoadMaxFramerate();
			SetFramerate(currentTargetFramerate); // Try to set for the highest framerate possible

			Debug.LogFormat("SettingManager:InitGraphics - Screen.width={0}, Screen.height={1}, screenRatio={2}", Screen.width, Screen.height, screenRatio);
			SetGraphicsQuality();
		}

		/// <summary>
		/// Start auto calibrate, this would be mannually call at fresh install, while nothing is loading.
		/// </summary>
		public static void AutoCalibrateGraphicsSetting()
		{
			if (!isAutoCalibrationCompleted) {
				SetGraphicsQuality(GraphicsSetting.Best); // Set graphics to best and start a stretch test there
				FPSMonitor.Start();
				TimeManager.Invoke(ConfirmAutoCalibrateGraphicsSetting, FPSMonitor.sampleDuration);
				autoCalibrateStarted?.Invoke();
			}
		}

		private static void ConfirmAutoCalibrateGraphicsSetting()
		{
			int fps = FPSMonitor.medianFramesPerSec;
			Debug.LogFormat("SettingManager:AutoCalibrateGraphicsSetting - fps={0}", fps);

			currentGraphicsSetting = GraphicsSetting.Best;
			if (fps <= FALLBACK_FPS_GOOD)
				currentGraphicsSetting = GraphicsSetting.Good;
			else if (fps <= FALLBACK_FPS_FAST)
				currentGraphicsSetting = GraphicsSetting.Fast;

			SetGraphicsQuality();

			PlayerPrefs.SetInt(PREFS_AUTO_CALIBRATION_COMPLETED, 1);

			autoCalibrateCompleted?.Invoke();
		}

		public static void SetDefaultGraphicsQuality()
		{
			currentGraphicsSetting = SettingSettings.current.desultGraphicsSetting;
			SetGraphicsQuality();
			SaveGraphicsSetting();
		}

		public static void SetGraphicsQuality(GraphicsSetting graphicsSetting)
		{
			currentGraphicsSetting = graphicsSetting;
			SetGraphicsQuality();
		}

		public static void SetLowerGraphicsQuality()
		{
			if (currentGraphicsSetting <= 0)
				currentGraphicsSetting = (GraphicsSetting)EnumUtils.GetCount<GraphicsSetting>() - 1;
			else
				currentGraphicsSetting--;
			SetGraphicsQuality();
		}

		public static void SetHigherGraphicsQuality()
		{
			if ((int)currentGraphicsSetting >= EnumUtils.GetCount<GraphicsSetting>() - 1)
				currentGraphicsSetting = GraphicsSetting.Fastest;
			else
				currentGraphicsSetting++;
			SetGraphicsQuality();
		}

		public static void ToggleGraphicsQuality()
		{
			currentGraphicsSetting++;
			if ((int)currentGraphicsSetting >= EnumUtils.GetCount<GraphicsSetting>())
				currentGraphicsSetting = 0; // Loop to Fastest
			SetGraphicsQuality();
		}

		private static void SetGraphicsQuality()
		{
			SetResolution();
			TimeManager.Invoke(SetQualitySettings, 1); // Wait 1 second to avoid crash on low-end devices
		}

		private static void SetResolution()
		{
#if UNITY_IOS || UNITY_ANDROID
			// No Matter what we want to update Scren resolution to match aspect ratio
			bool isLandscapeOrientation = (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight);
			Vector2Int newResolutionOfDevice = new Vector2Int(Screen.width, Screen.height);
			if (isLandscapeOrientation) {
				if (Screen.width > 1280) {
					float downScaleAmnt = 1280f / Screen.width;
					newResolutionOfDevice = new Vector2Int(Mathf.RoundToInt(Screen.width * downScaleAmnt), Mathf.RoundToInt(Screen.height * downScaleAmnt));
				}
			} else {
				if (Screen.height > 1280) {
					float downScaleAmnt = 1280f / Screen.height;
					newResolutionOfDevice = new Vector2Int(Mathf.RoundToInt(Screen.width * downScaleAmnt), Mathf.RoundToInt(Screen.height * downScaleAmnt));
				}
			}
			Screen.SetResolution(newResolutionOfDevice.x, newResolutionOfDevice.y, true);
#elif UNITY_STANDALONE
			if (SettingSettings.current.manualScreenWidth > 0)
				Screen.SetResolution(SettingSettings.current.manualScreenWidth, Mathf.RoundToInt(1f * SettingSettings.current.manualScreenWidth * Screen.currentResolution.height / Screen.currentResolution.width), Screen.fullScreen);
#endif
		}

		public static void SetQualitySettings()
		{
			QualitySettings.SetQualityLevel((int)currentGraphicsSetting, true);
			graphicsQualityChanged?.Invoke(currentGraphicsSetting);
			TimeManager.Invoke(SaveGraphicsSetting, 3); // Save in 3 seconds, in case of crashing, app won't be launched with the crashed setting next time
		}

		public static void SaveGraphicsSetting()
		{
			PlayerPrefs.SetInt(PREFS_GRAPHICS_SETTING, (int)currentGraphicsSetting);
		}

		#region Set Max Target Framerate

		public static void SetMaxFramerate(int maxFramerate)
		{
			if (maxFramerate != currentMaxTargetFramerate && maxFramerate > 0) {

				currentMaxTargetFramerate = maxFramerate;

				// If our current framerate is more than our max, we re-set our framerate
				SetFramerate(currentTargetFramerate);

				maxTargetFramerateChanged?.Invoke(currentMaxTargetFramerate);

				// Save our changes after 3 seconds in case of crashing, app won't be launched with crashed settings next time
				TimeManager.Invoke(SaveCurrentMaxFramerate, 3);
			}
		}

		public static void SaveCurrentMaxFramerate()
		{
			PlayerPrefs.SetInt(PREFS_MAX_FRAMERATE, currentMaxTargetFramerate);
		}

		public static void LoadMaxFramerate()
		{
			// Load an existing max framerate from our settings, otherwise use our defaul max framerate
			currentMaxTargetFramerate = PlayerPrefs.GetInt(PREFS_MAX_FRAMERATE, SettingSettings.current.defaultMaxFramerate);
		}

		#endregion

		#region Set Framerate

		public static void SetFramerate(int framerate)
		{
			int modifiedFramerate = framerate;

			// If our framerate target is -1, but our max is not -1, we need to set our framerate to something which will clamp correctly.
			if (modifiedFramerate == -1 && currentMaxTargetFramerate != -1)
				modifiedFramerate = 5000;

			// Clamp our framerate if we have a max defined
			if (currentMaxTargetFramerate != -1 && modifiedFramerate != -1)
				modifiedFramerate = Mathf.Clamp(modifiedFramerate, MIN_FRAMERATE, currentMaxTargetFramerate);

			// If our framerate is more than our cap, or is not the same as the last framerate we have targeted
			if (framerate != currentTargetFramerate || framerate != Application.targetFrameRate || modifiedFramerate != currentTargetFramerate) {

				// We set our target framerate to our unmodified framerate
				currentTargetFramerate = framerate;

				// We then set our target framerate to our modified framerate so we preserve our max framerate changes
				Application.targetFrameRate = modifiedFramerate;

				targetFramerateChanged?.Invoke(framerate);
			}
		}

		#endregion

		#region FullScreen Mode Settings

		public static void SetFullscreenMode(FullScreenMode fullscreenMode)
		{
			// Only Execute this on standalone platforms which support fullscreen modes
#if UNITY_STANDALONE || UNITY_EDITOR
			if (fullscreenMode != currentFullscreenMode) {
				// Set our fullscreen mode and save this data to our player prefs
				Screen.fullScreenMode = fullscreenMode;
				fullScreenModeChanged?.Invoke(currentFullscreenMode);
				// Save our changes after 3 seconds in case of crashing, app won't be launched with crashed setting next time
				TimeManager.Invoke(SaveFullscreenModeSetting, 3);
			}
#endif
		}

		/// <summary>
		/// Save the full screen settings into PlayerPrefs.
		/// </summary>
		public static void SaveFullscreenModeSetting()
		{
			PlayerPrefs.SetInt(PREFS_FULLSCREEN_MODE, (int)currentFullscreenMode);
		}

		/// <summary>
		/// Load the fullscreen settings saved in PlayerPrefs and set fullscreen or not.
		/// </summary>
		public static void LoadFullscreenMode()
		{
			// Load an existing fullscreen mode from our settings, otherwise use our default screen mode
			int fullScreenModeIndex = PlayerPrefs.GetInt(PREFS_FULLSCREEN_MODE, (int)SettingSettings.current.defaultFullscreenMode);
			SetFullscreenMode((FullScreenMode)fullScreenModeIndex);
		}

		#endregion

		#region Target Display Settings

		/// <summary>
		/// [Requires Restart] Sets our target display to render our game onto.
		/// </summary>
		public static void SetTargetDisplay(int displayIndex)
		{
			// Unity does not have proper support for this at the moment, so we rely on the information from this post,
			// When this no longer works, migrate to the new system
			// Forum post here https://forum.unity.com/threads/switch-monitor-at-runtime.501336/
#if UNTIY_EDITOR || UNITY_STANDALONE
			// If we try to set our display index to something larger than what we can support, reset it back to our current target display
			displayIndex = (displayIndex > Display.displays.Length) ? currentTargetDisplay : displayIndex;
			// If our display is still larger than we can support, then we reset our index back to 0
			displayIndex = (displayIndex > Display.displays.Length) ? 0 : displayIndex;
			if (displayIndex != currentTargetDisplay) {
				PlayerPrefs.SetInt(PREFS_TARGET_DISPLAY, displayIndex);
				// Then finally invoke our event
				targetDisplayChanged?.Invoke(displayIndex);
			}
#endif
		}

		#endregion

		#region V-Sync Settings

		public static void SetVSyncMode(bool vSyncEnabled)
		{
			// Only Execute this on standalone platforms which support Vsync properly for the moment
#if UNITY_STANDALONE || UNITY_EDITOR
			if (vSyncEnabled != currentVsyncOn) {
				// Set our Vsync count to 0 if not enabled, or to every vBlank if enabled
				QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
				vsyncChanged?.Invoke(vSyncEnabled);
				// Save our changes after 3 seconds in case of crashing, app won't be launched with crashed setting next time
				TimeManager.Invoke(SaveFullscreenModeSetting, 3);
			}
#endif
		}

		public static void SaveCurrentVSyncMode()
		{
			PlayerPrefs.SetString(PREFS_VSYNC_MODE, DataUtils.BoolToString(currentVsyncOn));
		}

		public static void LoadVSyncMode()
		{
			// Load an existing vsync mode from our settings, otherwise use no vsync
			bool vSyncMode = DataUtils.GetBool(PlayerPrefs.GetString(PREFS_VSYNC_MODE, "0"));
			SetVSyncMode(vSyncMode);
		}

		#endregion

		#endregion

		#region Facebook Page

		public static bool hasOpenedFacebookPage {
			get { return PlayerPrefs.GetInt(PREFS_FACEBOOK_PAGE_PAGE_OPENED) == 1; }
			set { PlayerPrefs.SetInt(PREFS_FACEBOOK_PAGE_PAGE_OPENED, value ? 1 : 0); }
		}

		public static void OpenFacebookPage()
		{
			Application.OpenURL(SettingSettings.current.facebookPageURL);
			hasOpenedFacebookPage = true;
		}

		#endregion

		#region Facebook Link

		public static event UnityAction facebookLinkToggledEvent;

		public static void ToggleFacebookLink()
		{

		}

		#endregion

		#region Studio

#if UNITY_IOS || UNITY_TVOS
		private const string URL_STORE_STUDIO = "itms://itunes.apple.com/developer/sweatychair-pty.-ltd./id781132111";
#else
		private const string URL_STORE_STUDIO = "https://play.google.com/store/apps/developer?id=Sweaty+Chair+Studio";
#endif
		private const string URL_WEBSITE_GAMES = "https://www.sweatychair.com#games";

		public static void OpenStoreStudioPage()
		{
			Application.OpenURL(URL_STORE_STUDIO);
		}

		public static void OpenWebsite()
		{
			Application.OpenURL(URL_WEBSITE_GAMES);
		}

		#endregion

		#region Video Ads

		public static event UnityAction<bool> videoAdsToggledEvent;

		public static void ToggleVideoAds()
		{
			if (AdsManager.isRewardedVideoEnabled) {
#if EASY_MOBILE
				EasyMobile.NativeUI.AlertPopup alert = EasyMobile.NativeUI.ShowTwoButtonAlert(
					LocalizeUtils.Get(TermCategory.Setting, "Disable Video Ads?"),
					LocalizeUtils.Get(TermCategory.Setting, "We make money from ads. Please leave them on if you can."),
					LocalizeUtils.Get(TermCategory.Setting, "Disable"),
					LocalizeUtils.Get(TermCategory.Setting, "Keep")
				);
				if (alert != null) {
					alert.OnComplete += (buttonIndex) => {
						if (buttonIndex == 0)
							DisableVideoAds();
					};
				}
#else
				DisableVideoAds();
#endif
			} else {
				AdsManager.ToggleRewardedVideo(true);
				OnVideoAdsToggled();
			}
		}

		private static void DisableVideoAds()
		{
			AdsManager.ToggleRewardedVideo(false);
			OnVideoAdsToggled();
		}

		private static void OnVideoAdsToggled()
		{
			if (videoAdsToggledEvent != null)
				videoAdsToggledEvent(AdsManager.isRewardedVideoEnabled);
		}

		#endregion

	}

}