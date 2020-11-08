namespace SweatyChair
{

	/// <summary>
	/// A wrapper that wraps a ShopData, and can be used as generic item.
	/// </summary>
	public class ShopItem : DataItem
	{

		public ShopData shopData => (ShopData)data;

		public override ItemType type => shopData.item.type;

		public override int amount => shopData.item.amount;

		public ShopItem(ShopData data)
			: base(data)
		{
		}

		// Temporily set the reward ID, only used for direcly buy card, use it with cautions
		public override void SetId(int id)
		{
			shopData.item.SetId(id);
		}

		public override bool Obtain()
		{
			return shopData.item.Obtain();
		}

		public override string ToString()
		{
			return string.Format("[ShopItem: shopData={0}]", shopData);
		}

	}

}