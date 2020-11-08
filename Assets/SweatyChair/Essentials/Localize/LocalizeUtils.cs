using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SweatyChair
{

	public enum Language
	{
		English,
		Spanish,
		French,
		Italian,
		Portuguese,
		German,
		Turkish,
		Polish,
		Russian,
		Ukrainian,
		ChineseTraditional,
		ChineseSimplified,
		Japanese,
		Korean,
		Vietnamese,
		Thai,
		Mongolian,
		Arabic
	}

	public enum TermCategory
	{
		Default,
		Tutorial,
		Menu,
		Game,
		Ending,
		Setting,
		Message,
		Notification,
		Button,
		Loading,
		Shop,
		Leaderboard,
		Achievement,
		Social,
		Card,
		Consumable,
		Avatar,
		Weapon,
		Enemy,
		Rank,
		Cargo,
		Hint,
		Guild,
		Resource,
		World,
		Technology,
		Mission,
		RedeemCode,
		Dungeon,
		Story,
		Intro,
		Inventory,
		Clothing,
		Chest,
		Player,
		Login,
		Editor,
		Review,
		InGameMission
	}

	/// <summary>
	/// A wrapper for using I2 Localization.
	/// </summary>
	public static class LocalizeUtils
	{

		public static event UnityAction languageChanged;

		public static string systemLanguage {
			get {
				string tmp = Application.systemLanguage.ToString();
				if (tmp == "ChineseSimplified")
					tmp = "Chinese (Simplified)";
				if (tmp == "ChineseTraditional")
					tmp = "Chinese (Traditional)";
				return tmp;
			}
		}

		public static Language currentLanguage {
			get {
				string langString = I2.Loc.LocalizationManager.CurrentLanguage;
				if (langString == "Chinese (Simplified)")
					return Language.ChineseSimplified;
				else if (langString == "Chinese (Traditional)")
					return Language.ChineseTraditional;
				if (EnumUtils.IsDefined<Language>(langString))
					return EnumUtils.Parse<Language>(langString);
				return Language.English;
			}
		}

		public static string currentLanguageString => LanguageToString(currentLanguage);

		public static string currentLanguageCode => I2.Loc.LocalizationManager.CurrentLanguageCode;

		/// <summary>
		/// Sets the current lanauge to a given language enum.
		/// </summary>
		/// <param name="language">A language enum.</param>
		public static void SetLanguage(Language language)
		{
			SetLanguage(LanguageToString(language));
		}

		/// <summary>
		/// Sets the current lanauge to a given language string.
		/// </summary>
		/// <param name="language">A language string.</param>
		public static void SetLanguage(string language)
		{
			if (string.IsNullOrEmpty(language))
				return;
			I2.Loc.LocalizationManager.CurrentLanguage = language;
			languageChanged?.Invoke();
		}

		/// <summary>
		/// Converts a Language enum to string.
		/// </summary>
		/// <param name="language">A Language enum</param>
		/// <returns>A language string.</returns>
		public static string LanguageToString(this Language language)
		{
			switch (language) {
				case Language.ChineseSimplified:
					return "Chinese (Simplified)";
				case Language.ChineseTraditional:
					return "Chinese (Traditional)";
				default:
					return language.ToString();
			}
		}

		/// <summary>
		/// Gets a categorized term with a category and a term string.
		/// </summary>
		/// <param name="category">A category for the term.</param>
		/// <param name="term">A composite format term.</param>
		/// <returns>A categorized term string.</returns>
		public static string GetTerm(TermCategory category, object term)
		{
			if (category == TermCategory.Default)
				return term.ToString();
			return string.Format("{0}/{1}", category, term);
		}

		/// <summary>
		/// Checks if a categorized term exists in term source.
		/// </summary>
		/// <param name="term">A composite format term.</param>
		/// <param name="category">A category for the term.</param>
		public static bool HasTerm(TermCategory category, object term)
		{
			return HasTerm(GetTerm(category, term));
		}

		/// <summary>
		/// Checks if a categorized term exists in term source.
		/// </summary>
		/// <param name="categorizedTerm">A term that already in category format.</param>
		public static bool HasTerm(string categorizedTerm)
		{
			return I2.Loc.LocalizationManager.TryGetTranslation(categorizedTerm, out string dump);
		}

		/// <summary>
		/// Gets a localized string with a category and a term into specific language.
		/// </summary>
		/// <param name="term">A composite format term.</param>
		/// <returns>A localized string.</returns>
		public static string Localize(this string term)
		{
			return Get(TermCategory.Default, term);
		}

		/// <summary>
		/// Gets a localized string with a category and a term into specific language.
		/// </summary>
		/// <param name="term">A composite format term.</param>
		/// <param name="category">A category for the term.</param>
		/// <returns>A localized string.</returns>
		public static string Localize(this string term, TermCategory category)
		{
			return Get(category, term);
		}

		/// <summary>
		/// Gets a localized string with a category and a term into specific language.
		/// </summary>
		/// <param name="term">A composite format term.</param>
		/// <param name="language">A language to be localized to.</param>
		/// <returns>A localized string.</returns>
		public static string Localize(this string term, Language language)
		{
			return Get(TermCategory.Default, term, language);
		}

		/// <summary>
		/// Gets a localized string with a category and a term into specific language.
		/// </summary>
		/// <param name="term">A composite format term.</param>
		/// <param name="category">A category for the term.</param>
		/// <param name="language">A language to be localized to.</param>
		/// <returns>A localized string.</returns>
		public static string Localize(this string term, TermCategory category, Language language)
		{
			return Get(category, term, language);
		}

		/// <summary>
		/// Gets a localized string with a category and a term into specific language.
		/// </summary>
		/// <param name="category">A category for the term.</param>
		/// <param name="term">A composite format term.</param>
		/// <param name="language">A language to be localized to.</param>
		/// <returns>A localized string.</returns>
		public static string Get(TermCategory category, object term, Language language)
		{
			if (term == null || string.IsNullOrEmpty(term.ToString()))
				return string.Empty;
			string categorizedTerm = GetTerm(category, term);
			string tmp = Get(categorizedTerm);
			if (tmp == categorizedTerm)
				return Get(term.ToString(), language);
			return tmp;
		}

		/// <summary>
		/// Gets a localized string with a category and a term.
		/// </summary>
		/// <param name="category">A category for the term.</param>
		/// <param name="term">A composite format term.</param>
		/// <returns>A localized string.</returns>
		public static string Get(TermCategory category, object term)
		{
			if (term == null || string.IsNullOrEmpty(term.ToString()))
				return string.Empty;
			string categorizedTerm = GetTerm(category, term);
			string tmp = Get(categorizedTerm);
			if (tmp == categorizedTerm)
				return Get(term.ToString());
			return tmp;
		}

		/// <summary>
		/// Gets a localized string with a categorized term.
		/// </summary>
		/// <param name="categorizedTerm">A term that already in category format.</param>
		/// <returns>A localized string.</returns>
		public static string Get(string categorizedTerm)
		{
			if (string.IsNullOrEmpty(categorizedTerm))
				return string.Empty;
			string tmp = I2.Loc.LocalizationManager.GetTranslation(categorizedTerm);
			if (string.IsNullOrEmpty(tmp))
				return categorizedTerm;
			return tmp;
		}

		/// <summary>
		/// Gets a localized string with a categorized term into specific language.
		/// </summary>
		/// <param name="categorizedTerm">A term that already in category format.</param>
		/// <param name="language">A language to be localized to.</param>
		/// <returns>A localized string.</returns>
		public static string Get(string categorizedTerm, Language language)
		{
			if (string.IsNullOrEmpty(categorizedTerm))
				return string.Empty;
			string tmp = I2.Loc.LocalizationManager.GetTranslation(categorizedTerm, overrideLanguage: LanguageToString(language));
			if (string.IsNullOrEmpty(tmp))
				return categorizedTerm;
			return tmp;
		}

		/// <summary>
		/// Gets a localized string with the string representation of a corresponding object in a specific array.
		/// </summary>
		/// <param name="term">A composite format term.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A localized string.</returns>
		public static string LocalizeFormat(this string term, params object[] args)
		{
			return GetFormat(TermCategory.Default, term, args);
		}

		/// <summary>
		/// Gets a localized string with the string representation of a corresponding object in a specific array.
		/// </summary>
		/// <param name="term">A composite format term.</param>
		/// <param name="category">A category for the term.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A localized string.</returns>
		public static string LocalizeFormat(this string term, TermCategory category, params object[] args)
		{
			return GetFormat(category, term, args);
		}

		/// <summary>
		/// Gets a localized string with the string representation of a corresponding object in a specific array.
		/// </summary>
		/// <param name="category">A category for the term.</param>
		/// <param name="term">A composite format term.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A localized string.</returns>
		public static string GetFormat(TermCategory category, object term, params object[] args)
		{
			try {
				return string.Format(Get(category, term), args);
			} catch {
				Debug.LogErrorFormat("LocalizeUtils:LocalizeFormat - FormatException at term={0}", Get(category, term));
				return Get(term as string);
			}
		}

		/// <summary>
		/// Gets a localized string with the string representation of a corresponding object in a specific array.
		/// </summary>
		/// <param name="term">A composite format term.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A localized string.</returns>
		public static string GetFormat(object term, params object[] args)
		{
			return GetFormat(TermCategory.Default, term, args);
		}

		/// <summary>
		/// Gets the localized plural form of a term.
		/// </summary>
		/// <param name="category">A category for the term.</param>
		/// <param name="term">A term text.</param>
		/// <param name="num">A number count of the term.</param>
		/// <returns>A localized string.</returns>
		public static string GetPlural(TermCategory category, object term, int num = 2)
		{
			return Get(category, term.ToString().ToPlural(num));
		}

		/// <summary>
		/// Gets the localized plural form of a term.
		/// </summary>
		/// <param name="term">A term text.</param>
		/// <param name="num">A number count of the term.</param>
		/// <returns>A localized string.</returns>
		public static string GetPlural(object term, int num = 2)
		{
			string pluralTerm = term.ToString().ToPlural(num);
			string localizedPluralTerm = Get(pluralTerm);
			if (pluralTerm == localizedPluralTerm) // For languages not having pural, e.g. Chinese
			localizedPluralTerm = Get(term as string);
			return localizedPluralTerm;
		}

		/// <summary>
		/// Sets the term for the UGUI Text component.
		/// </summary>
		/// <param name="text">An UGUI Text component.</param>
		/// <param name="category">A category for the term.</param>
		/// <param name="term">A term text.</param>
		public static void SetLocalizeText(this Text text, TermCategory category, string term)
		{
			var localize = text.GetComponent<I2.Loc.Localize>();
			if (localize != null)
				localize.Term = GetTerm(category, term);
		}

		/// <summary>
		/// A special function to localizes a sale percentage in Chinese.
		/// </summary>
		/// <param name="percentage">A sale percentage in integer</param>
		/// <returns>The string of the localized sale percentage text</returns>
		public static string GetSalePercentageString(int percentage)
		{
			if (I2.Loc.LocalizationManager.CurrentLanguage.Contains("Chinese")) {
				if (percentage % 10 == 0)
					return ((100 - percentage) / 10).ToString();
				else if (percentage > 90)
					return GetFormat("Less than {0}", 1);
				return (100 - percentage).ToString();
			}
			return percentage.ToString();
		}

	}

}