using UnityEngine;

namespace SweatyChair.UI
{

	public class FirstPurchasePanel : Panel
	{

		[Tooltip("The item type of shop to show, such as Coins, Gems, or None")]
		[SerializeField] private ItemType _shopItemType = ItemType.Coin;

		[SerializeField] private GameObject _goPurchaseButtonGO, _collectButtonGO;

		public override void Init()
		{
			FirstPurchaseManager.uiToggled += Toggle;
			ShopManager.uiToggled += CheckPurchased;
		}

		public override void Reset()
		{
			FirstPurchaseManager.uiToggled -= Toggle;
			ShopManager.uiToggled -= CheckPurchased;
		}

		public override void Toggle(bool show)
		{
			base.Toggle(show);
			if (show)
				SetButtons();
		}

		private void SetButtons()
		{
			_goPurchaseButtonGO.SetActive(!ShopManager.hasPaidRealMoney);
			_collectButtonGO.SetActive(ShopManager.hasPaidRealMoney);
		}

		private void CheckPurchased(ItemType itemType, bool show)
		{
			if (!show && isShown) // Refresh the buttons while shop close
				SetButtons();
		}

		public void OnGoPurchaseClick()
		{
			ShopManager.ToggleUI(_shopItemType);
			FirstPurchaseManager.ToggleUI(false);
		}

		public void OnCollectClick()
		{
			if (FirstPurchaseSettings.current.hidePanelAfterCollect)
				FirstPurchaseManager.ToggleUI(false);
			FirstPurchaseManager.Reward();
		}

	}

}