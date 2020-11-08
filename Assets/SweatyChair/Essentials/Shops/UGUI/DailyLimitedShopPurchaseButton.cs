using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair
{

	public class DailyLimitedShopPurchaseButton : ShopPurchaseButton
	{

		#region Constants

		protected string PP_DAILY_PURCHASE_COUNT => $"dailyLimitedShopPurchaseCount_id-{_shopDataID}";
		protected string PP_LAST_PURCHASE_DATE => $"dailyLimitedShopPurchaseDate_id-{_shopDataID}";

		#endregion

		#region Variables
		#region Inspector

		[Header("Limit UI")]
		[SerializeField] protected GameObject _canPurchaseObj;
		[SerializeField] protected GameObject _cooldownObj;
		[SerializeField] protected GameObject _noPurchaseLeftObj;
		[Space]
		[SerializeField] protected Text _dailyLimitText;
		[SerializeField] protected Text _timeRemainingText;

		[Header("Limit Settings")]
		[SerializeField] protected int _dailyLimit = 1;
		[Space]
		[SerializeField] protected float _puchaseCooldownMins = 5;

		#endregion

		#region Cached Vars

		// Cached Variables
		protected bool canBePurchased => hasPurchasesRemaining && !_purchaseOnCooldown;

		// Purchase Count
		protected int _purchases = -1;
		public int purchases
		{
			get {
				// First check if a day has passed
				bool lastPurchaseMoreThanDay = lastPurchaseTime.Date < DateTimeUtils.Now().Date;

				// If we purchased something a day ago, reset all of our data
				if (lastPurchaseMoreThanDay)
					purchases = 0;
				else {

					if (_purchases < 0) {
						// Player Prefs
						_purchases = PlayerPrefs.GetInt(PP_DAILY_PURCHASE_COUNT, -1);

						// If our Purchases is less than 0
						if (_purchases < 0)
							purchases = 0;
					}
				}
				return _purchases;
			}
			set {
				_purchases = value;
				PlayerPrefs.SetInt(PP_DAILY_PURCHASE_COUNT, _purchases);

				// Then set our last purchase time
				if (_purchases > 0)
					lastPurchaseTime = DateTimeUtils.Now();
			}
		}
		public bool hasPurchasesRemaining => purchases < _dailyLimit;


		// Purchase Cooldown
		private DateTime _lastPurchaseTime;
		protected DateTime lastPurchaseTime
		{
			get {

				if (_lastPurchaseTime == default) {

					// PlayerPrefs
					_lastPurchaseTime = DateTimeUtils.GetPlayerPrefs(PP_LAST_PURCHASE_DATE, DateTime.MinValue);

					// If our time is still default, set it to yesterday at this time
					if (_lastPurchaseTime == DateTime.MinValue)
						lastPurchaseTime = DateTimeUtils.Now().AddDays(-1);

				}
				return _lastPurchaseTime;
			}
			set {
				_lastPurchaseTime = value;
				DateTimeUtils.SetPlayerPrefs(PP_LAST_PURCHASE_DATE, _lastPurchaseTime);
			}
		}

		protected DateTime _nextPurchaseTime => lastPurchaseTime.AddMinutes(_puchaseCooldownMins);
		protected bool _purchaseOnCooldown => DateTimeUtils.IsFuture(_nextPurchaseTime);


		// Other
		protected IEnumerator _yieldRoutine;

		#endregion

		#endregion

		#region OnEnable / OnDisable

		protected virtual void OnEnable()
		{
			ShopManager.purchaseCompleted += OnPurchaseCompleted;

			// Udpate our changing UI
			UpdateDailyCountUI();
		}

		protected virtual void OnDisable()
		{
			ShopManager.purchaseCompleted -= OnPurchaseCompleted;

			// Stop our update routine
			if (_yieldRoutine != null)
				TimeManager.Stop(_yieldRoutine);
		}

		#endregion

		#region Event Callbacks

		protected void OnPurchaseCompleted(ShopData shopData)
		{
			// Make sure the purchase is our purchase
			if (shopData.id != _shopDataID)
				return;

			// Then update our UI
			UpdateDailyCountUI();
		}

		protected override void OnButtonClick()
		{
			// Only allow button clicking if we can be purchased
			if (canBePurchased)
				base.OnButtonClick();
		}

		protected override void PurchaseSucceed()
		{
			// if our purchase succeeds, Set all of our data, We update our ui in our event listener
			purchases++;

			base.PurchaseSucceed();
		}

		#endregion

		#region UI

		protected override void InitializeUI()
		{
			base.InitializeUI();

			// Update our Daily count ui
			UpdateDailyCountUI();
		}

		protected void UpdateDailyCountUI()
		{
			if (_yieldRoutine != null)
				TimeManager.Stop(_yieldRoutine);

			// Toggle our objects
			_canPurchaseObj.SetActive(canBePurchased);
			_cooldownObj.SetActive(hasPurchasesRemaining && _purchaseOnCooldown);
			_noPurchaseLeftObj.SetActive(!hasPurchasesRemaining);

			// Set our text
			_dailyLimitText.text = $"{LocalizeUtils.GetFormat("Daily Limit")}: {_dailyLimit - purchases}";
			_timeRemainingText.text = _nextPurchaseTime.GetTimeSpanFromNow().ToString(@"mm\:ss");

			if (hasPurchasesRemaining && _purchaseOnCooldown)
				_yieldRoutine = TimeManager.Invoke(UpdateDailyCountUI, 0.98f);
		}

		#endregion

		#region Debug

#if UNITY_EDITOR

		[ContextMenu("Reset Purchase Count")]

		private void EDITOR_ResetPurchaseCount()
		{
			purchases = 0;
			UpdateDailyCountUI();
		}

#endif

		#endregion

	}

}
