using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SweatyChair
{

	public class LoadingHintManager
	{
	
		private const string FILENAME_LOADING_HINTS = "LoadingHints";

		private static List<string> _hintList = new List<string>();
		private static int _numHints;

		static LoadingHintManager()
		{
			TextAsset ta = Resources.Load<TextAsset>(FILENAME_LOADING_HINTS);
			string[] rows = ta.text.Split("\n"[0]);
			foreach (string r in rows) {
				if (r.Length == 0)
					continue;
				_hintList.Add(r);
			}
			_numHints = _hintList.Count;
		}

		public static string hint {
			get { return LocalizeUtils.Get(_hintList[Random.Range(0, _numHints)]); }
		}

		#if UNITY_EDITOR

		public static void PrintHints()
		{
			DebugUtils.LogEach(_hintList, "_hintList");
		}

		#endif
	}

}