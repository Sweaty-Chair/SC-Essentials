using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	/// <summary>
	/// Settings panel in text style.
	/// </summary>
	public class SettingPanel : SingletonPanel<SettingPanel>
	{

		[Header("Graphics")]
		[SerializeField] private Text _graphicsSettingText;

		[Header("Audio")]
		[SerializeField] private Text _musicSettingText;
		[SerializeField] private Text _audioSettingText;

		[Header("Notification")]
		[SerializeField] private Text _notificationSettingText;

		public override void Init()
		{
			SettingManager.uiToggled += Toggle;
			SettingManager.graphicsQualityChanged += SetGraphicsSettings;
			SettingManager.musicToggled += SetMusicSetting;
			SettingManager.soundToggled += SetAudioSetting;
			NotificationsManager.toggled += SetNotificationSetting;
		}

		public override void Reset()
		{
			SettingManager.uiToggled -= Toggle;
			SettingManager.graphicsQualityChanged -= SetGraphicsSettings;
			SettingManager.musicToggled -= SetMusicSetting;
			SettingManager.soundToggled -= SetAudioSetting;
			NotificationsManager.toggled -= SetNotificationSetting;
		}

		private void Start()
		{
			SetGraphicsSettings(SettingManager.currentGraphicsSetting);
			SetMusicSetting(SettingManager.isMusicEnabled);
			SetAudioSetting(SettingManager.isSoundEnabled);
			SetNotificationSetting(NotificationsManager.isEnabled);
		}

		#region Graphics

		private void SetGraphicsSettings(SettingManager.GraphicsSetting graphicsSettings)
		{
			if (_graphicsSettingText != null)
				_graphicsSettingText.text = graphicsSettings.ToString();
		}

		#endregion

		#region Audio

		private void SetMusicSetting(bool enable)
		{
			if (_musicSettingText != null)
				_musicSettingText.text = LocalizeUtils.Get(TermCategory.Setting, enable ? "On" : "Off");
		}

		private void SetAudioSetting(bool enable)
		{
			if (_audioSettingText != null)
				_audioSettingText.text = LocalizeUtils.Get(TermCategory.Setting, enable ? "On" : "Off");
		}

		#endregion

		#region Notification

		private void SetNotificationSetting(bool enable)
		{
			if (_notificationSettingText != null)
				_notificationSettingText.text = LocalizeUtils.Get(TermCategory.Setting, enable ? "On" : "Off");
		}

		#endregion

		#region Button Controls

		public void OnGraphicsHigherClick()
		{
			SettingManager.SetHigherGraphicsQuality();
		}

		public void OnGraphicsLowerClick()
		{
			SettingManager.SetLowerGraphicsQuality();
		}

		public void OnMusicToggleClick()
		{
			SettingManager.ToggleMusic();
		}

		public void OnAudioToggleClick()
		{
			SettingManager.ToggleSound();
		}

		public void OnNotificationToggleClick()
		{
			NotificationsManager.Toggle();
		}

		public virtual void OnDefaultClick()
		{
			SettingManager.SetDefaultGraphicsQuality();
			SettingManager.ToggleMusic(true);
			SettingManager.ToggleSound(true);
		}

		public override void OnBackClick()
		{
			SettingManager.ToggleUI(false);
		}

		#endregion

	}

}