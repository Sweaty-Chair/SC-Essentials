using UnityEngine;
using UnityEngine.UI;
using SweatyChair.UI;

namespace SweatyChair
{

	public class ShopItemSlot : ItemSlot<ShopItem>
	{

		[SerializeField] protected Image _iconImage;

		[SerializeField] protected Text _productNameText;

		[SerializeField] protected Text _costAmountText;
		[SerializeField] protected GameObject _videoAdGO;

		[SerializeField] protected GameObject _coinIconGO;
		[SerializeField] protected GameObject _gemIconGO;
		[SerializeField] protected GameObject _extraCurrencyIconGO;

		[SerializeField] protected Text _descriptionLabel;
		[SerializeField] protected Vector3 _slotScale = Vector3.one;

		[SerializeField] protected GameObject _lockMaskGO;

		protected override void OnEnable()
		{
			if (item == null || !(item is ShopItem))
				return;

			ShopItem shopItem = (ShopItem)item;
			ShopData shopData = shopItem.shopData;
			if (shopData.cost.type != ItemType.RewardedVideo)
				return;

			//ToggleUnlock(AdsManager.isRewardedVideoAvailable);
		}

		public override void Set(ShopItem sItem)
		{
			base.Set(sItem);
			transform.localScale = _slotScale;

			ShopItem shopItem = (ShopItem)sItem;
			if (shopItem == null)
				return;

			ShopData shopData = shopItem.shopData;

			if (_productNameText != null)
				_productNameText.text = shopData.localizedName;

			SetIcon(shopData);

			SetConfirmText(shopData);

			if (_descriptionLabel != null) {
				string desc = shopData.localizedDescription;
				if (shopData.noAds)
					desc += string.Format(" + {0}", LocalizeUtils.Get(TermCategory.Shop, "No Forced Ads"));
				_descriptionLabel.text = desc;
			}

			//if (shopData.cost.type == ItemType.RewardedVideo)
			//	//ToggleUnlock(AdsManager.isRewardedVideoAvailable);
			//else
			//	ToggleUnlock(true);
		}

		private void SetConfirmText(ShopData shopData)
		{
			// Cost
			CurrencyItem cost = shopData.cost;

			if (cost.type == ItemType.RewardedVideo) {

				if (_costAmountText != null)
					_costAmountText.text = string.Empty;
				_videoAdGO?.SetActive(true);
				_coinIconGO?.SetActive(false);
				_gemIconGO?.SetActive(false);
				_extraCurrencyIconGO?.SetActive(false);

			} else {

				if (_costAmountText != null)
					_costAmountText.text = shopData.costFullString;
				if (_videoAdGO != null)
					_videoAdGO.SetActive(false);
				switch (cost.type) {
					case ItemType.Coin:
					case ItemType.Gem:
					case ItemType.ExtraCurrency:
						_coinIconGO?.SetActive(cost.type == ItemType.Coin);
						_gemIconGO?.SetActive(cost.type == ItemType.Gem);
						_extraCurrencyIconGO?.SetActive(cost.type == ItemType.ExtraCurrency);
						break;

					case ItemType.Money:
						_coinIconGO?.SetActive(false);
						_gemIconGO?.SetActive(false);
						_extraCurrencyIconGO?.SetActive(false);
						break;
				}

			}
		}

		protected virtual void SetIcon(ShopData shopData)
		{
			if (_iconImage == null)
				return;
			_iconImage.sprite = shopData.iconSprite;
		}

		public void ToggleUnlock(bool isUnlock = true)
		{
			_lockMaskGO?.SetActive(!isUnlock);
		}

		#region Button Controls

		public void OnPurchaseClick()
		{
			ShopManager.TryPurchase(item.shopData, OnPurchaseSucceed, OnPurchaseFailed);
		}

		#endregion

		protected virtual void OnPurchaseSucceed()
		{
			MessageManager.CancelLastMessage(); // Cancel the last purchase confirm dialog

			ShopData shopData = item.shopData;

			if (shopData == null) // Just in case
				return;

			//if (shopData.noAds)
			//	AdsManager.RemoveAds(); // Just do one more to be sure

			//if (shopData.cost.type == ItemType.RewardedVideo)
			//	ToggleUnlock(AdsManager.isRewardedVideoAvailable);

			new Message {
				format = MessageFormat.Reward,
				title = LocalizeUtils.Get(TermCategory.Shop, shopData.cost.type == ItemType.RewardedVideo ? CommonTexts.MSG_VIDEO_ADS_COMPLETE_TITLE : CommonTexts.MSG_PURCHASE_SUCCEED_TITLE),
				content = LocalizeUtils.GetFormat(TermCategory.Shop, CommonTexts.MSG_RECEIVED_CONTENT, shopData.item.localizedString)
			}.Show();

			//if (shopData.cost.type == ItemType.RewardedVideo)
			//	ToggleUnlock(AdsManager.isRewardedVideoAvailable);
		}

		protected virtual void OnPurchaseFailed()
		{
			ShopData shopData = item.shopData;
			switch (shopData.cost.type) {
				case ItemType.Money: // IAP failed, show support hint
					new Message {
						title = LocalizeUtils.Get(TermCategory.Shop, CommonTexts.MSG_PURCHASE_FAILED_TITLE),
						content = LocalizeUtils.Get(TermCategory.Shop, CommonTexts.MSG_PURCHASE_FAILED_IAP_CONTENT),
					}.Show();
					break;
				case ItemType.RewardedVideo: // No video ads, do nothing
					break;
				default: // Show quick shop
					new Message {
						format = MessageFormat.QuickShop,
						cost = item.shopData.cost
					}.Show();
					break;
			}
		}

	}

}