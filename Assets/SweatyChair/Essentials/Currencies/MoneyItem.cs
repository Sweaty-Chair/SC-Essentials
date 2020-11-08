using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A item class that referenced to real money.
	/// Author: Richard
	/// </summary>
	public class MoneyItem : CurrencyItem
	{

		public ShopData shopData { get; private set; }

		public override Sprite iconSprite => null;

		public override string amountString => LocalizeUtils.GetFormat("${0}", shopData.cost.floatAmount);

		public MoneyItem() : base(ItemType.Money)
		{
		}

		public MoneyItem(int amount) : base(ItemType.Money, amount)
		{
			shopData = ShopDatabase.GetData(amount);
		}

		public MoneyItem(float floatAmount) : base(ItemType.Money, floatAmount)
		{
		}

	}

}