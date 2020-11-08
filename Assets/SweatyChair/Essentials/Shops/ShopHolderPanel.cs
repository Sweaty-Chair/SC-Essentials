using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	/// <summary>
	/// A holder panel that hold number of shop panels.
	/// </summary>
	public class ShopHolderPanel : Panel
	{

		#region Variables

		[SerializeField] private Toggle[] _tabToggles;
		[SerializeField] private Panel[] _shopPanels;

		#endregion

		#region Init / Reset

		public override void Init()
		{
			ShopManager.uiToggled += OnShopToggled;
			foreach (Panel shopPanel in _shopPanels)
				shopPanel.Init();
		}

		public override void Reset()
		{
			ShopManager.uiToggled -= OnShopToggled;
			foreach (Panel shopPanel in _shopPanels)
				shopPanel.Reset();
		}

		#endregion

		#region Event Callbacks

		private void OnShopToggled(ItemType itemType, bool show)
		{
			base.Toggle(show);
			if (show) {
				ShopShowToggle[] toggles = GetComponentsInChildren<ShopShowToggle>();
				foreach (ShopShowToggle toggle in toggles) {
#if UNITY_2019_1_OR_NEWER
					toggle.GetComponent<Toggle>().SetIsOnWithoutNotify(toggle.itemType == itemType);
#else
					toggle.GetComponent<Toggle>().Set(toggle.itemType == itemType, false);
#endif
				}
			}
		}

		#endregion

	}

}