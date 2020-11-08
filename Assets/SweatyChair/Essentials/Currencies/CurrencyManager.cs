using UnityEngine;
using UnityEngine.Events;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SweatyChair
{

	/// <summary>
	/// A base class that controls player's virtual currency, childrens classes for different saving method.
	/// Author: Richard
	/// </summary>
	public static class CurrencyManager
	{

		private const string PREFS_TOTAL_COIN_SPENT = "TotalCoinsSpent";
		private const string PREFS_TOTAL_GEM_SPENT = "TotalGemsSpent";
		private const string PREFS_TOTAL_EXTRA_CURRENCY_SPENT = "TotalExtraCurrencySpent";

		private const string GS_COINS_COUNT = "Coins";
		private const string GS_GEMS_COUNT = "Gems";
		private const string GS_EXTRA_CURRENCY_COUNT = "ExtraCurrency";

		public static event UnityAction<int> coinsChanged, gemsChanged, extraCurrencyChanged;
		public static event UnityAction<bool> uiToggled;

		public static CurrencySettings _settings => CurrencySettings.current;

		public static ObscuredInt coinCount { get; private set; }
		public static ObscuredInt gemCount { get; private set; }
		public static ObscuredInt extraCurrencyCount { get; private set; }

		public static int totalCoinsSpent {
			get { return PlayerPrefs.GetInt(PREFS_TOTAL_COIN_SPENT); }
			private set { PlayerPrefs.SetInt(PREFS_TOTAL_GEM_SPENT, value); }
		}

		public static int totalGemsSpent {
			get { return PlayerPrefs.GetInt(PREFS_TOTAL_COIN_SPENT); }
			private set { PlayerPrefs.SetInt(PREFS_TOTAL_GEM_SPENT, value); }
		}

		public static int totalExtraCurrencySpent {
			get { return PlayerPrefs.GetInt(PREFS_TOTAL_EXTRA_CURRENCY_SPENT); }
			private set { PlayerPrefs.SetInt(PREFS_TOTAL_EXTRA_CURRENCY_SPENT, value); }
		}

		static CurrencyManager()
		{
			if (GameSave.isNew) {
				coinCount = _settings.startCoins;
				gemCount = _settings.startGems;
				extraCurrencyCount = _settings.startExtraCurrency;
				if (coinCount > 0) GameSave.SetInt(GS_COINS_COUNT, coinCount, SyncPolicy.NoSync);
				if (gemCount > 0) GameSave.SetInt(GS_GEMS_COUNT, gemCount, SyncPolicy.NoSync);
				if (extraCurrencyCount > 0) GameSave.SetInt(GS_EXTRA_CURRENCY_COUNT, extraCurrencyCount, SyncPolicy.ForceSync);
			} else {
				Load();
			}

			GameSaveSync.gameSaveUpdated += Load; // Reload the currenty if server game save updated

#if DEMO
			coinCount = 1000000;
			gemCount = 10000;
			extraCurrencyCount = 1000000;
#endif
		}

		private static void Load()
		{
			coinCount = GameSave.GetInt(GS_COINS_COUNT, _settings.startCoins);
			gemCount = GameSave.GetInt(GS_GEMS_COUNT, _settings.startGems);
			extraCurrencyCount = GameSave.GetInt(GS_EXTRA_CURRENCY_COUNT, _settings.startExtraCurrency);
			coinsChanged?.Invoke(coinCount);
			gemsChanged?.Invoke(gemCount);
			extraCurrencyChanged?.Invoke(extraCurrencyCount);
		}

		public static int GetCurrency(ItemType itemType)
		{
			switch (itemType) {
				case ItemType.Coin:
					return coinCount;
				case ItemType.Gem:
					return gemCount;
			}
			return 0;
		}

		private static void OnCoinsChanged()
		{
			coinsChanged?.Invoke(coinCount);
			GameSave.SetInt(GS_COINS_COUNT, coinCount);
		}

		private static void OnGemsChanged()
		{
			gemsChanged?.Invoke(gemCount);
			GameSave.SetInt(GS_GEMS_COUNT, gemCount);
		}

		private static void OnExtraCurrencyChanged()
		{
			extraCurrencyChanged?.Invoke(extraCurrencyCount);
			GameSave.SetInt(GS_EXTRA_CURRENCY_COUNT, extraCurrencyCount);
		}

		public static void SetCoins(int? amount)
		{
			if (_settings.debugMode)
				Debug.LogFormat("CurrencyManager:SetCoins({0})", amount);
			if (amount != null) {
				coinCount = (int)amount;
				OnCoinsChanged();
			}
		}

		public static void SetGems(int? amount)
		{
			if (_settings.debugMode)
				Debug.LogFormat("CurrencyManager:SetGems({0})", amount);
			if (amount != null) {
				gemCount = (int)amount;
				OnGemsChanged();
			}
		}

		public static void SetExtraCurrency(int? amount)
		{
			if (_settings.debugMode)
				Debug.LogFormat("CurrencyManager:SetExtraCurrency({0})", amount);
			if (amount != null) {
				extraCurrencyCount = (int)amount;
				OnExtraCurrencyChanged();
			}
		}

		public static void AddCoins(long? amount)
		{
			if (amount != null)
				AddCoins((int)amount);
		}
		public static void AddCoins(int amount = 1, string eventName = "")
		{
			if (amount <= 0) return; // Prevent negative add
			coinCount += amount;
			OnCoinsChanged();
			GameAnalytics.RewardCoins(amount, eventName);
		}

		public static void AddGems(long? amount)
		{
			if (amount != null)
				AddGems((int)amount);
		}
		public static void AddGems(int amount = 1, string eventName = "")
		{
			if (amount <= 0) return; // Prevent negative add
			gemCount += amount;
			OnGemsChanged();
			GameAnalytics.RewardGems(amount, eventName);
		}

		public static void AddExtraCurrency(long? amount)
		{
			if (amount != null)
				AddExtraCurrency((int)amount);
		}
		public static void AddExtraCurrency(int amount = 1, string eventName = "")
		{
			if (amount <= 0) return; // Revent negative add hack
			extraCurrencyCount += amount;
			OnExtraCurrencyChanged();
			GameAnalytics.RewardExtraCurrency(amount, eventName);
		}

		public static void AddCurrency(CurrencyItem currency, string eventName = "")
		{
			switch (currency.type) {
				case ItemType.Coin:
					AddCoins(currency.amount, eventName);
					return;
				case ItemType.Gem:
					AddGems(currency.amount, eventName);
					return;
				case ItemType.ExtraCurrency:
					AddExtraCurrency(currency.amount, eventName);
					return;
			}
		}

		public static bool CheckCoins(int amount)
		{
			return amount >= 0 && coinCount >= amount;
		}

		public static bool CheckGems(int amount)
		{
			return amount >= 0 && gemCount >= amount;
		}

		public static bool CheckExtraCurrency(int amount)
		{
			return amount >= 0 && extraCurrencyCount >= amount;
		}

		public static bool CheckCurrency(Item item)
		{
			if (item is CurrencyItem currency)
				return CheckCurrency(currency);
			if (item is SerializableItem serializableItem)
				return CheckCurrency(serializableItem.ToItem());
			return false;
		}
		public static bool CheckCurrency(CurrencyItem currency)
		{
			switch (currency.type) {
				case ItemType.Coin:
					return CheckCoins((int)currency.amount);
				case ItemType.Gem:
					return CheckGems((int)currency.amount);
				case ItemType.ExtraCurrency:
					return CheckExtraCurrency((int)currency.amount);
				default:
					return false;
			}
		}

		public static void SpendCoins(int amount, string eventName = "")
		{
			SpendCoins(amount, null, null, eventName);
		}
		public static void SpendCoins(long? amount, string eventName = "")
		{
			if (amount != null) SpendCoins((int)amount, null, null, eventName);
		}
		public static void SpendCoins(int amount, UnityAction onSucceed, UnityAction onFailed = null, string eventName = "")
		{
			if (!CheckCoins(amount)) {
				onFailed?.Invoke();
			} else {
				if (amount > 0) {
					coinCount -= amount;
					OnCoinsChanged();
					++totalCoinsSpent;
				}
				onSucceed?.Invoke();
				GameAnalytics.SpendCoins(amount, eventName);
			}
		}

		public static void SpendGems(int amount, string eventName = "")
		{
			SpendGems(amount, null, null, eventName);
		}
		public static void SpendGems(long? amount, string eventName = "")
		{
			if (amount != null) SpendGems((int)amount, null, null, eventName);
		}
		public static void SpendGems(int amount, UnityAction onSucceed, UnityAction onFailed = null, string eventName = "")
		{
			if (!CheckGems(amount)) {
				onFailed?.Invoke();
			} else {
				if (amount > 0) {
					gemCount -= amount;
					OnGemsChanged();
					++totalGemsSpent;
				}
				onSucceed?.Invoke();
				GameAnalytics.SpendGems(amount, eventName);
			}
		}

		public static void SpendExtraCurrency(int amount, string eventName = "")
		{
			SpendExtraCurrency((int)amount, null, null, eventName);
		}
		public static void SpendExtraCurrency(long? amount, string eventName = "")
		{
			if (amount != null) SpendExtraCurrency((int)amount, null, null, eventName);
		}
		public static void SpendExtraCurrency(int amount, UnityAction onSucceed, UnityAction onFailed = null, string eventName = "")
		{
			if (!CheckExtraCurrency(amount)) {
				onFailed?.Invoke();
			} else {
				if (amount > 0) {
					extraCurrencyCount -= amount;
					OnExtraCurrencyChanged();
					++totalExtraCurrencySpent;
				}
				onSucceed?.Invoke();
				GameAnalytics.SpendExtraCurrency(amount, eventName);
			}
		}

		public static void SpendCurrency(Item item, UnityAction onSucceed = null, UnityAction onFailed = null)
		{
			if (item is CurrencyItem currency)
				SpendCurrency(currency, onSucceed, onFailed);
			if (item is SerializableItem serializableItem)
				SpendCurrency(serializableItem.ToItem(), onSucceed, onFailed);
		}
		public static void SpendCurrency(CurrencyItem currency, UnityAction onSucceed = null, UnityAction onFailed = null)
		{
			switch (currency.type) {
				case ItemType.Coin:
					SpendCoins(currency.amount, onSucceed, onFailed);
					break;
				case ItemType.Gem:
					SpendGems(currency.amount, onSucceed, onFailed);
					break;
				case ItemType.ExtraCurrency:
					SpendExtraCurrency(currency.amount, onSucceed, onFailed);
					break;
			}
		}

		public static string GetCurrencyTypeString(CurrencyItem currency)
		{
			return LocalizeUtils.GetPlural(TermCategory.Shop, currency.type);
		}

		public static void ToggleUI(bool doShow)
		{
			uiToggled?.Invoke(doShow);
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Currency/Print Parameters")]
		private static void PrintParameters()
		{
			DebugUtils.Log(coinCount, "coinCount");
			DebugUtils.Log(gemCount, "gemCount");
			DebugUtils.Log(extraCurrencyCount, "extraCurrencyCount");
		}

		[UnityEditor.MenuItem("Debug/Currency/Add 100000 Coins", false, 500)] // 500 sorting order
		private static void Add100000Coins()
		{
			AddCoins(100000);
		}

		[UnityEditor.MenuItem("Debug/Currency/ZERO Coins")]
		private static void SetCoinsToZero()
		{
			SpendCoins(coinCount);
		}

		[UnityEditor.MenuItem("Debug/Currency/Add 100000 Gems")]
		private static void AddThousandGems()
		{
			AddGems(100000);
		}

		[UnityEditor.MenuItem("Debug/Currency/ZERO Gems")]
		private static void SetGemsToZero()
		{
			SpendGems(gemCount);
		}

		[UnityEditor.MenuItem("Debug/Currency/Add 100000 Extra Currency")]
		private static void AddThousandExtraCurrency()
		{
			AddExtraCurrency(100000);
		}

		[UnityEditor.MenuItem("Debug/Currency/ZERO Extra Currency")]
		private static void SetExtraCurrencyToZero()
		{
			SpendExtraCurrency(extraCurrencyCount);
		}

#endif

	}

}