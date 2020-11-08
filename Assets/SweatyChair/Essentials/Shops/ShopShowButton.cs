using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(Button))]
	public class ShopShowButton : MonoBehaviour
	{

		[Tooltip("The item type of shop to show, such as Coins, Gems. If set to None, open the last opened shop.")]
		[SerializeField] private ItemType _itemType = ItemType.Coin;

		private ItemType _lastOpenedShopType = ItemType.Coin;

		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
			ShopManager.uiToggled += OnShopToggle;
		}

		private void OnDestroy()
		{
			ShopManager.uiToggled -= OnShopToggle;
		}

		private void OnShopToggle(ItemType itemType, bool show)
		{
			if (show)
				_lastOpenedShopType = itemType;
		}

		private void OnClick()
		{
			if (_itemType == ItemType.None)
				ShopManager.ToggleUI(_lastOpenedShopType);
			else
				ShopManager.ToggleUI(_itemType);
		}

	}

}