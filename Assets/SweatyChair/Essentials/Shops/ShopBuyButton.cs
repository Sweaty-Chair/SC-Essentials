using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(Button))]
	public class ShopBuyButton : MonoBehaviour
	{

		[Tooltip("The ID of the shop data, set to 0 to disable and use product id and item instead")]
		[SerializeField] private int _shopDataId = 0;
		[Tooltip("The ID of the product, only used when shop data ID is set to 0")]
		[SerializeField] private string _productId = "";
		[Tooltip("Puchasing item, only used when shop data ID is set to 0")]
		[SerializeField] private SerializableItem _item;
		[Tooltip("Close shop on purchase succeed")]
		[SerializeField] private bool _closeShopOnPurchaseSucceed = true;
		[Tooltip("Notification icon, used for video reward mostly")]
		[SerializeField] private GameObject _notificationIcon;
		[Tooltip("Claimed icon,  used for video reward mostly")]
		[SerializeField] private GameObject _claimedIcon;

		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
			SetNotificationIcons();
		}

		private void SetNotificationIcons()
		{
			if (_shopDataId != 0) {
				if (_notificationIcon != null)
					_notificationIcon.SetActive(ShopManager.CanPurchase(_shopDataId));
				if (_claimedIcon != null)
					_claimedIcon.SetActive(!ShopManager.CanPurchase(_shopDataId));
			}
		}

		private void OnClick()
		{
			if (BillingManager.isPurchasing) // Avoid multiple transation at the same time
				return;
			if (_shopDataId == 0) { // Purchase with IAP product ID
				BillingManager.Purchase(_productId, () => {
					if (_closeShopOnPurchaseSucceed)
						ShopManager.ToggleUI(_item.type, false);
				});
			} else { // Purchase with shop ID
				ShopManager.TryPurchase(
					_shopDataId,
					() => {
						if (_closeShopOnPurchaseSucceed)
							ShopManager.ToggleUI(_item.type, false);
						SetNotificationIcons();
					}
				);
			}
		}

#if UNITY_EDITOR

		[ContextMenu("Reset Claim")]
		public void DebugResetClaim()
		{
			PlayerPrefs.DeleteKey(ShopManager.PREFS_REWARDED_VIDEO_CLAIM_COUNT + _shopDataId);
		}
#endif

	}

}