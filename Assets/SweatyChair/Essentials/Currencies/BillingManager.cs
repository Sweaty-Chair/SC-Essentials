using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using SweatyChair.UI;
#if EM_UIAP
using EasyMobile;
using UnityEngine.Purchasing;
#endif

namespace SweatyChair
{

	// A master class that take care of all in-app-purchases
	public static class BillingManager
	{

		// The minimum interval of two consecutive transactions, in seconds
		private const float MIN_TRANSACTION_INTERVAL = 2;

		// Call when restore purchase button is clicked
		public static event UnityAction<string> purchaseRestored;
		// Call when product data requested
		public static event UnityAction productDataReceived;
		// Call when promoted IAP is purchased and back to the app
		public static event UnityAction<string> promotedPurchaseSucceed;

		public static bool isPurchasing { get; private set; } // Used for avoid making two transaction at the same time

		private static float _lastTransactionTime = 0;

		// Cache variables for login retry
		private static bool _isPromotedPurchase;
		private static UnityAction _purchaseSucceed, _puchaseFailed;

		private static bool _isWithinMinTransactionInterval { // Avoid duplicate clicks and bit harder way to hack
			get {
				bool tmp = Time.unscaledTime - _lastTransactionTime < MIN_TRANSACTION_INTERVAL;
				if (tmp)
					_lastTransactionTime = Time.unscaledTime;
				return tmp;
			}
		}

		static BillingManager()
		{
#if EM_UIAP
			InAppPurchasing.PurchaseCompleted += OnPurchaseSucceed;
			InAppPurchasing.PurchaseFailed += OnPurchaseFailed;
			InAppPurchasing.PromotionalPurchaseIntercepted += OnPromotionalPurchaseIntercepted;
#endif
		}

		/// <summary>
		/// Check is App Store / Google Play are initialized and ready to buy.
		/// </summary>
		private static bool CheckCanBuy()
		{
			if (_isWithinMinTransactionInterval) {
				Debug.Log("BillingManager:CheckCanBuy - Transaction requests are too close in between! This can be caused by double clicking a buy button, or hack.");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Purchase a product with given ID.
		/// </summary>
		public static void Purchase(string productId, UnityAction succeedCallback, UnityAction failedCallback = null)
		{
			_purchaseSucceed = succeedCallback;
			_puchaseFailed = failedCallback;

			if (!CheckCanBuy()) {
				// Failed getting the product data due to offline before, try login again now
				failedCallback?.Invoke();
				return;
			}

			isPurchasing = true;
			Loading.Display();

#if UNITY_EDITOR || DEMO || BYPASS_BILL
			// Always success on Editor, for debug only
			Debug.Log("BillingManager:Purchase - Simulate successfully purchase.");
			OnPurchaseSucceed();
			Loading.Hide();
			return;
#endif

#if EM_UIAP
			InAppPurchasing.PurchaseWithId(productId);
#endif
#if KAISER // TODO: Use agent for different publisher IAP
			Kaiser.Purchase(productId);
#endif
		}

		public static void OnPurchaseSucceed()
		{
			_purchaseSucceed?.Invoke();
			isPurchasing = false;
		}

		public static void OnPurchaseFailed()
		{
			_puchaseFailed?.Invoke();
			isPurchasing = false;
		}

#if EM_UIAP

		private static void OnPurchaseSucceed(IAPProduct product)
		{
			if (_isPromotedPurchase) {
				promotedPurchaseSucceed?.Invoke(product.Id);
				_purchaseSucceed = null;
			}
			_isPromotedPurchase = false;
			OnPurchaseSucceed();
		}

		private static void OnPurchaseFailed(IAPProduct product)
		{
			Debug.Log("The purchase of product " + product.Name + " has failed.");
			OnPurchaseFailed();
		}

		private static void OnPromotionalPurchaseIntercepted(IAPProduct product)
		{
			Debug.Log("Promotional purchase of product " + product.Name + " has been intercepted.");

			// Here you can perform necessary actions, e.g. presenting parental gates, 
			// sending analytics events, etc.

			_isPromotedPurchase = true;

			// Finally, you must call the ContinueApplePromotionalPurchases method
			// to continue the normal processing of the purchase!
			InAppPurchasing.ContinueApplePromotionalPurchases();
		}

#endif

		public static string PriceToPriceString(string currencyCode, decimal price)
		{
			return LocalizeUtils.GetFormat("{0} ${1:0.00}", currencyCode, price);
		}

		public static string GetProductPriceString(string productId, string defaultString = "")
		{
			if (string.IsNullOrEmpty(productId))
				return string.Empty;

#if UNITY_EDITOR
			//Debug.LogFormat("BillingManager:GetProductPriceString - Price for {0} cannot be retrived in editor, try in device instead", productId);
			return defaultString;
#endif

#if EM_UIAP
			// Get all products created in the In-App Purchasing module settings
			IAPProduct[] products = EM_Settings.InAppPurchasing.Products;
			foreach (IAPProduct prod in products) {
				if (prod.Id != productId) continue;
				ProductMetadata data = InAppPurchasing.GetProductLocalizedData(prod.Name);
				if (data != null)
					return data.localizedPriceString;
			}
#endif

			return defaultString;
		}

		public static string GetProductMultipliedPriceString(string productId, float multiplier)
		{
			if (string.IsNullOrEmpty(productId))
				return string.Empty;
			return PriceToPriceString(GetProductCurrencyCode(productId), GetProductMultipliedPrice(productId, multiplier));
		}

		public static decimal GetProductPrice(string productId, decimal defaultValue = 0)
		{
			if (string.IsNullOrEmpty(productId))
				return defaultValue;

#if UNITY_EDITOR
			Debug.LogFormat("BillingManager:GetProductPrice - Price for {0} cannot be retrived in editor, try in device instead", productId);
			return defaultValue;
#endif

#if EM_UIAP
			// Get all products created in the In-App Purchasing module settings
			IAPProduct[] products = EM_Settings.InAppPurchasing.Products;
			foreach (IAPProduct prod in products) {
				if (prod.Id != productId) continue;
				ProductMetadata data = InAppPurchasing.GetProductLocalizedData(prod.Name);
				if (data != null)
					return data.localizedPrice;
			}
#endif

			return defaultValue;
		}

		public static decimal GetProductMultipliedPrice(string productId, float multiplier)
		{
			return GetProductPrice(productId) * (decimal)multiplier;
		}

		public static string GetProductTitle(string productId)
		{
#if EM_UIAP
			// Get all products created in the In-App Purchasing module settings
			IAPProduct[] products = EM_Settings.InAppPurchasing.Products;
			foreach (IAPProduct prod in products) {
				if (prod.Id != productId) continue;
				ProductMetadata data = InAppPurchasing.GetProductLocalizedData(prod.Name);
				if (data != null)
					return data.localizedTitle;
			}
#endif
			return string.Empty;
		}

		public static string GetProductCurrencyCode(string productId)
		{
#if EM_UIAP
			// Get all products created in the In-App Purchasing module settings
			IAPProduct[] products = EM_Settings.InAppPurchasing.Products;
			foreach (IAPProduct prod in products) {
				if (prod.Id != productId) continue;
				ProductMetadata data = InAppPurchasing.GetProductLocalizedData(prod.Name);
				if (data != null)
					return data.isoCurrencyCode;
			}
#endif
			return string.Empty;
		}

#region Conversion Hack

		// Use localize to translate the USD to local currency, use this only when app store billing failed, or a Chinese android version
		public static string USDToLocalizedCurrencyString(float usdAmount)
		{
			switch (LocalizeUtils.currentLanguage) {
				case Language.ChineseSimplified:
					return LocalizeUtils.GetFormat("CNY ${0}", USDToCNY(usdAmount));
				case Language.ChineseTraditional:
					return LocalizeUtils.GetFormat("TWD ${0}", USDToTWD(usdAmount));
				default:
					return LocalizeUtils.GetFormat("USD ${0}", usdAmount);
			}
		}

		public static float USDToLocalizedCurrency(float usdAmount)
		{
#if EM_UIAP
			// Get all products created in the In-App Purchasing module settings
			IAPProduct[] products = EM_Settings.InAppPurchasing.Products;
			if (products.Length > 0) {
				ProductMetadata data = InAppPurchasing.GetProductLocalizedData(products[0].Name);
				if (data != null) {
					float tmp = usdAmount * ((float)data.localizedPrice / float.Parse(products[0].Price));
					if (tmp > 0) return tmp;
				}
			}
#endif
			switch (LocalizeUtils.currentLanguage) {
				case Language.ChineseSimplified:
					return USDToCNY(usdAmount);
				case Language.ChineseTraditional:
					return USDToTWD(usdAmount);
				default:
					return usdAmount;
			}
		}

		// A hack function to change USD amount to CNY
		public static int USDToCNY(float usdAmount)
		{
			Dictionary<float, int> convertDict = new Dictionary<float, int> {
				{ 3.99f, 25 },
				{ 5.99f, 40 },
				{ 6.99f, 45 },
				{ 7.99f, 50 },
				{ 8.99f, 60 },
				{ 9.99f, 68 },
				{ 10.99f, 73 },
				{ 11.99f, 78 },
				{ 12.99f, 88 },
				{ 13.99f, 93 },
				{ 14.99f, 98 },
				{ 19.99f, 128 },
				{ 23.99f, 158 },
				{ 24.99f, 163 },
				{ 29.99f, 198 },
				{ 39.99f, 258 },
				{ 49.99f, 328 },
				{ 59.99f, 388 },
			};
			if (convertDict.ContainsKey(usdAmount))
				return convertDict[usdAmount];
			return Mathf.CeilToInt(usdAmount * 6);
		}

		// A hack function to change USD amount to CNY
		public static float USDToTWD(float usdAmount)
		{
			return Mathf.CeilToInt(usdAmount * 30);
		}

#endregion

	}

}