using SweatyChair.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair
{

	public class ShopPurchaseButton : MonoBehaviour
	{

		#region Variables

		[Header("UI")]
		[SerializeField] protected Text _productName;
		[SerializeField] protected Text _productDescription;
		[SerializeField] protected Image _productIcon;
		[Space]
		[SerializeField] protected GameObject _rewardSpriteParent;
		[SerializeField] protected Image _rewardSprite;
		[SerializeField] protected Text _rewardAmount;
		[Space]
		[SerializeField] protected GameObject _costSpriteParent;
		[SerializeField] protected Image _costSprite;
		[SerializeField] protected Text _costAmount;

		[Header("Settings")]
		[SerializeField] protected int _shopDataID;
		[Space]
		[SerializeField] protected bool _closeShopOnPurchaseSucceed = true;

		private Button _button;

		#endregion

		#region Awake / OnDestroy

		protected virtual void Awake()
		{
			if (_button == null)
				_button = GetComponent<Button>();

			if (_button != null)
				_button.onClick.AddListener(OnButtonClick);

			// Finally Initialize our UI
			InitializeUI();
		}

		protected virtual void OnDestroy()
		{
			if (_button != null)
				_button.onClick.RemoveListener(OnButtonClick);
		}


		protected virtual void InitializeUI()
		{
			// Get our Data
			ShopData shopData = ShopDatabase.GetData(_shopDataID);
			if (shopData == null) {
				Debug.LogError($"ShopPurchaseButton: Unable to Initialize our UI. Shop Data id '{_shopDataID}' is not a valid id. Please check it is correct", this);
				return;
			}

			// Product Info
			if (_productName != null)
				_productName.text = shopData.localizedName;

			if (_productDescription != null)
				_productDescription.text = shopData.localizedDescription;

			if (_productIcon != null)
				_productIcon.sprite = shopData.iconSprite;

			// Reward Visuals
			if (_rewardSpriteParent != null)
				_rewardSpriteParent.SetActive(shopData.item.iconSprite != null);
			if (_rewardSprite != null)
				_rewardSprite.sprite = shopData.item.iconSprite;

			if (_rewardAmount != null)
				_rewardAmount.text = shopData.item.amountString;

			// Cost Visuals
			if (_costSpriteParent != null)
				_costSpriteParent.SetActive(shopData.cost.iconSprite != null);
			if (_costSprite != null)
				_costSprite.sprite = shopData.cost.iconSprite;

			if (_costAmount != null) {
				_costAmount.gameObject.SetActive(shopData.cost.amount != 0);
				_costAmount.text = shopData.cost.amountString;
			}
		}

		#endregion

		#region Event Callbacks

		protected virtual void OnButtonClick()
		{
			// If we are already purchasing, avoid multiple transaction
			if (BillingManager.isPurchasing)
				return;

			ShopData shopData = ShopDatabase.GetData(_shopDataID);
			ShopManager.TryPurchase(shopData, PurchaseSucceed, null);
		}

		protected virtual void PurchaseSucceed()
		{
			if (_closeShopOnPurchaseSucceed) {
				ShopData shopData = ShopDatabase.GetData(_shopDataID);
				ShopManager.ToggleUI(shopData.item.type, false);
			}
		}

		#endregion

	}

}
