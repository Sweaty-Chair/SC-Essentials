using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Achievement module settings.
	/// </summary>
	[CreateAssetMenu(fileName = "CurrencySettings", menuName = "Sweaty Chair/Settings/Currency")]
	public class CurrencySettings : ScriptableObjectSingleton<CurrencySettings>
	{

		[Tooltip("The starting amount of coins.")]
		public int startCoins;

		[Tooltip("The starting amount of gems.")]
		public int startGems;

		[Tooltip("The starting amount of extra currency.")]
		public int startExtraCurrency;

		[Tooltip("Debug mode.")]
		public bool debugMode;

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Sweaty Chair/Settings/Currency")]
		private static void OpenSettings()
		{
			current.OpenAssetsFile();
		}
#endif

	}

}