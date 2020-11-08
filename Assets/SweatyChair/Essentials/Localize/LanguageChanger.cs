using UnityEngine;

namespace SweatyChair
{

	// A force set language script that can be used for testing or fixed-language builds.
	public class LanguageChanger : MonoBehaviour
	{

		[SerializeField] private bool _followScriptingSymbol = false;
		[SerializeField] private Language _lanuage = Language.English;
		[SerializeField] private bool _setOnlyOnce = true;

		private void Awake()
		{
			if (_setOnlyOnce) {
				if (PlayerPrefs.GetInt("LanguageChanged") == 1)
					return;
				PlayerPrefs.SetInt("LanguageChanged", 1);
			}
			if (_followScriptingSymbol) {
#if CHS
				LocalizeUtils.SetLanguage(Language.ChineseSimplified);
#elif CHT
				LocalizeUtils.SetLanguage(Language.ChineseTraditional);
#elif KOR
				LocalizeUtils.SetLanguage(Language.Korean);
#endif
			} else {
				LocalizeUtils.SetLanguage(_lanuage);
			}
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Localize/Set to English")]
		private static void SetToEnglish()
		{
			LocalizeUtils.SetLanguage(Language.English);
		}

		[UnityEditor.MenuItem("Debug/Localize/Set to Simplified Chinese")]
		private static void SetToSimplifiedChinese()
		{
			LocalizeUtils.SetLanguage(Language.ChineseSimplified);
		}

		[UnityEditor.MenuItem("Debug/Localize/Set to Korean")]
		private static void SetToKorean()
		{
			LocalizeUtils.SetLanguage(Language.Korean);
		}

#endif

	}

}