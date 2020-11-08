using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using SweatyChair.StateManagement;
using SweatyChair.UI;

namespace SweatyChair
{

	public static class ShopManager
	{

		private const string GS_TOTAL_REAL_MONEY_SPENT = "TotalRealMoneySpent";
		private const string PREFS_REWARDED_VIDEO_LAST_CLAIM_DATE_TIME = "RewardedVideoLastClaimDateTime"; // +id
		public const string PREFS_REWARDED_VIDEO_CLAIM_COUNT = "RewardedVideoClaimCount"; // +id

		public static event UnityAction<List<Item>> shopInitialized;
		public static event UnityAction<ItemType, bool> uiToggled;
		public static event UnityAction totalRealMoneySpentChanged;

		public static event UnityAction<ShopData> purchaseCompleted;

		private static UnityAction _purchaseSucceed;
		private static Dictionary<ItemType, List<Item>> _shopItemDict = new Dictionary<ItemType, List<Item>>();
		private static State _lastState = State.Menu;
		private static ShopData _lastShopData;

		// Tier 0 - never spent; Tier 1 - USD $0~5; Tier 2 - $5~20; Tier 3 - $20+
		public static int tier {
			get {
				float totalSpent = totalRealMoneySpent;
				if (totalSpent > 20)
					return 1;
				else if (totalSpent > 5)
					return 2;
				else if (totalSpent > 0)
					return 3;
				return 0;
			}
		}

		// Records the total purchase using real money
		public static float totalRealMoneySpent {
			get { return GameSave.GetFloat(GS_TOTAL_REAL_MONEY_SPENT); }
			set { GameSave.SetFloat(GS_TOTAL_REAL_MONEY_SPENT, value); }
		}

		public static bool hasPaidRealMoney => totalRealMoneySpent > 0;

		static ShopManager()
		{
			List<ShopData> shopDataList = DatabaseManager.GetDatas<ShopData>();
			for (int i = 0; i < shopDataList.Count; i++) {
				ShopData shopData = shopDataList[i];
				if (shopData == null)
					continue;
				ItemType shopType = shopData.item.type;
				if (!_shopItemDict.ContainsKey(shopType))
					_shopItemDict.Add(shopType, new List<Item>());
				_shopItemDict[shopType].Add(new ShopItem(shopData));
			}
		}

		public static void TryPurchase(int shopDataId, UnityAction onSucceed, UnityAction onFailed = null)
		{
			TryPurchase(ShopDatabase.GetData(shopDataId), onSucceed, onFailed);
		}
		public static void TryPurchase(ShopData shopData, UnityAction onSucceed, UnityAction onFailed = null)
		{
			if (shopData == null)
				return;

			_lastShopData = shopData;
			CurrencyItem cost = shopData.cost;

			_purchaseSucceed = onSucceed;

			switch (cost.type) {
				case ItemType.Money: // Real Money
					BillingManager.Purchase(shopData.productId, () => {
						OnPurchaseSucceed();
#if !UNITY_EDITOR && !DEMO // Don't send analytics event in Editor and demo build, to avoid polluting the data
						GameAnalytics.Transaction(shopData.productId, (decimal)cost.floatAmount);
#endif
					}, onFailed);
					break;
				case ItemType.Gem: // Buy using Gems
					CurrencyManager.SpendGems(cost.amount, () => {
						OnPurchaseSucceed();
						GameAnalytics.SpendGems(cost.amount, "buy_" + shopData.item.type.ToString().ToLower());
					}, () => {
						ShowCurrencyShopMessage();
						onFailed?.Invoke();
					});
					break;
				case ItemType.Coin: // Buy using Coins
					CurrencyManager.SpendCoins(cost.amount, () => {
						OnPurchaseSucceed();
						GameAnalytics.SpendCoins(cost.amount, "buy_" + shopData.item.type.ToString().ToLower());
					}, () => {
						ShowCurrencyShopMessage();
						onFailed?.Invoke();
					});
					break;
				case ItemType.RewardedVideo:
					if (!CanClaim(shopData)) {
						new Message {
							title = LocalizeUtils.Get(TermCategory.Shop, CommonTexts.MSG_NO_MORE_CLAIMS_TITLE),
							content = LocalizeUtils.Get(TermCategory.Shop, CommonTexts.MSG_NO_MORE_CLAIMS_CONTENT)
						}.Show();
						return;
					}
					AdsManager.ShowRewardedVideo(AdTag.FreeCoins, () => {
						if (cost.amount > 0) {
							DateTimeUtils.SetPlayerPrefsToNow(PREFS_REWARDED_VIDEO_LAST_CLAIM_DATE_TIME + shopData.id);
							PlayerPrefs.SetInt(PREFS_REWARDED_VIDEO_CLAIM_COUNT + shopData.id, GetClaimCount(shopData) + 1);
						}
						OnPurchaseSucceed();
					});
					break;
			}
		}

		private static bool CanClaim(ShopData shopData)
		{
			int claimCount = PlayerPrefs.GetInt(PREFS_REWARDED_VIDEO_CLAIM_COUNT + shopData.id);
			if (shopData.cost.amount > 0) { // Limited daily usage
				DateTime lastClaimDateTime = DateTimeUtils.GetPlayerPrefs(PREFS_REWARDED_VIDEO_LAST_CLAIM_DATE_TIME + shopData.id);
				if (lastClaimDateTime.Date < DateTimeUtils.Now().Date) {
					claimCount = 0;
					PlayerPrefs.SetInt(PREFS_REWARDED_VIDEO_CLAIM_COUNT + shopData.id, 0);
				}
				return claimCount < shopData.cost.amount;
			}
			return true;
		}

		private static int GetClaimCount(ShopData shopData)
		{
			int claimCount = PlayerPrefs.GetInt(PREFS_REWARDED_VIDEO_CLAIM_COUNT + shopData.id);
			if (shopData.cost.amount > 0) { // Limited daily usage
				DateTime lastClaimDateTime = DateTimeUtils.GetPlayerPrefs(PREFS_REWARDED_VIDEO_LAST_CLAIM_DATE_TIME + shopData.id);
				if (lastClaimDateTime.Date < DateTimeUtils.Now().Date) {
					claimCount = 0;
					PlayerPrefs.SetInt(PREFS_REWARDED_VIDEO_CLAIM_COUNT + shopData.id, 0);
				}
			}
			return claimCount;
		}

		public static bool CanPurchase(int shopDataId)
		{
			return CanPurchase(ShopDatabase.GetData(shopDataId));
		}
		public static bool CanPurchase(ShopData shopData)
		{
			if (shopData.cost.type == ItemType.RewardedVideo)
				return CanClaim(shopData);
			return true;
		}

		private static void OnPurchaseSucceed()
		{
			if (_lastShopData != null) { // Just in case

				_lastShopData.item.Obtain(""); // No event

				if (_lastShopData.noAds)
					AdsManager.RemoveAds();

				if (!string.IsNullOrEmpty(_lastShopData.productId)) { // Analytics for total real money purchased
					totalRealMoneySpent += _lastShopData.cost.floatAmount;
					totalRealMoneySpentChanged?.Invoke();
				}
			}

			_purchaseSucceed?.Invoke();
			purchaseCompleted?.Invoke(_lastShopData);

			// Analytics
			switch (_lastShopData.item.type) {
				case ItemType.Coin:
					GameAnalytics.AcquireCoins(_lastShopData.item.amount, _lastShopData.cost.type == ItemType.Money, _lastShopData.id);
					break;
				case ItemType.Gem:
					GameAnalytics.AcquireGems(_lastShopData.item.amount, _lastShopData.cost.type == ItemType.Money, _lastShopData.id);
					break;
				default:
					GameAnalytics.AcquireItem(_lastShopData.item.type.ToString().ToLower(), _lastShopData.cost.type == ItemType.Money, _lastShopData.id);
					break;
			}
		}

		private static void ShowCurrencyShopMessage()
		{
			new Message {
				format = MessageFormat.Confirm,
				title = LocalizeUtils.GetFormat(TermCategory.Shop, CommonTexts.MSG_NOT_ENOUGH_TITLE, LocalizeUtils.Get(TermCategory.Shop, _lastShopData.cost.type.ToString())),
				content = LocalizeUtils.Get(TermCategory.Shop, CommonTexts.MSG_NOT_ENOUGH_CONTENT),
				confirmCallback = () => ShopManager.ToggleUI(_lastShopData.cost.type)
			}.Show();
		}

		public static void Init()
		{
			List<Item> shopItemList = new List<Item>();
			shopItemList.AddRange(_shopItemDict[ItemType.Coin]);
			shopItemList.AddRange(_shopItemDict[ItemType.Gem]);
			shopItemList.AddRange(_shopItemDict[ItemType.ExtraCurrency]);
			shopInitialized?.Invoke(shopItemList);
		}

		public static void ToggleUI(ItemType itemType, bool show = true)
		{
			uiToggled?.Invoke(itemType, show);

			State currentState = StateManager.Get();
			if (show && currentState != State.Shop)
				_lastState = currentState;

			StateManager.Set(show ? State.Shop : _lastState);

			if (show) {
				if (itemType == ItemType.Coin)
					GameAnalytics.OpenSoftStore();
				else
					GameAnalytics.OpenPremiumStore();
			}
		}

		public static float GetRealMoneyValue(Item bItem)
		{
			// $1 = 15Gem = 12500
			float convertedAmount = bItem.GetRealMoneyValue();

			// convertedAmount is in USD, try convert is to local currency if Billing is available
#if UNITY_EDITOR
			if (LocalizeUtils.currentLanguage == Language.ChineseSimplified) // Hack: Force convert USD to CNY when language is Chinese
				convertedAmount = BillingManager.USDToCNY(convertedAmount);
#else
			//if (BillingManager.isAvailable) {
			//	if (BillingManager.HasProduct("com.cmge.3dtd.1starcard")) { // HARD CODE: com.cmge.3dtd.1starcard is the USD$0.99 product and used to scale
			//		float tmp = convertedAmount / 0.99f * (float)BillingManager.GetProductPrice("com.cmge.3dtd.1starcard");
			//		if (tmp > 0)
			//			convertedAmount = tmp;
			//	}
			//} else if (LocalizeUtils.currentLanguage == Language.ChineseSimplified) { // Hack: Force convert USD to CNY when language is Chinese
			//	convertedAmount = BillingManager.USDToCNY(convertedAmount);
			//}
#endif

			return convertedAmount;
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Shop/Print Parameters")]
		private static void PrintParameters()
		{
			DebugUtils.Log(totalRealMoneySpent, "totalRealMoneySpent");
		}

		[UnityEditor.MenuItem("Debug/Shop/Reset Real Money Spent")]
		public static void ResetRealMoneySpent()
		{
			totalRealMoneySpent = 0;
			totalRealMoneySpentChanged?.Invoke();
		}

#endif

	}

}