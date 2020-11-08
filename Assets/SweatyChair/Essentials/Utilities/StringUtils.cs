using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SweatyChair
{

	#region Enum

	public enum StringFormatType
	{
		None,
		Scientific,
		Latin,
		KMB,
		Alphabet
	}

	#endregion

	public static class StringUtils
	{

		// Contains letter and number
		private static Regex REGEX_NON_DIGIT = new Regex(@"[^\d]");
		private static Regex REGEX_NON_NUMBER = new Regex(@"[^\d\.]");
		private static Regex REGEX_NON_ALPHANUMERIC = new Regex(@"[^a-zA-Z0-9]");
		private static Regex REGEX_ALPHANUMERIC = new Regex(@"[a-zA-Z0-9]");

		// Contains chinese characters
		private const string PATTERN_CHS_CHT = @"[\u4e00-\u9fa5]";
		private static Regex REGEX_CHS_CHT = new Regex(PATTERN_CHS_CHT);
		private const string PATTERN_EN_NUM = @"[a-zA-Z0-9\(\)_\-\.\!\@]";
		private static Regex REGEX_EN_NUM = new Regex(PATTERN_EN_NUM);

		public static int IndexOfNth(this string str, char c, int n)
		{
			int s = -1;
			for (int i = 0; i < n; i++) {
				s = str.IndexOf(c, s + 1);
				if (s == -1)
					break;
			}
			return s;
		}

		public static string StripName(this string name, int maxLength = 16)
		{
			if (name.Length > maxLength) {
				string[] names = name.Split(' ');
				if (names.Length >= 2) { // 2+ words name
					if (names[0].Length > maxLength) {
						return names[0].Substring(0, maxLength - 1) + ".";
					} else if (names[0].Length == maxLength) {
						return names[0];
					} else {
						if (names.Length == 2) {
							return names[0] + " " + names[1].Substring(0, Mathf.Clamp(maxLength - names[0].Length, 0, names[1].Length)) + ".";
						} else {
							if (names[0].Length + names[1].Length > maxLength)
								return names[0] + " " + names[1].Substring(0, Mathf.Clamp(maxLength - names[0].Length, 0, names[1].Length)) + ".";
							else
								return names[0] + " " + names[1].Substring(0, Mathf.Clamp(maxLength - names[0].Length, 0, names[1].Length)) + " " + names[2].Substring(0, Mathf.Clamp(maxLength - names[0].Length - names[1].Length, 0, names[2].Length)) + ".";
						}
					}
				} else { // 1 word name
					return name.Substring(0, maxLength - 1) + ".";
				}
			}
			return name;
		}

		public static string StripAfterLast(this string s, string delimiter = ",")
		{
			int delimiterInd = s.LastIndexOf(delimiter);
			if (delimiterInd >= 0)
				return s.Substring(0, delimiterInd + 1);
			return s;
		}

		public static string StripBeforeLast(this string s, string delimiter = ",")
		{
			int delimiterInd = s.LastIndexOf(delimiter);
			if (delimiterInd >= 0)
				return s.Substring(0, delimiterInd);
			return s;
		}

		public static string StripParenthesis(this string s)
		{
			int parenthesisOpenPos = s.IndexOf("("[0]);
			int parenthesisClosePos = s.IndexOf(")"[0]);
			if (parenthesisOpenPos > 0) {
				string strip = s.Substring(parenthesisOpenPos, parenthesisClosePos - parenthesisOpenPos);
				return s.Replace(strip, "");
			}
			return s;
		}

		public static string StripSpaces(this string s)
		{
			return s.Replace(" ", "");
		}

		public static string SubstringAfterLast(this string s, string delimiter = ",")
		{
			int delimiterInd = s.LastIndexOf(delimiter);
			if (delimiterInd >= 0)
				return s.Substring(delimiterInd + delimiter.Length);
			return s;
		}

		public static string SubstringBeforeLast(this string s, string delimiter = ",")
		{
			int delimiterInd = s.LastIndexOf(delimiter);
			if (delimiterInd >= 0)
				return s.Substring(0, delimiterInd);
			return s;
		}

		public static string ToUCFirst(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return "";
			return char.ToUpper(s[0]) + s.Substring(1);
		}

		public static string ToSingular(this string s)
		{
			if (s.LastIndexOf("ies") == s.Length - 3)
				return s.Substring(0, s.Length - 3);
			if (s.LastIndexOf("es") == s.Length - 2)
				return s.Substring(0, s.Length - 2);
			if (s.LastIndexOf("s") == s.Length - 1)
				return s.Substring(0, s.Length - 1);
			return s;
		}

		public static string ToPlural(this string s, int num = 2)
		{
			if (num <= 1)
				return s; // No need pural

			if (s.LastIndexOf("y") == s.Length - 1)
				return s.Substring(0, s.Length - 1) + "ies";
			if (s.LastIndexOf("th") == s.Length - 2)
				return s + "es";
			if (s.LastIndexOf("s") == s.Length - 1 || s.LastIndexOf("es") == s.Length - 2) // Skip if string already ended with -s or -es
				return s;
			return s + "s";
		}

		public static string SecondsToMinutesStr(float seconds)
		{
			return "" + Mathf.FloorToInt(seconds / 60).ToString("D2") + ":" + Mathf.RoundToInt(seconds % 60).ToString("D2");
		}

		public static string SecondsToHoursStr(float seconds)
		{
			return "" + Mathf.FloorToInt(seconds / 3600).ToString("D2") + ":" + Mathf.FloorToInt((seconds % 3600) / 60).ToString("D2") + ":" + Mathf.RoundToInt(seconds % 60).ToString("D2");
		}

		public static string ToRomanString(this int i)
		{
			switch (i) {
				case 0:
					return "I";
				case 1:
					return "II";
				case 2:
					return "III";
				default:
					return string.Empty;
			}
		}

		public static string MultiplerString(this int i)
		{
			switch (i) {
				case 2:
					return "Double";
				case 3:
					return "Triple";
				case 4:
					return "Quadruple";
				case 5:
					return "Quintuple";
			}
			return "";
		}

		public static string MarkMystery(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;
			return REGEX_ALPHANUMERIC.Replace(s, "?");
		}

		// Strip all characters except digits (0~9)
		public static string StripNonDigit(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;
			return REGEX_NON_DIGIT.Replace(s, "");
		}

		// Try prase to the first appeared integer
		public static int ParseToInt(this string s, int defaultValue = 0)
		{
			int result = defaultValue;
			int.TryParse(StripNonDigit(s), out result);
			return result;
		}

		// Try prase to the first appeared decimal
		public static decimal ParseToDecimal(this string s, decimal defaultValue = 0)
		{
			decimal result = defaultValue;
			decimal.TryParse(StripNonNumber(s), out result);
			return result;
		}

		// Strip all characters except digits (0~9 AND .)
		public static string StripNonNumber(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;
			return REGEX_NON_NUMBER.Replace(s, "");
		}

		public static string StripNonAlphanumeric(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;
			return REGEX_NON_ALPHANUMERIC.Replace(s, "");
		}

		public static bool IsChinese(this string s)
		{
			return Regex.IsMatch(s, PATTERN_CHS_CHT);
		}

		public static bool ContainChinese(this string s)
		{
			return REGEX_CHS_CHT.IsMatch(s);
		}

		public static bool IsEngOrNum(this string s)
		{
			return Regex.IsMatch(s, PATTERN_EN_NUM);
		}

		public static bool IsValidIP(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return false;
			System.Net.IPAddress address;
			if (System.Net.IPAddress.TryParse(s, out address)) {
				switch (address.AddressFamily) {
					case System.Net.Sockets.AddressFamily.InterNetwork:
					case System.Net.Sockets.AddressFamily.InterNetworkV6:
						return true;
					default:
						return false;
				}
			}
			return false;
		}

		public static bool ContainEngOrNum(this string s)
		{
			return REGEX_EN_NUM.IsMatch(s);
		}

		public static string CamelCaseToSpace(this string s)
		{
			return Regex.Replace(s, "(\\B[A-Z])", " $1");
		}

		public static string TrimSpaces(this string s)
		{
			return s.Replace(" ", "");
		}

		// Use string builder to build string by string array is good for performance, for 10+ strings.
		public static string BuildString(string[] strings)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < strings.Length; i++)
				sb.Append(strings[i]);
			return sb.ToString();
		}

		public static string ObjectToString(object obj)
		{
			if (obj is IList)
				return ListToString((IList)obj);
			if (obj is IDictionary)
				return DictionaryToString((IDictionary)obj);
			if (obj is object[])
				return ListToString((object[])obj);
			return obj.ToString();
		}

		public static string ArrayToString<T>(T[] array, string seperator = ",")
		{
			if (array == null)
				return "null";
			if (array.Length <= 0)
				return "[]";
			string tmp = "[";
			foreach (T t in array)
				tmp += (t == null ? "null" : t.ToString()) + seperator;
			return tmp.Substring(0, tmp.Length - 1) + "]";
		}

		public static string ArrayToString(object[] array, string seperator = ",")
		{
			if (array == null)
				return "null";
			if (array.Length <= 0)
				return "[]";
			string tmp = "[";
			foreach (object o in array)
				tmp += (o == null ? "null" : o.ToString()) + seperator;
			return tmp.Substring(0, tmp.Length - 1) + "]";
		}


		// Convert a List to a string, for debug purpose
		public static string ListToString(IList list, string seperator = ",")
		{
			if (list == null)
				return "null";
			if (list.Count <= 0)
				return "[]";
			string tmp = "[";
			foreach (object o in list)
				tmp += (o == null ? "null" : o.ToString()) + seperator;
			return tmp.Substring(0, tmp.Length - 1) + "]";
		}

		// Convert a Dictionary to a string, for debug purpose
		public static string DictionaryToString(IDictionary dict)
		{
			if (dict == null)
				return "null";
			if (dict.Count <= 0)
				return "[]";
			string tmp = "[";
			foreach (DictionaryEntry de in dict) {
				if (de.Value is IList)
					tmp += string.Format("{{{0}:{1}}},", de.Key, ListToString(de.Value as IList));
				else if (de.Value is IDictionary)
					tmp += string.Format("{{{0}:{1}}},", de.Key, DictionaryToString(de.Value as IDictionary));
				else
					tmp += string.Format("{{{0}:{1}}},", de.Key, de.Value);
			}
			return tmp.Substring(0, tmp.Length - 1) + "]";
		}

		// Convert a Collection to a string, for debug purpose
		public static string CollectionToString(ICollection coll, string seperator = ",")
		{
			if (coll == null)
				return "null";
			if (coll.Count <= 0)
				return "[]";
			string tmp = "[";
			foreach (object o in coll)
				tmp += (o == null ? "null" : o.ToString()) + seperator;
			return tmp.Substring(0, tmp.Length - 1) + "]";
		}

		public static string ToRed(this string txt)
		{
			return ToColor(txt, "ff0000");
		}

		public static string ToGreen(this string txt)
		{
			return ToColor(txt, "00ff00");
		}

		public static string ToBlue(this string txt)
		{
			return ToColor(txt, "0000ff");
		}

		public static string ToBlack(this string txt)
		{
			return ToColor(txt, "000000");
		}

		public static string ToColor(this string txt, string color)
		{
//#if NGUI // TODO: Backward compatiable for No Humanity using NGUI (Sep20)
//			return string.Format("[{0}]{1}[-]", color, txt);
//#else
			return string.Format("<color=#{0}>{1}</color>", color, txt);
//#endif
		}

		public static void BestFitHeight(this UnityEngine.UI.Text text)
		{
			RectTransform rectTransform = text.rectTransform;

			float canvasScaleFactor = 1;
			UnityEngine.UI.CanvasScaler canvasScaler = text.GetComponentInParent<UnityEngine.UI.CanvasScaler>();
			if (canvasScaler)
				canvasScaleFactor = canvasScaler.scaleFactor;

			// Current text settings
			TextGenerationSettings settings = text.GetGenerationSettings(rectTransform.rect.size);
			settings.generateOutOfBounds = false;
			settings.scaleFactor = canvasScaleFactor; // HACK: scale factor of settings not following the global scale factor... make sure it do

			float vecticalOffset = text.fontSize / 2;

			// Preferred text rect height
			float preferredHeight = (new TextGenerator().GetPreferredHeight(text.text, settings) / canvasScaleFactor) + vecticalOffset;

			// Current text rect height
			float currentHeight = rectTransform.rect.height;

			// Force resize
			if (Mathf.Abs(currentHeight - preferredHeight) > Mathf.Epsilon)
				rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, preferredHeight);
		}

		public static int CountLines(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return 0;

			int index = -1;
			int count = 0;
			while (-1 != (index = str.IndexOf(Environment.NewLine, index + 1, StringComparison.Ordinal)))
				count++;

			return count + 1;
		}

#region Validate Correct URI

		public static bool IsValidWebURL(this string path)
		{
			return Uri.TryCreate(path, UriKind.Absolute, out Uri uriResult)
				&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
		}

#endregion

#region Get Numbers From String

		public static List<int> ExtractIntegersFromString(string input)
		{
			string[] possibleOutputs = Regex.Split(input, @"\D+");
			List<int> outputs = new List<int>();

			// Go through all our regexed strings.
			for (int i = 0; i < possibleOutputs.Length; i++) {
				//Try parse our string to int, and if it succeeds, add it to our list
				int outInt;
				if (int.TryParse(possibleOutputs[i], out outInt)) {
					outputs.Add(outInt);
				}
			}

			// Return our outputs
			return outputs;
		}

#endregion

#region Clipboard

		public static void CopyToClipboard(this string str)
		{
			GUIUtility.systemCopyBuffer = str;
		}

#endregion

#region Double To Display String

#region Const

		private const string DEFAULT_NOTATION_FORMATTING = "0.##";

#endregion

#region String Format Helper

		public static string ToFormattedString(this float value, StringFormatType format)
		{
			return ToFormattedString((double)value, format);
		}
		public static string ToFormattedString(this float value, StringFormatType format, string stringFormat)
		{
			return ToFormattedString((double)value, format, stringFormat);
		}

		public static string ToFormattedString(this double value, StringFormatType format)
		{
			switch (format) {
				default:
					string outVal = value.ToString();
					Debug.LogError("Not Implemented");
					return outVal;

				case StringFormatType.None:
					return value.ToString();

				case StringFormatType.Scientific:
					return value.ToScientificNotationString();

				case StringFormatType.Latin:
					return value.ToLatinNotationString();

				case StringFormatType.KMB:
					return value.ToKMBNotationString();

				case StringFormatType.Alphabet:
					return value.ToAlphabetNotationString();
			}
		}
		public static string ToFormattedString(this double value, StringFormatType format, string stringFormat)
		{
			switch (format) {
				default:
					string outVal = value.ToString(stringFormat);
					Debug.LogError("Not Implemented");
					return outVal;

				case StringFormatType.None:
					return value.ToString(stringFormat);

				case StringFormatType.Scientific:
					return value.ToScientificNotationString(stringFormat);

				case StringFormatType.Latin:
					return value.ToLatinNotationString(stringFormat);

				case StringFormatType.KMB:
					return value.ToKMBNotationString(stringFormat);

				case StringFormatType.Alphabet:
					return value.ToAlphabetNotationString(stringFormat);
			}
		}

#endregion

#region Scientific notation

		public static string ToScientificNotationString(this double input, string format = DEFAULT_NOTATION_FORMATTING)
		{

			// If our number is less than 1 million, do default formatting
			if (input < 1000000)
				return input.ToString(format);

			// Significand
			GetScientificNotation(input, out double significand, out ushort exponent);

			// Then, return our Scientific notation string
			return $"{significand.ToString(format)}e+{exponent}";
		}

#endregion

#region Latin Notation

#region Const

		private static readonly string[] latin = { "mi", "bi", "tri", "quadri", "quinti", "sexti", "septi", "octi", "noni" };
		private static readonly string[] ones = { "un", "duo", "tre", "quattuor", "quinqua", "se", "septe", "octo", "nove" };
		private static readonly string[] tens = { "deci", "viginti", "triginta", "quadraginta", "quinquaginta", "sexaginta", "septuaginta", "octoginta", "nonaginta" };
		private static readonly string[] hundreds = { "centi", "ducenti", "trecenti", "quadringenti", "quingenti", "sescenti", "septingenti", "octingenti", "nongenti" };

#endregion

		public static string ToLatinNotationString(this double input, string format = DEFAULT_NOTATION_FORMATTING)
		{

			// If our number is less than 1 million, do default formatting
			if (input < 1000000) {
				return input.ToString(format);
			}

			// Get our Scientific notation
			GetScientificNotation(input, out double significand, out ushort exponent);

			ushort adjustedExponent = (ushort)((exponent / 3) - 1);
			string prefix = "";
			if (adjustedExponent < 10) {
				prefix = latin[adjustedExponent - 1];
			} else {
				ushort hundredsPlace = (ushort)(adjustedExponent / 100);
				ushort tensPlace = (ushort)((adjustedExponent / 10) % 10);
				ushort onesPlace = (ushort)(adjustedExponent % 10);
				string onesString = (onesPlace > 0) ? ones[onesPlace - 1] : "";
				string modifier = "";
				if ((onesPlace == 7) || (onesPlace == 9)) {
					if (tensPlace > 0) {
						if ((tensPlace == 2) || (tensPlace == 8)) {
							modifier = "m";
						} else if (tensPlace != 9) {
							modifier = "n";
						}
					} else if (hundredsPlace > 0) {
						if (hundredsPlace == 8) {
							modifier = "m";
						} else if (hundredsPlace != 9) {
							modifier = "n";
						}
					}
				}
				if ((onesPlace == 3) || (onesPlace == 6)) {
					if (tensPlace > 0) {
						if ((tensPlace == 2) || (tensPlace == 3) || (tensPlace == 4) || (tensPlace == 5) || (tensPlace == 8)) {
							modifier = ((onesPlace == 6) && (tensPlace == 8)) ? "x" : "s";
						}
					} else if (hundredsPlace > 0) {
						if ((hundredsPlace == 1) || (hundredsPlace == 3) || (hundredsPlace == 4) || (hundredsPlace == 5) || (hundredsPlace == 8)) {
							modifier = ((onesPlace == 6) && ((tensPlace == 1) || (tensPlace == 8))) ? "x" : "s";
						}
					}
				}
				string tensString = (tensPlace > 0) ? tens[tensPlace - 1] : "";
				string hundredsString = (hundredsPlace > 0) ? hundreds[hundredsPlace - 1] : "";
				prefix = string.Format("{0}{1}{2}{3}", onesString, modifier, tensString, hundredsString);
			}
			double adjustedSignificand = significand * Math.Pow(10, exponent % 3);
			double integralPart = Math.Truncate(adjustedSignificand);
			return string.Format("{0} {1}llion", (((adjustedSignificand - integralPart) > 0.99) ? integralPart + 0.99 : adjustedSignificand).ToString(format), prefix.TrimEnd('a'));
		}

#endregion

#region KMB Notation

#region Const

		private static readonly string[] kmbBase = {
		"K", "M", "B", "T", "Qa", "Qt", "Sx", "Sp", "Oc", "No", "Dc", "UDc", "DDc",
		"TDc", "QaDc", "QtDc", "SxDc", "SpDc", "ODc", "NDc", "Vg", "UVg", "DVg", "TVg",
		"QaVg", "QtVg", "SxVg", "SpVg", "OVg", "NVg", "Tg", "UTg", "DTg", "TTg", "QaTg",
		"QtTg", "SxTg", "SpTg", "OTg", "NTg", "Qd", "UQd", "DQd", "TQd", "QaQd", "QtQd",
		"SxQd", "SpQd", "OQd", "NQd", "Qi", "UQi", "DQi", "TQi", "QaQi", "QtQi", "SxQi",
		"SpQi", "OQi", "NQi", "Se", "USe", "DSe", "TSe", "QaSe", "QtSe", "SxSe", "SpSe",
		"OSe", "NSe", "St", "USt", "DSt", "TSt", "QaSt", "QtSt", "SxSt", "SpSt", "OSt",
		"NSt", "Og", "UOg", "DOg", "TOg", "QaOg", "QtOg", "SxOg", "SpOg", "OOg", "NOg",
		"Nn", "UNn", "DNn", "TNn", "QaNn", "QtNn", "SxNn", "SpNn", "ONn", "NNn", "Ce",
		"UCe", "Infinity"
	};

#endregion

		public static string ToKMBNotationString(this double input, string format = DEFAULT_NOTATION_FORMATTING)
		{
			// If our number is less than 1 thousand, do default formatting
			if (input < 1000)
				return input.ToString(format);

			// Get our Scientific notation
			GetScientificNotation(input, out double significand, out ushort exponent);
			ushort adjustedExponent = (ushort)((exponent / 3) - 1);

			// Grab our prefix from our list. It shouldn't be possible to exceed this number, but we log error if we somehow manage to
			if (adjustedExponent >= kmbBase.Length)
				return $"{significand.ToString(format)}Infin";
			else
				return $"{significand.ToString(format)}{kmbBase[adjustedExponent]}";
		}

#endregion

#region Alphabet Notation

#region Const

		private static readonly string[] basicKMB = { "K", "M", "B", "T" };
		private static readonly char[] letters = {
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
		'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
		'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
		};

#endregion

		public static string ToAlphabetNotationString(this double input, string format = DEFAULT_NOTATION_FORMATTING)
		{
			// If our number is less than 1000, do default formatting
			if (input < 1000)
				return input.ToString(format);

			// Get our Scientific notation
			GetScientificNotation(input, out double significand, out ushort exponent);
			ushort adjustedExponent = (ushort)((exponent / 3) - 1);

			string suffix = "";
			// Check which list we are in
			if (adjustedExponent < basicKMB.Length)
				suffix = basicKMB[adjustedExponent];
			else {
				int remainingExponent = (adjustedExponent - basicKMB.Length);

				int letterIndex = remainingExponent % letters.Length;
				char letter = letters[letterIndex];

				int repeats = 2 + Mathf.FloorToInt(remainingExponent / (float)letters.Length);

				suffix = new string(letter, repeats);
			}

			return $"{significand.ToString(format)}{suffix}";
		}

#endregion

#region Helper

		public static void GetScientificNotation(double input, out double significand, out ushort exponent)
		{
			exponent = (ushort)Math.Log10(input);
			significand = input * Math.Pow(0.1, exponent);
		}

#endregion

#endregion

	}

}