using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class CurrencyPanel : Panel
	{

		[SerializeField] private Text _coinText, _gemsText, _extraCurrencyText;

		private void Awake()
		{
			CurrencyManager.coinsChanged += SetCoins;
			CurrencyManager.gemsChanged += SetGems;
			CurrencyManager.extraCurrencyChanged += SetExtraCurrency;
		}

		private void OnEnable()
		{
			SetCoins(CurrencyManager.coinCount);
			SetGems(CurrencyManager.coinCount);
			SetExtraCurrency(CurrencyManager.coinCount);
		}

		private void SetCoins(int count)
		{
			_coinText.text = count.ToString();
		}

		private void SetGems(int count)
		{
			_gemsText.text = count.ToString();
		}

		private void SetExtraCurrency(int count)
		{
			_extraCurrencyText.text = count.ToString();
		}

		private void Destroy()
		{
			CurrencyManager.coinsChanged -= SetCoins;
			CurrencyManager.gemsChanged -= SetGems;
			CurrencyManager.extraCurrencyChanged -= SetExtraCurrency;
		}

	}

}