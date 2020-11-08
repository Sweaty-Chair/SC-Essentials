using UnityEngine;

namespace SweatyChair.UI
{

	public class ShopBuyNotification : MonoBehaviour
	{

		[Tooltip("The ID of the shop data, set to 0 to disable and use product id and item instead")]
		[SerializeField] private int _shopDataId = 0;
		[Tooltip("Notification icon, used for video reward mostly")]
		[SerializeField] private GameObject _notificationIcon;

		private void Awake()
		{
			SetNotificationIcon();
			ShopManager.purchaseCompleted += SetNotificationIcon;
		}

		private void OnDestroy()
		{
			ShopManager.purchaseCompleted -= SetNotificationIcon;
		}

		private void SetNotificationIcon(ShopData dump)
		{
			SetNotificationIcon();
		}
		private void SetNotificationIcon()
		{
			if (_notificationIcon != null)
				_notificationIcon.SetActive(ShopManager.CanPurchase(_shopDataId));
		}

	}

}