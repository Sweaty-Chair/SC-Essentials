namespace SweatyChair
{

	/// <summary>
	/// A item class that referenced to in-game extra-currency.
	/// Author: Richard
	/// </summary>
	public class ExtraCurrencyItem : CurrencyItem
	{

		public ExtraCurrencyItem()
			: base(ItemType.ExtraCurrency)
		{
		}

		public ExtraCurrencyItem(int amount)
			: base(ItemType.ExtraCurrency, amount)
		{
		}

	}

}