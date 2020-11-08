using UnityEngine;

namespace SweatyChair.UI
{

	public class ShopPanel : Panel
	{

		[Tooltip("The item type this shop showing, such as Coins, Gems, or None")]
		[SerializeField] private ItemType _itemType = ItemType.Coin;

		public override void Init()
		{
			ShopManager.uiToggled += Toggle;
		}

		public override void Reset()
		{
			ShopManager.uiToggled -= Toggle;
		}

		private void Toggle(ItemType itemType, bool show)
		{
			if (_itemType == itemType)
				base.Toggle(show);
			else
				Hide(); // Make sure this shop panels is hiden if not showing it
		}

		public override void OnBackClick()
		{
			if (BillingManager.isPurchasing) // Avoid multiple transation at the same time
				return;
			ShopManager.ToggleUI(_itemType, false);
		}

	}

}