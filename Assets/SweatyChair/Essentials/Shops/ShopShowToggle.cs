using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(Toggle))]
	public class ShopShowToggle : MonoBehaviour
	{

		[Tooltip("The item type of shop to show, such as Coins, Gems, or None")]
		[SerializeField] private ItemType _itemType = ItemType.Coin;

		public ItemType itemType => _itemType;

		private void Awake()
		{
			GetComponent<Toggle>().onValueChanged.AddListener(OnValueChanged);
		}

		private void OnValueChanged(bool isToggled)
		{
			if (isToggled) ShopManager.ToggleUI(_itemType);
		}

	}

}