using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace SweatyChair
{

	public static class DataUtils
	{

		public static string Vector2ToString(Vector2 v, char split = ',')
		{
			return string.Empty + Math.Round(v.x, 4) + split + Math.Round(v.y, 4); // Around the 4 decimal places so extremely small number are zero-ized
		}

		public static Vector3 GetVector2(object obj, Vector2 defaultValue = default, string error = "")
		{
			return GetVector2((string)obj, defaultValue, error);
		}

		public static Vector2 GetVector2(string s, Vector2 defaultValue = default, string error = "")
		{
			return GetVector2(s, ',', defaultValue, error);
		}

		public static Vector2 GetVector2(string s, char separator, Vector2 defaultValue = default, string error = "")
		{
			if (!string.IsNullOrEmpty(s))
			{
				string[] values = s.Split(separator);
				if (values.Length >= 2)
				{
					bool success = true;
					success &= float.TryParse(values[0], out float f1);
					success &= float.TryParse(values[1], out float f2);
					if (success)
						return new Vector2(f1, f2);
				}
			}
			if (!string.IsNullOrEmpty(error))
				Debug.LogError(error);
			return defaultValue;
		}

		public static Vector3 GetVector3(object obj, Vector3 defaultValue = default, string error = "")
		{
			return GetVector3((string)obj, defaultValue, error);
		}

		public static Vector3 GetVector3(string s, Vector3 defaultValue = default, string error = "")
		{
			return GetVector3(s, ',', defaultValue, error);
		}

		public static Vector3 GetVector3(string s, char separator, Vector3 defaultValue = default, string error = "")
		{
			if (!string.IsNullOrEmpty(s))
			{
				string[] values = s.Split(separator);
				if (values.Length >= 2)
				{
					bool success = true;
					success &= float.TryParse(values[0], out float f1);
					success &= float.TryParse(values[1], out float f2);
					success &= float.TryParse(values[2], out float f3);
					if (success)
						return new Vector3(f1, f2, f3);
				}
			}
			if (!string.IsNullOrEmpty(error))
				Debug.LogError(error);
			return defaultValue;
		}

		public static string Vector3ToString(Vector3 v, char split = ',')
		{
			return string.Empty + Math.Round(v.x, 4) + split + Math.Round(v.y, 4) + split + Math.Round(v.z, 4); // Around the 4 decimal places so extremely small number are zero-ized
		}

		public static Vector4 GetVector4(object obj, Vector4 defaultValue = default, string error = "")
		{
			return GetVector4((string)obj, defaultValue, error);
		}

		public static Vector4 GetVector4(string s, Vector4 defaultValue = default, string error = "")
		{
			return GetVector4(s, ',', defaultValue, error);
		}

		public static Vector4 GetVector4(string s, char separator, Vector4 defaultValue = default, string error = "")
		{
			if (!string.IsNullOrEmpty(s))
			{
				string[] values = s.Split(separator);
				if (values.Length >= 2)
				{
					bool success = true;
					success &= float.TryParse(values[0], out float f1);
					success &= float.TryParse(values[1], out float f2);
					success &= float.TryParse(values[2], out float f3);
					success &= float.TryParse(values[3], out float f4);
					if (success)
						return new Vector4(f1, f2, f3, f4);
				}
			}
			if (!string.IsNullOrEmpty(error))
				Debug.LogError(error);
			return defaultValue;
		}

		public static string Vector4ToString(Vector4 v, char split = ',')
		{
			return string.Empty + System.Math.Round(v.x, 4) + split + System.Math.Round(v.y) + split + System.Math.Round(v.z) + split + System.Math.Round(v.w); // Around the 4 decimal places so extremely small number are zero-ized
		}

		public static int GetInt(object o, int defaultValue = 0, string error = "")
		{
			if (o is int)
				return (int)o;
			if (o is double)
				return (int)((double)o);
			return GetInt(o as string, defaultValue, error);
		}

		public static int GetInt(string s, int defaultValue = 0, string error = "")
		{
			bool success = int.TryParse(s, out int i);
			if (success)
				return i;
			if (!string.IsNullOrEmpty(error))
				Debug.LogError(error);
			return defaultValue;
		}

		public static float GetFloat(object o, float defaultValue = 0, string error = "")
		{
			if (o is float)
				return (float)o;
			if (o is double)
				return (float)((double)o);
			if (o is int)
				return (int)o;
			return GetFloat(o as string, defaultValue, error);
		}

		public static float GetFloat(string s, float defaultValue = 0, string error = "")
		{
			bool success = float.TryParse(s, out float f);
			if (success)
				return f;
			if (!string.IsNullOrEmpty(error))
				Debug.LogError(error);
			return defaultValue;
		}

		public static string BoolToString(bool b)
		{
			return b ? "1" : "0";
		}

		public static bool GetBool(string s)
		{
			return !string.IsNullOrEmpty(s) && s != "0";
		}

		public static bool TryGetBool(string s, out bool result)
		{
			result = false; // Initialize our out value

			if (string.IsNullOrWhiteSpace(s)) { return false; } //If our string is empty, return false
			s = s.Trim(); // Trim out whiteSpace

			// If we can parse our string to an int. We might have recieved our bool as a int value (0 / 1)
			if (int.TryParse(s, out int boolInt))
			{
				result = (boolInt == 1);
				return true;
			}

			// Otherwise we can just test for string equality
			s = s.ToLower();
			if (s == "true" || s == "false")
			{
				result = (s == "true");
				return true;
			}

			return false;
		}

		public static DateTime GetDateTime(string s, DateTime defaultValue = default, string error = "")
		{
			bool success = DateTime.TryParse(s, out DateTime d);
			if (success)
				return d;
			if (!string.IsNullOrEmpty(error))
				Debug.LogError(error);
			return defaultValue;
		}

		public static int[] GetIntArray(string s, int defaultValue = 0, string error = "")
		{
			string[] stringArray = s.Split('|');
			int[] intArray = new int[stringArray.Length];
			for (int i = 0, imax = stringArray.Length; i < imax; i++)
			{
				intArray[i] = GetInt(stringArray[i], defaultValue, error);
			}
			return intArray;
		}

		public static string[] GetStringArray(string s)
		{
			string[] stringArray = s.Split('|');
			return stringArray;
		}

		public static string[] GetStringArray(object obj)
		{
			if (obj is ArrayList)
				return (obj as ArrayList).OfType<string>().ToArray();
			return (string[])obj;
		}

		public static bool IsAllDigital(string s)
		{
			if (!string.IsNullOrEmpty(s))
				return s.All(char.IsDigit);
			else
				return false;
		}

		public static int Parse(int? i, int defaultValue = 0)
		{
			return i == null ? defaultValue : (int)i;
		}

		public static Color32 GetColor32(string s, Color32 defaultValue = default(Color32), string error = "")
		{
			return GetColor32(s, ',', defaultValue, error);
		}

		public static Color32 GetColor32(string s, char separator, Color32 defaultValue = default(Color32), string error = "")
		{
			if (!string.IsNullOrEmpty(s))
			{
				string[] values = s.Split(separator);
				if (values.Length >= 2)
				{
					bool success = true;
					success &= byte.TryParse(values[0], out byte c1);
					success &= byte.TryParse(values[1], out byte c2);
					success &= byte.TryParse(values[2], out byte c3);
					success &= byte.TryParse(values[3], out byte c4);
					if (success)
						return new Color32(c1, c2, c3, c4);
				}
			}
			if (!string.IsNullOrEmpty(error))
				Debug.LogError(error);
			return defaultValue;
		}

		public static string Color32ToString(Color32 c, char split = ',')
		{
			return string.Empty + c.r + split + c.g + split + c.b + split + c.a;
		}

		public static Quaternion GetQuaternion(object obj, Quaternion defaultValue = default, string error = "")
		{
			return GetQuaternion((string)obj, defaultValue, error);
		}

		public static Quaternion GetQuaternion(string s, Quaternion defaultValue = default, string error = "")
		{
			return GetQuaternion(s, ',', defaultValue, error);
		}

		public static Quaternion GetQuaternion(string s, char separator, Quaternion defaultValue = default, string error = "")
		{
			if (!string.IsNullOrEmpty(s))
			{
				string[] values = s.Split(separator);
				if (values.Length == 4)
				{
					bool success = true;
					success &= float.TryParse(values[0], out float c1);
					success &= float.TryParse(values[1], out float c2);
					success &= float.TryParse(values[2], out float c3);
					success &= float.TryParse(values[3], out float c4);
					if (success)
						return new Quaternion(c1, c2, c3, c4);
				}
			}
			if (!string.IsNullOrEmpty(error))
				Debug.LogError(error);
			return defaultValue;
		}

		public static string QuaternionToString(Quaternion c, char split = ',')
		{
			return string.Empty + c.x + split + c.y + split + c.z + split + c.w;
		}

		/// <summary>
		/// Convert a big integer to a number readable string, e.g. 1.5k, 20.6M, 100.7B
		/// </summary>
		public static string ToKMBString(this int num, bool highPrecision = false)
		{
			if (num < 999)
				return num.ToString(System.Globalization.CultureInfo.InvariantCulture);
			else if (num < 999999)
				return num.ToString(highPrecision ? "0,.00K" : "0,.#K", System.Globalization.CultureInfo.InvariantCulture);
			else if (num < 999999999)
				return num.ToString(highPrecision ? "0,,.000M" : "0,,.##M", System.Globalization.CultureInfo.InvariantCulture);
			else
				return num.ToString(highPrecision ? "0,,,.0000B" : "0,,,.###B", System.Globalization.CultureInfo.InvariantCulture);
		}

	}

}