using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
#if TextMeshPro
using TMPro;
#endif

namespace SweatyChair.UI
{

	/// <summary>
	/// Settings panel in dropdown style.
	/// </summary>
	public class SettingDropdownPanel : SingletonPanel<SettingPanel>
	{

#if TextMeshPro
		[Header("Graphics")]
		[SerializeField] private TMP_Dropdown _graphicsSettingDropdown;
#endif

		[Header("Audio")]
		[SerializeField] private Dropdown _musicSettingDropdown;
		[SerializeField] private Dropdown _audioSettingDropdown;

		public override void Init()
		{
			
			SettingManager.uiToggled += Toggle;
			SettingManager.graphicsQualityChanged += SetGraphicsSettings;
			SettingManager.musicToggled += SetMusicSetting;
			SettingManager.soundToggled += SetAudioSetting;
		}

		public override void Reset()
		{
			SettingManager.uiToggled -= Toggle;
			SettingManager.graphicsQualityChanged -= SetGraphicsSettings;
			SettingManager.musicToggled -= SetMusicSetting;
			SettingManager.soundToggled -= SetAudioSetting;
		}

		private void Start()
		{
			InitDropdown();
			SetGraphicsSettings(SettingManager.currentGraphicsSetting);
			SetMusicSetting(SettingManager.isMusicEnabled);
			SetAudioSetting(SettingManager.isSoundEnabled);
		}

		#region Graphics

		protected virtual void InitDropdown()
		{
#if TextMeshPro
			if (_graphicsSettingDropdown != null) {
				_graphicsSettingDropdown.options.Clear();
				List<string> graphicSettings = EnumUtils.GetValues<SettingManager.GraphicsSetting>()
				.Select(value => LocalizeUtils.Get(TermCategory.Setting, value.ToString())).ToList();
				_graphicsSettingDropdown.AddOptions(graphicSettings);
			}
#endif

			if (_musicSettingDropdown != null) {
				_musicSettingDropdown.options.Clear();
				_musicSettingDropdown.AddOptions(new List<string> { LocalizeUtils.Get(TermCategory.Setting, "Off"), LocalizeUtils.Get(TermCategory.Setting, "On") });
			}

			if (_audioSettingDropdown != null) {
				_audioSettingDropdown.options.Clear();
				_audioSettingDropdown.AddOptions(new List<string> { LocalizeUtils.Get(TermCategory.Setting, "Off"), LocalizeUtils.Get(TermCategory.Setting, "On") });
			}
		}

		private void SetGraphicsSettings(SettingManager.GraphicsSetting graphicsSettings)
		{
#if TextMeshPro
			if (_graphicsSettingDropdown != null)
				_graphicsSettingDropdown.value = (int)graphicsSettings;
#endif
		}

		#endregion

		#region Audio

		private void SetMusicSetting(bool enable)
		{
			if (_musicSettingDropdown != null)
				_musicSettingDropdown.value = enable ? 1 : 0;
		}

		private void SetAudioSetting(bool enable)
		{
			if (_audioSettingDropdown != null)
				_audioSettingDropdown.value = enable ? 1 : 0;
		}

		#endregion

		#region Button Controls

		public void OnGraphicsDropdownSelect(int value)
		{
			SettingManager.SetGraphicsQuality((SettingManager.GraphicsSetting)value);
		}

		public void OnMusicDropdownSelect(int value)
		{
			SettingManager.ToggleMusic(value > 0);
		}

		public void OnAudioDropdownSelect(int value)
		{
			SettingManager.ToggleSound(value > 0);
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