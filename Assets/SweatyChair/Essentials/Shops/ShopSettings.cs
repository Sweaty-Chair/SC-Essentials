using UnityEngine;

namespace SweatyChair
{

	[CreateAssetMenu(fileName = "ShopSettings", menuName = "Sweaty Chair/Settings/Shop")]
	public class ShopSettings : ScriptableObjectSingleton<ShopSettings>
	{

		[Tooltip("1 Gems = ? Coins")]
		public int conversionGemToCoin = 125;
		[Tooltip("USD $1 = ? Gems")]
		public int conversionRealMoneyToGem = 125;

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Sweaty Chair/Settings/Shop")]
		private static void OpenSettings()
		{
			current.OpenAssetsFile();
		}

#endif

	}

}