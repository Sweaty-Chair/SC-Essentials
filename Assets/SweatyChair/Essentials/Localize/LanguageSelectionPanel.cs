using UnityEngine;
using System.Collections.Generic;
using I2.Loc;

namespace SweatyChair.UI
{

	/// <summary>
	/// A language selection button listing page.
	/// </summary>
	public class LanguageSelectionPanel : Panel
	{

		public const string TEXT_SYSTEM_LANGUAGE = "System Language";

		[SerializeField] private GameObject _languageButtonPrefab;
		[SerializeField] private GameObject _layoutGroupHolderGO;
		[SerializeField] private bool _reloadScene;

		private List<LanguageSelectionSlot> _languageSlots = new List<LanguageSelectionSlot>();

		private void Start()
		{
			List<string> languages = LocalizationManager.GetAllLanguages();

			// Add system language option
			languages.Insert(0, TEXT_SYSTEM_LANGUAGE);

			// Show localized langauges names on drop list, ensure to do localization at the ending
			foreach (string language in languages)
				AddButton(language);

			SetButtonActive(LocalizationManager.CurrentLanguage);
		}

		public void OnLanguageClick(string language)
		{
			if (language == TEXT_SYSTEM_LANGUAGE)
				SelectSystemLanguage();
			else
				LocalizeUtils.SetLanguage(language);
			if (_reloadScene)
				GameSceneManager.ReloadActiveScene();
			else
				SetButtonActive(language);
		}

		private void AddButton(string language)
		{
			GameObject go = _layoutGroupHolderGO.AddChild(_languageButtonPrefab, false);
			LanguageSelectionSlot slot = go.GetComponent<LanguageSelectionSlot>();
			slot.Set(language);
			_languageSlots.Add(slot);
		}

		private void SetButtonActive(string language)
		{
			foreach (LanguageSelectionSlot slot in _languageSlots)
				slot.SetActivate(slot.language == language);
		}

		// Hack of LocalizationManager::SelectStartupLanguage()
		private void SelectSystemLanguage()
		{
			// Check if the device language is supported. 
			string validLanguage = LocalizationManager.GetSupportedLanguage(LocalizeUtils.systemLanguage);
			if (!string.IsNullOrEmpty(validLanguage)) {
				LocalizationManager.CurrentLanguage = validLanguage;
				return;
			}
		}

	}

}