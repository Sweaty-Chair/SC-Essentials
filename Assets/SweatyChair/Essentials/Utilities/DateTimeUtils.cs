using UnityEngine;
using System;

namespace SweatyChair
{

	public static class DateTimeUtils
	{

		private const int DAY_TO_SECONDS = 86400;

		/// <summary>
		/// Returns the next day 00:00:00.
		/// </summary>
		public static DateTime nextDay => Now().AddSeconds(DAY_TO_SECONDS - Now().TimeOfDay.TotalSeconds);

		/// <summary>
		/// Converts a binary string to a DateTime object.
		/// </summary>
		public static DateTime BinaryStringToDateTime(string s)
		{
			return DateTime.FromBinary(Convert.ToInt64(s));
		}

		/// <summary>
		/// Converts a DateTime object to a binary string.
		/// </summary>
		public static string ToBinaryString(this DateTime value)
		{

			return value.ToBinary().ToString();
		}

		/// <summary>
		/// Converts a DateTime object to a formatted string.
		/// </summary>
		// dd: two digit day; MM: two digit month; HH: two digit hour, 24 hour clock; mm: tow digit minuts; ss: two digit second.
		// hh: two digit hour, 12 hour clock.
		public static string GetFormatStringByDateTime(DateTime value)
		{
			return value.ToString("ddMMyyyyHHmmss");
		}

		/// <summary>
		/// Converts a formatted date string to a DateTime object.
		/// </summary>
		public static DateTime FormattedStringToDateTime(string s)
		{
			return DateTime.ParseExact(s, "ddMMyyyyHHmmss", null);
		}

		/// <summary>
		/// Converts a binary string array to a DateTime object array.
		/// </summary>
		public static DateTime[] GetDateTimesFromFormatStrings(string[] strArray)
		{
			if (strArray == null)
				return null;
			DateTime[] results = new DateTime[strArray.Length];
			for (int i = 0, imax = strArray.Length; i < imax; i++)
				results[i] = BinaryStringToDateTime(strArray[i]);
			return results;
		}

		/// <summary>
		/// Converts a binary string array to a DateTime object array with added hours on each.
		/// </summary>
		public static DateTime[] GetFutureHoursDateTimes(string[] strArray, int hours)
		{
			if (strArray == null)
				return null;
			DateTime[] results = new DateTime[strArray.Length];
			for (int i = 0, imax = strArray.Length; i < imax; i++) {
				DateTime dt = BinaryStringToDateTime(strArray[i]);
				results[i] = ValidateFutureHours(dt, hours);
			}
			return results;
		}

		/// <summary>
		/// Converts a binary string array to a DateTime object array with added minutes on each.
		/// </summary>
		public static DateTime[] GetFutureMinutesDateTimes(string[] strArray, int minutes)
		{
			if (strArray == null)
				return null;
			DateTime[] results = new DateTime[strArray.Length];
			for (int i = 0, imax = strArray.Length; i < imax; i++) {
				DateTime dt = BinaryStringToDateTime(strArray[i]);
				results[i] = ValidateFutureMinutes(dt, minutes);
			}
			return results;
		}

		/// <summary>
		/// Converts a binary string array to a DateTime object array with added seconds on each.
		/// </summary>
		public static DateTime[] GetFutureSecondsDateTimes(string[] strArray, int seconds)
		{
			if (strArray == null)
				return null;
			DateTime[] results = new DateTime[strArray.Length];
			for (int i = 0, imax = strArray.Length; i < imax; i++) {
				DateTime dt = BinaryStringToDateTime(strArray[i]);
				results[i] = ValidateFutureSeconds(dt, seconds);
			}
			return results;
		}

		/// <summary>
		/// Returns the DateTime object corresponding to key in the preference file if it exists.
		/// </summary>
		public static DateTime GetPlayerPrefs(string key, DateTime? defaultValue = null)
		{
			if (PlayerPrefs.HasKey(key))
				return BinaryStringToDateTime(PlayerPrefs.GetString(key));
			if (defaultValue == null)
				return Now();
			return (DateTime)defaultValue;
		}

		/// <summary>
		/// Returns the DateTime object corresponding to key in the preference file if it exists, and ensures it's a past time.
		/// </summary>
		public static DateTime GetPastPlayerPrefs(string key, DateTime? defaultValue = null)
		{
			DateTime dt = GetPlayerPrefs(key, defaultValue);
			return ValidateIsPast(dt);
		}

		/// <summary>
		/// Returns the DateTime object corresponding to key in the preference file if it exists, and ensures it's a future time for at least X hours.
		/// </summary>
		/// <returns>The future hours player prefs.</returns>
		public static DateTime GetFutureHoursPlayerPrefs(string key, int hours, DateTime? defaultValue = null)
		{
			DateTime dt = GetPlayerPrefs(key, defaultValue);
			return ValidateFutureHours(dt, hours);
		}

		/// <summary>
		/// Returns the DateTime object corresponding to key in the preference file if it exists, and ensures it's a future time for at least X minutes.
		/// </summary>
		/// <returns>The future minutes player prefs.</returns>
		public static DateTime GetFutureMinutesPlayerPrefs(string key, int minutes, DateTime? defaultValue = null)
		{
			DateTime dt = GetPlayerPrefs(key, defaultValue);
			return ValidateFutureMinutes(dt, minutes);
		}

		/// <summary>
		/// Returns the DateTime object corresponding to key in the preference file if it exists, and ensures it's a future time for at least X seconds.
		/// </summary>
		public static DateTime GetFutureSecondsPlayerPrefs(string key, int seconds, DateTime? defaultValue = null)
		{
			DateTime dt = GetPlayerPrefs(key, defaultValue);
			return ValidateFutureSeconds(dt, seconds);
		}

		/// <summary>
		/// Returns a DateTime object arrays corresponding to key in the perferce file if it exists.
		/// </summary>
		public static DateTime[] GetPlayerPrefsX(string key, DateTime defaultValue = default, int defaultSize = 0)
		{
			return GetDateTimesFromFormatStrings(PlayerPrefsX.GetStringArray(key, defaultValue.ToBinaryString(), defaultSize));
		}

		/// <summary>
		/// Returns a DateTime object arrays corresponding to key in the perferce file if it exists, and ensures they are future times for X seconds.
		/// </summary>
		public static DateTime[] GetFutureSecondsPlayerPrefsX(string key, int seconds, DateTime? defaultValue = null, int defaultSize = 0)
		{
			if (defaultValue == null)
				defaultValue = Now();
			return GetFutureSecondsDateTimes(PlayerPrefsX.GetStringArray(key, ((DateTime)defaultValue).ToBinaryString(), defaultSize), seconds);
		}

		/// <summary>
		/// Sets a DateTime object into the perference file identified by key.
		/// </summary>
		public static void SetPlayerPrefs(string key, DateTime value)
		{
			PlayerPrefs.SetString(key, value.ToBinaryString());
		}

		/// <summary>
		/// Sets current time into the perference file identified by key.
		/// </summary>
		public static DateTime SetPlayerPrefsToNow(string key)
		{
			DateTime dt = Now();
			SetPlayerPrefs(key, dt);
			return dt;
		}

		/// <summary>
		/// Sets a future time X days from now into the perference file identified by key.
		/// </summary>
		public static DateTime SetPlayerPrefsDaysFromNow(string key, int days)
		{
			DateTime dt = Now().AddDays(days);
			SetPlayerPrefs(key, dt);
			return dt;
		}

		/// <summary>
		/// Sets a future time X hours from now into the perference file identified by key.
		/// </summary>
		public static DateTime SetPlayerPrefsHoursFromNow(string key, int hours)
		{
			DateTime dt = Now().AddHours(hours);
			SetPlayerPrefs(key, dt);
			return dt;
		}

		/// <summary>
		/// Sets a future time X minutes from now into the perference file identified by key.
		/// </summary>
		public static DateTime SetPlayerPrefsMinutesFromNow(string key, int minutes)
		{
			DateTime dt = Now().AddMinutes(minutes);
			SetPlayerPrefs(key, dt);
			return dt;
		}

		/// <summary>
		/// Sets a future time X seconds from now into the perference file identified by key.
		/// </summary>
		public static DateTime SetPlayerPrefsSecondsFromNow(string key, int seconds)
		{
			DateTime dt = Now().AddSeconds(seconds);
			SetPlayerPrefs(key, dt);
			return dt;
		}

		/// <summary>
		/// Sets a array of DateTime object into the perference file identified by key.
		/// </summary>
		public static void SetPlayerPrefsX(string key, DateTime[] value)
		{
			if (value == null || value.Length == 0)
				PlayerPrefs.DeleteKey(key);
			string[] strArray = new string[value.Length];
			for (int i = 0, imax = value.Length; i < imax; i++)
				strArray[i] = value[i].ToBinaryString();
			PlayerPrefsX.SetStringArray(key, strArray);
		}

		/// <summary>
		/// Gets the interval between two DateTime objects.
		/// </summary>
		public static TimeSpan GetInterval(DateTime start, DateTime end)
		{
			return end.Subtract(start);
		}

		/// <summary>
		/// Converts a TimeSpan object into a simplfied human readable format string in 1 word.
		/// </summary>
		public static string ToSimplifiedDisplayString(this TimeSpan timeSpan)
		{
			string textFormat = string.Empty;
			if (timeSpan.GetYears() > 0) {
				textFormat = timeSpan.GetYears() > 1 ? "{0} Years" : "{0} Year";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, (int)timeSpan.GetYears());
			} else if (timeSpan.GetMonths() > 0) {
				textFormat = timeSpan.GetMonths() > 1 ? "{0} Months" : "{0} Month";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, (int)timeSpan.GetMonths());
			} else if (timeSpan.Days > 0) {
				textFormat = timeSpan.TotalHours > 1 ? "{0} Days" : "{0} Day";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, (int)timeSpan.Days);
			} else if (timeSpan.Hours > 0) {
				textFormat = timeSpan.Hours > 1 ? "{0} Hours" : "{0} Hour";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Hours);
			} else if (timeSpan.Minutes > 0) {
				textFormat = timeSpan.Minutes > 1 ? "{0} Minutes" : "{0} Minute";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Minutes);
			} else if (timeSpan.Seconds > 0) {
				textFormat = timeSpan.Seconds > 1 ? "{0} Seconds" : "{0} Second";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Seconds);
			} else if (timeSpan.Milliseconds >= 0) {
				textFormat = timeSpan.Milliseconds > 1 ? "{0} Milliseconds" : "{0} Millisecond";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Milliseconds);
			} else {
				return LocalizeUtils.Get(TermCategory.Default, "Completed!");
			}
		}

		/// <summary>
		/// Converts a TimeSpan object into a human readable format string.
		/// </summary>
		public static string ToDisplayString(this TimeSpan timeSpan, bool highDetail = false)
		{
			string textFormat = string.Empty;
			if (timeSpan.Days > 0) {

				if (highDetail) {
					textFormat = "{0}d {1}h {2}m {3}s {4}ms";
					return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
				}

				textFormat = "{0}h {1}min";
				if (timeSpan.Minutes == 0)
					textFormat = "{0}h";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, (int)timeSpan.TotalHours, timeSpan.Minutes);

			} else if (timeSpan.Hours > 0) {

				if (highDetail) {
					textFormat = "{0}h {1}m {2}s";
					return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				}

				textFormat = "{0}h {1}min";
				if (timeSpan.Minutes == 0)
					textFormat = "{0}h";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Hours, timeSpan.Minutes);

			} else if (timeSpan.Minutes > 0) {

				textFormat = "{0}m {1}s";
				if (timeSpan.Seconds == 0)
					textFormat = "{0}m";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Minutes, timeSpan.Seconds);

			} else if (timeSpan.Seconds > 0) {

				textFormat = "{0}s";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Seconds);

			} else if (timeSpan.Milliseconds >= 0) {

				textFormat = "{0}ms";
				return LocalizeUtils.GetFormat(TermCategory.Default, textFormat, timeSpan.Milliseconds);

			} else {

				return LocalizeUtils.Get(TermCategory.Default, "Completed!");

			}
		}

		/// <summary>
		/// Gets the years of a given TimeSpan object.
		/// </summary>
		public static int GetYears(this TimeSpan timespan)
		{
			return (int)(timespan.Days / 365.2425);
		}

		/// <summary>
		/// Gets the months of a given TimeSpan object.
		/// </summary>
		public static int GetMonths(this TimeSpan timespan)
		{
			return (int)(timespan.Days / 30.436875);
		}

		/// <summary>
		/// Gets the current UTC time.
		/// </summary>
		public static DateTime UtcNow()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return DateTime.UtcNow; // Just use DateTime.UtcNow when debuging in Editor and not playing
#endif
			DateTime utcNow = UnbiasedTime.Instance.UtcNow();
			if (utcNow.Year <= 2016) {
#if UNITY_EDITOR || DEBUG
				Debug.LogErrorFormat("DateTimeUtils:UTCNow - Failed: UnbiasedTime.Instance.UtcNow={0}, DateTime.UtcNow={1}", utcNow, DateTime.UtcNow);
#endif
				utcNow = DateTime.UtcNow;
			}
			return utcNow;
		}

		/// <summary>
		/// Gets the current time.
		/// </summary>
		public static DateTime Now()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return DateTime.Now; // Just use DateTime.Now when debuging in Editor and not playing
#endif
			DateTime dateNow = UnbiasedTime.Instance.Now();
			if (dateNow.Year <= 2016) {
#if UNITY_EDITOR || DEBUG
				Debug.LogErrorFormat("TimeManager- UnbiasedTime failed:{0} using default DateTime:{1}", dateNow, DateTime.Now);
#endif
				dateNow = DateTime.Now;
			}
			return dateNow;
		}

		/// <summary>
		/// Gets the seconds from a future DateTime.
		/// </summary>
		public static double GetSecondsFromNow(this DateTime futureDateTime)
		{
			double result = (futureDateTime - Now()).TotalSeconds;
			return result > 0 ? result : 0;
		}

		/// <summary>
		/// Gets the seconds from a pased DateTime.
		/// </summary>
		public static double GetSecondsToNow(this DateTime pastDateTime)
		{
			double result = (Now() - pastDateTime).TotalSeconds;
			return result > 0 ? result : 0;
		}

		/// <summary>
		/// Gets the seconds from a future DateTime.
		/// </summary>
		public static double GetMinutesFromNow(this DateTime futureDateTime)
		{
			double result = (futureDateTime - Now()).TotalMinutes;
			return result > 0 ? result : 0;
		}

		/// <summary>
		/// Gets the seconds from a pased DateTime.
		/// </summary>
		public static double GetMinutesToNow(this DateTime pastDateTime)
		{
			double result = (Now() - pastDateTime).TotalMinutes;
			return result > 0 ? result : 0;
		}

		/// <summary>
		/// Gets the seconds from a future DateTime.
		/// </summary>
		public static double GetHoursFromNow(this DateTime futureDateTime)
		{
			double result = (futureDateTime - Now()).TotalHours;
			return result > 0 ? result : 0;
		}

		/// <summary>
		/// Gets the seconds from a pased DateTime.
		/// </summary>
		public static double GetHoursToNow(this DateTime pastDateTime)
		{
			double result = (Now() - pastDateTime).TotalHours;
			return result > 0 ? result : 0;
		}

		/// <summary>
		/// Gets the TimeSpan from a future DateTime.
		/// </summary>
		public static TimeSpan GetTimeSpanFromNow(this DateTime futureDateTime)
		{
			return futureDateTime - Now();
		}

		/// <summary>
		/// Gets the TimeSpan from a pased DateTime.
		/// </summary>
		public static TimeSpan GetTimeSpanToNow(this DateTime pastDateTime)
		{
			return Now() - pastDateTime;
		}

		/// <summary>
		/// Gets the TimeSpan from a future UTC DateTime.
		/// </summary>
		public static TimeSpan GetTimeSpanFromUtcNow(this DateTime futureUtcDateTime)
		{
			return futureUtcDateTime - UtcNow();
		}

		/// <summary>
		/// Gets the TimeSpan from a future UTC DateTime.
		/// </summary>
		public static TimeSpan GetTimeSpanToUtcNow(this DateTime pastUtcDateTime)
		{
			return UtcNow() - pastUtcDateTime;
		}

		/// <summary>
		/// Gets the DateTime object after seconds from now.
		/// </summary>
		public static DateTime GetDateTimeAfterSeconds(double seconds)
		{
			return Now().AddSeconds(seconds);
		}

		/// <summary>
		/// Gets the DateTime object after minutes from now.
		/// </summary>
		public static DateTime GetDateTimeAfterMinutes(double minutes)
		{
			return Now().AddMinutes(minutes);
		}

		/// <summary>
		/// Checks if a DateTime object is in past time.
		/// </summary>
		public static bool IsPast(DateTime dt)
		{
			return Now() > dt;
		}

		/// <summary>
		/// Checks if a DateTime object is in future time.
		/// </summary>
		public static bool IsFuture(DateTime dt)
		{
			return Now() < dt;
		}

		/// <summary>
		/// Checks if a Date is in the past. Useful for checking if we are on a new day
		/// </summary>
		/// <returns></returns>
		public static bool IsPastDate(DateTime dt)
		{
			return dt.Date < Now().Date;
		}

		/// <summary>
		/// Checks if a Date is in the future.
		/// </summary>
		/// <returns></returns>
		public static bool IsFutureDate(DateTime dt)
		{
			return dt.Date > Now().Date;
		}

		/// <summary>
		/// Ensures a DateTime object is in past time.
		/// PlayerPrefs saved DateTime has wired chance to be set to be a far future DateTime after a crash, validate here
		/// </summary>
		public static DateTime ValidateIsPast(DateTime dt)
		{
			if (IsFuture(dt))
				return Now();
			return dt;
		}

		/// <summary>
		/// Ensures a DateTime object is in future time in X hours.
		/// </summary>
		public static DateTime ValidateFutureHours(DateTime dt, int hours)
		{
			if (GetHoursFromNow(dt) > hours)
				return Now().AddHours(hours);
			return dt;
		}

		/// <summary>
		/// Ensures a DateTime object is in future time in X minutes.
		/// </summary>
		public static DateTime ValidateFutureMinutes(DateTime dt, int minutes)
		{
			if (GetMinutesFromNow(dt) > minutes)
				return Now().AddMinutes(minutes);
			return dt;
		}

		/// <summary>
		/// Ensures a DateTime object is in future time in X seconds.
		/// </summary>
		public static DateTime ValidateFutureSeconds(DateTime dt, int seconds)
		{
			if (GetSecondsFromNow(dt) > seconds)
				return Now().AddSeconds(seconds);
			return dt;
		}

		/// <summary>
		/// Try parse a nullable DateTime object into a DateTime object.
		/// </summary>
		public static DateTime Parse(DateTime? dateTime, DateTime defaultValue = default(DateTime))
		{
			return dateTime == null ? defaultValue : (DateTime)dateTime;
		}

		/// <summary>
		/// Try parse a nullable long value from epoch time into a DateTime object.
		/// </summary>
		public static DateTime ParseEpoch(long? epochMilliseconds, DateTime defaultValue = default(DateTime))
		{
			if (epochMilliseconds == null)
				return defaultValue;
			return (new DateTime(1970, 1, 1)).AddMilliseconds((long)epochMilliseconds);
		}

		/// <summary>
		/// Converts an unbiased time to local machine time.
		/// </summary>
		public static DateTime UnbiasedToLocalDateTime(this DateTime unbiasedDateTime)
		{
			return DateTime.Now.Add(unbiasedDateTime - Now());
		}

		/// <summary>
		/// Converts a local machine time to an unbiased time.
		/// </summary>
		public static DateTime LocalToUnbiasedDateTime(this DateTime localDateTime)
		{
			return Now().Add(localDateTime - DateTime.Now);
		}

		/// <summary>
		/// Converts a server UTC time to a local machine time.
		/// </summary>
		public static DateTime ServerUtcToLocalDateTime(this DateTime serverUtcDateTime)
		{
			return DateTime.Now.Add(serverUtcDateTime - UtcNow());
		}

		/// <summary>
		/// Converts a local machine time to a server UTC time.
		/// </summary>
		public static DateTime LocalToServerUtcDateTime(DateTime localDateTime)
		{
			return UtcNow().Add(localDateTime - DateTime.Now);
		}

	}

}