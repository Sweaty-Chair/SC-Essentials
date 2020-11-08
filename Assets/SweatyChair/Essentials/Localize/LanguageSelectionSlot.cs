using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	/// <summary>
	/// A language selection button listing page.
	/// </summary>
	public class LanguageSelectionSlot : Slot<string>
	{

		[System.Serializable]
		private class LanguageFont
		{
			public Language language;
			public Font font;
		}

		[SerializeField] private string _defaultFontTerm = "KidSans";
		[SerializeField] private LanguageFont[] _languageFonts;
		[SerializeField] private I2.Loc.Localize _languageLocalize;
		[SerializeField] private GameObject _activatedGO;

		public string language { get; private set; }

		protected override void Awake()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		public override void Set(string language)
		{
			this.language = language;
			_languageLocalize.Term = LocalizeUtils.GetTerm(TermCategory.Setting, language);

			// Other languages do not look good with current lanaguage font. Change the font to its corresponding language.
			// I2 didn't cache the fonts in other languages, so we have to manually set it
			if (language != "System Language") {
				_languageLocalize.SecondaryTerm = "";
				foreach (LanguageFont lf in _languageFonts) {
					if (lf.language.LanguageToString() == language)
						_languageLocalize.GetComponent<Text>().font = lf.font;
				}
			}
		}

		public void OnClick()
		{
			GetComponentInParent<LanguageSelectionPanel>().OnLanguageClick(language);
		}

		public void SetActivate(bool activated)
		{
			_activatedGO.SetActive(activated);
		}

	}

}