using System;
using System.Collections.Generic;
using System.Linq;

namespace SweatyChair
{

	/// <summary>
	/// A A function extend class to operate enum.
	/// </summary>
	public static class EnumUtils
	{

		public static bool IsDefined<T>(int i) where T : struct, IConvertible
		{
			return Enum.IsDefined(typeof(T), i);
		}

		public static bool IsDefined<T>(string enumStr) where T : struct, IConvertible
		{
			if (string.IsNullOrEmpty(enumStr))
				return false;
			return Enum.IsDefined(typeof(T), enumStr);
		}

		public static IEnumerable<T> GetValues<T>() where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static int GetCount<T>() where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");
			return Enum.GetNames(typeof(T)).Length;
		}

		public static T GetRandom<T>() where T : struct, IConvertible
		{
			List<T> list = new List<T>(GetValues<T>());
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static T Parse<T>(string value, bool ignoreCase = true) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");
			if (string.IsNullOrEmpty(value))
				return default(T);
			return (T)Enum.Parse(typeof(T), value, ignoreCase);
		}

		public static bool HasNext<T>(this T src) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");

			T[] Arr = (T[])Enum.GetValues(src.GetType());
			int j = Array.IndexOf(Arr, src) + 1;
			return j < Arr.Length;
		}

		public static T Next<T>(this T src) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");

			T[] Arr = (T[])Enum.GetValues(src.GetType());
			int j = Array.IndexOf(Arr, src) + 1;
			return (Arr.Length == j) ? Arr[0] : Arr[j];
		}

		public static bool HasPrev<T>(this T src) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");

			T[] Arr = (T[])Enum.GetValues(src.GetType());
			int j = Array.IndexOf(Arr, src) - 1;
			return j >= 0;
		}

		public static T Prev<T>(this T src) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");

			T[] Arr = (T[])Enum.GetValues(src.GetType());
			int j = Array.IndexOf(Arr, src) - 1;
			return (j == -1) ? Arr[Arr.Length - 1] : Arr[j];
		}
		public static List<T> GetAllFlagsSet<T>(this T src) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");

			Enum castEnum = src as Enum;

			List<T> result = new List<T>();
			foreach (T enumItem in Enum.GetValues(typeof(T))) {
				if (castEnum.HasFlag(enumItem as Enum))
					result.Add(enumItem);
			}

			return result;
		}

	}

}