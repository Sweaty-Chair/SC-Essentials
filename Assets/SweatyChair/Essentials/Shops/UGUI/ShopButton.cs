using SweatyChair;
using UnityEngine;
using UnityEngine.UI;

namespace JiggleTD
{

	public class ShopButton : MonoBehaviour
	{

		#region Variables

		[Header("Settings")]
		[SerializeField] private ItemType _itemType = ItemType.None;

		private Button _button;

		#endregion

		#region OnEnable / OnDisable

		private void OnEnable()
		{
			if (_button == null)
				_button = GetComponent<Button>();

			// If we have a button
			if (_button != null)
				_button.onClick.AddListener(OnButtonClicked);
		}

		private void OnDisable()
		{
			// If we still have a button remove our listener
			if (_button != null)
				_button.onClick.RemoveListener(OnButtonClicked);

			_button = null;
		}

		#endregion

		#region Event Callbacks

		private void OnButtonClicked()
		{
			ShopManager.ToggleUI(_itemType);
		}

		#endregion

	}

}