namespace SweatyChair
{

	/// <summary>
	/// A item class that referenced to in-game gems.
	/// Author: Richard
	/// </summary>
	public class GemItem : CurrencyItem
	{

		public GemItem()
			: base(ItemType.Gem)
		{
		}

		public GemItem(int amount)
			: base(ItemType.Gem, amount)
		{
		}

	}

}