using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SweatyChair.StateManagement;

namespace SweatyChair.UI
{

	public class QuickShopSlot : ShopItemSlot
	{

		//private static readonly Vector2 MEDIUM_SIZE = new Vector2(100, 100);
		//private static readonly Vector2 LARGE_SIZE = new Vector2(150, 150);

		[SerializeField] private Text _amountLabel;

		[SerializeField] private GameObject _watchVideoButtonGO;

		[SerializeField] private GameObject _buyButtonGO;

		public override void Set(ShopItem sItem)
		{
			base.Set(sItem);

			ShopData shopData = sItem.shopData;

			_amountLabel.text = string.Format("x{0}", shopData.item.amount);

			SetIcon(shopData);

			CurrencyItem cost = shopData.cost;

			if (cost.type != ItemType.RewardedVideo) {

				_buyButtonGO.SetActive(true);
				_watchVideoButtonGO.SetActive(false);
				//_lockMaskGO.SetActive(false);

			} else {

				_buyButtonGO.SetActive(false);
				_watchVideoButtonGO.SetActive(true);
				//_lockMaskGO.SetActive(!AdsManager.isRewardedVideoAvailable);

			}
		}

		protected override void SetIcon(ShopData shopData)
		{
			if (_iconImage == null)
				return;

			switch (shopData.item.type) {
				case ItemType.Coin:
					//_iconSprite.sprite = MessagePanel.coinSprite;
					break;

				case ItemType.Gem:
					//_iconSprite.sprite = MessagePanel.gemSprite;
					break;

				case ItemType.ExtraCurrency:
					//_iconImage.sprite = SpriteReferenceManager.GetSprite("Soul");
					break;

				//case ItemType.Consumable:
				//	_iconTexture.enabled = true;
				//	_iconSprite.enabled = false;
				//	_iconTexture.mainTexture = ConsumableDatabase.GetData(shopData.rewardId).icon;
				//	break;

				//case ItemType.Energy:
				//	_iconTexture.enabled = false;
				//	_iconSprite.enabled = true;

				//	if (_bItem.amount == EnergyManager.buyEnergiesAmount) {
				//		_iconSprite.width = (int)MEDIUM_SIZE.x;
				//		_iconSprite.height = (int)MEDIUM_SIZE.y;
				//	} else if (_bItem.amount >= EnergyManager.maxEnergy) {
				//		_iconSprite.width = (int)LARGE_SIZE.x;
				//		_iconSprite.height = (int)LARGE_SIZE.y;
				//	}

				//	_iconSprite.spriteName = Constants.ENERGY_ICON;
				//	break;

				//default:
				//	_iconTexture.enabled = false;
				//	_iconSprite.enabled = true;
				//	if (shopData.type == ItemType.Coin)
				//		_iconSprite.spriteName = "coin";
				//	else if (shopData.type == ItemType.Gem)
				//		_iconSprite.spriteName = "gem";
				//	break;
			}
		}

		public void ToggleLock(bool isLock = true)
		{
			if (_lockMaskGO == null)
				return;
			_lockMaskGO.SetActive(isLock);
		}

		#region Button Controls

		// To prevent clicking 2 shop slots at the same time
		private static float _lastClickedTime;

		void OnClick()
		{
			if (_lastClickedTime > Time.unscaledTime - 2)
				return;
			_lastClickedTime = Time.unscaledTime;
			ShopManager.TryPurchase(item.shopData, OnPurchaseSucceed, OnPurchaseFailed);
		}

		#endregion

		protected override void OnPurchaseSucceed()
		{
			if (item.type == ItemType.InventoryDraw) // Nothing to do if for item draw
				return;

			if (StateManager.Compare(State.Game)) {

				UnityAction confirmAction = GameSpeedManager.Resume;
				UnityAction cancelAction = GameSpeedManager.Resume;
				// Don't resume game if player is just buying coins or gems
				if (item.type == ItemType.Coin || item.type == ItemType.Gem) {
					confirmAction = null;
					cancelAction = null;
				}

				new Message {
					format = MessageFormat.Reward,
					title = LocalizeUtils.Get(TermCategory.Shop, CommonTexts.MSG_PURCHASE_SUCCEED_TITLE),
					content = LocalizeUtils.GetFormat(TermCategory.Shop, CommonTexts.MSG_RECEIVED_CONTENT, item.shopData.item.amountString),
					confirmCallback = confirmAction,
					cancelCallback = cancelAction
				}.Show();

			} else {

				UnityAction quickShopSuccessCallback = null;
				if (MessagePanel.currentMessage != null)
					quickShopSuccessCallback = MessagePanel.currentMessage.quickShopSuccessCallback;

				new Message {
					format = MessageFormat.Reward,
					title = LocalizeUtils.Get(TermCategory.Shop, CommonTexts.MSG_PURCHASE_SUCCEED_TITLE),
					content = LocalizeUtils.GetFormat(TermCategory.Shop, CommonTexts.MSG_RECEIVED_CONTENT, item.shopData.item.localizedString),
					confirmCallback = quickShopSuccessCallback,
					cancelCallback = quickShopSuccessCallback
				}.Show();

			}

			MessageManager.CancelLastMessage(); // Hide the quick shop message
		}

	}

}