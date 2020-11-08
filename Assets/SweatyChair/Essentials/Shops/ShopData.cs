using UnityEngine;

namespace SweatyChair
{

	public class ShopData : BaseData
	{

		public string name { get; private set; }

		public Item item { get; private set; }

		public string productId { get; private set; }

		public CurrencyItem cost { get; private set; }

		public bool noAds { get; private set; }

		public string description { get; private set; }

		public string iconResourcePath { get; private set; }

		public float valueRate => (float)item.amount / (float)cost.floatAmount;

		public string localizedName => LocalizeUtils.Get(TermCategory.Shop, name);

		public string localizedDescription => LocalizeUtils.Get(TermCategory.Shop, description);

		public string costFullString { // E.g. $0.99, 5 Gems
			get {
#if UNITY_ANDROID && CHS
				return LocalizeUtils.GetFormat("${0}", BillingManager.USDToCNY(cost.amount));
#endif
				if (cost.type == ItemType.Money) // Try get the live app store price
					return BillingManager.GetProductPriceString(productId, cost.amountFullString);
				return cost.amountFullString;
			}
		}

		private Sprite _iconSprite;
		public Sprite iconSprite {
			get {
				if (_iconSprite == null) // Lazy loading
					_iconSprite = Resources.Load<Sprite>(iconResourcePath);
				return _iconSprite;
			}
		}

		public ShopData() : base()
		{
		}

		public ShopData(Item item, CurrencyItem cost) : base()
		{
			this.item = item;
			this.cost = cost;
		}

		public ShopData(string name, Item item, CurrencyItem cost) : this(item, cost)
		{
			this.name = name;
		}

		public override bool FeedData(string[] data)
		{
			try {

				int i = 0;

				id = DataUtils.GetInt(data[i++]);
				name = data[i++];

				ItemType itemType = EnumUtils.Parse<ItemType>(data[i++]);
				int itemAmount = DataUtils.GetInt(data[i++]);
				int itemId = DataUtils.GetInt(data[i++]);
				item = new SerializableItem(itemId, itemType, itemAmount).ToItem();

				string productIdiOS = data[i++];
				string productIdAndroid = data[i++];

#if UNITY_IOS
				productId = productIdiOS;
#elif UNITY_ANDROID
				productId = !string.IsNullOrEmpty(productIdAndroid) ? productIdAndroid : productIdiOS;
#endif
				ItemType currencyType = EnumUtils.Parse<ItemType>(data[i++]);
				float costAmount = DataUtils.GetFloat(data[i++]); // Use float because can be real money
				cost = new CurrencyItem(currencyType, costAmount);

				noAds = DataUtils.GetBool(data[i++]);
				description = data[i++];
				iconResourcePath = data[i++];

				return true;

			} catch (System.Exception e) {

				Debug.LogErrorFormat("{0}:FeedData - Error={1}", GetType(), e);
				return false;

			}
		}

		public override string ToString()
		{
			return string.Format("[ShopData: id={0}, name={1}, item={2}, productId={3}, cost={4}, noAds={5}, description={6}]", id, name, item, productId, cost, noAds, description);
		}

	}

}