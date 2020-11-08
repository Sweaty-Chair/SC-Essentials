namespace SweatyChair
{

	/// <summary>
	/// A item class that referenced to in-game coins.
	/// Author: Richard
	/// </summary>
	public class CoinItem : CurrencyItem
	{

		public CoinItem() : base(ItemType.Coin)
		{
		}

		public CoinItem(int amount) : base(ItemType.Coin, amount)
		{
		}

	}

}