using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair {

	public class UICurrency : MonoBehaviour {

		#region Enum

		public enum CurrencyType {
			Coin,
			Gem,
			Extra
		}

		#endregion

		#region Variables

		[Header("Required")]
		[SerializeField] private Text _amountText;

		[Header("Settings")]
		[SerializeField] private CurrencyType _currencyType;

		#endregion

		#region Awake / OnDestroy

		private void Awake() {
			// Get our Component
			if (_amountText == null)
				_amountText = GetComponent<Text>();
		}

		#endregion

		#region OnEnable / OnDisable

		private void OnEnable() {
			CurrencyManager.coinsChanged += OnCoinsChanged;
			CurrencyManager.gemsChanged += OnGemsChanged;
			CurrencyManager.extraCurrencyChanged += OnExtraCurrencyChanged;

			// Then force our UI for update
			UpdateUI();
		}

		private void OnDisable() {
			CurrencyManager.coinsChanged -= OnCoinsChanged;
			CurrencyManager.gemsChanged -= OnGemsChanged;
			CurrencyManager.extraCurrencyChanged -= OnExtraCurrencyChanged;
		}

		#endregion

		#region Event Callbacks

		public void OnClickTakeToShop() {
			ShopManager.ToggleUI(GetItemTypeFromCurrency(_currencyType));
		}

		private void OnCoinsChanged(int amount) {
			if (_currencyType == CurrencyType.Coin)
				UpdateUI();
		}

		private void OnGemsChanged(int amount) {
			if (_currencyType == CurrencyType.Gem)
				UpdateUI();
		}

		private void OnExtraCurrencyChanged(int amount) {
			if (_currencyType == CurrencyType.Extra)
				UpdateUI();
		}

		#endregion

		#region Update UI

		private void UpdateUI() {
			switch (_currencyType) {
				case CurrencyType.Coin:
					_amountText.text = CurrencyManager.coinCount.ToString();
					break;

				case CurrencyType.Gem:
					_amountText.text = CurrencyManager.gemCount.ToString();
					break;

				case CurrencyType.Extra:
					_amountText.text = CurrencyManager.extraCurrencyCount.ToString();
					break;

				default:
					Debug.LogError($"Cannot convert input '{_currencyType}' to a displayable currency.");
					break;
			}
		}

		#endregion

		#region Utility

		private ItemType GetItemTypeFromCurrency(CurrencyType currencyType) {
			switch (currencyType) {
				case CurrencyType.Coin:
					return ItemType.Coin;
				case CurrencyType.Gem:
					return ItemType.Gem;
				case CurrencyType.Extra:
					return ItemType.ExtraCurrency;
				default:
					return ItemType.None;
			}
		}

		#endregion

	}

}
