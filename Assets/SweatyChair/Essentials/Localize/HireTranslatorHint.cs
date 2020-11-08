using UnityEngine;
using SweatyChair.UI;

namespace SweatyChair
{

	/// <summary>
	/// A generic hint for hiring translator.
	/// </summary>
	public class LanguageHireTranslatorHint : MonoBehaviour
	{

		[SerializeField] private GameObject _hintButton;

		private void Awake()
		{
			LocalizeUtils.languageChanged += Start;
		}

		private void OnDestroy()
		{
			LocalizeUtils.languageChanged -= Start;
		}

		private void Start()
		{
#if CHS || UNITY_WEBGL || OFFLINE
			_hintButton.SetActive(false); // No hint button on Chinese, WebGL and offline build
#else
			_hintButton.SetActive(LocalizeUtils.currentLanguage != Language.ChineseTraditional && LocalizeUtils.currentLanguage != Language.ChineseSimplified);
#endif
		}

		public void OnHintClick()
		{
			new Message {
				format = MessageFormat.Notice,
				content = "Missing your language or the translation is so horrible? We are sorry but why not consider doing it for us? We hire passionate gamers for translation. We pay cash or in-game currency, drop us an email at chicka@sweatychair.com",
			}.Show();
		}

	}

}