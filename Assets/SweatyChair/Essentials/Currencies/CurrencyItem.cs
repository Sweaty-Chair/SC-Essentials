using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

	/// <summary>
	/// A item class that referenced to either in-game coins, gems, extraCurrency or real money.
	/// Author: Richard
	/// </summary>
	[System.Serializable]
	public class CurrencyItem : Item
	{

		// Listen to this event for any obtain effect
		public static event UnityAction<Item> obtained;

		// Serialize itemType so it can be used in rewards, cost, etc
		[SerializeField] protected ItemType _itemType;
		public override ItemType type { get { return _itemType; } protected set { _itemType = value; } }

		protected Sprite _iconSprite;
		public override Sprite iconSprite {
			get {
				if (_iconSprite == null) {
					string stringPath = GetType().ToString().Replace("Item", string.Empty).Replace("SweatyChair.", string.Empty);
					_iconSprite = Resources.Load<Sprite>(stringPath);
					if (_iconSprite == null)
						_iconSprite = Resources.Load<Sprite>(type.ToString());
				}
				return _iconSprite;
			}
		}

		// Set our item to be able to be doubled
		public override bool canBeDoubled => true;

		// Serialize amount so it can be used in rewards, cost, etc
		[SerializeField] protected int _amount;
		public override int amount { get { return _amount; } protected set { _amount = value; } }

		[SerializeField] protected float _floatAmount;
		public float floatAmount {
			get {
				if (_floatAmount <= 0 && amount > 0) return amount;
				return _floatAmount;
			}
			protected set { _floatAmount = value; }
		}

		public float currencyAmount {
			get {
#if CHS
				if (LocalizeUtils.currentLanguage == Language.ChineseSimplified) // Hack: Chinese publisher don't like any other currency, mannualy convert from USD to CNY
					return BillingManager.USDToCNY(floatAmount);
#endif
				return floatAmount;
			}
		}

		public override string amountString { // E.g. USD $0.99, 5
			get {
				if (type == ItemType.Money) { // Avoid using amountString for real money, use ShopData.costFullString instead for live currency price
					if (LocalizeUtils.currentLanguage == Language.ChineseSimplified) // Hack: change simplified chinese from USD to CNY
						return LocalizeUtils.GetFormat("${0}", currencyAmount);
					return string.Format("${0}", floatAmount);
				}
				return floatAmount.ToString();
			}
		}

		public string amountFullString { // E.g. USD $0.99, 5 Gems (Localized)
			get {
				if (type == ItemType.Money)
					return amountString;
				if (type == ItemType.RewardedVideo)
					return LocalizeUtils.GetFormat(TermCategory.Default, "{0}", LocalizeUtils.GetPlural(TermCategory.Shop, type.ToString(), amount));
				return LocalizeUtils.GetFormat(TermCategory.Default, "{0} {1}", floatAmount, LocalizeUtils.GetPlural(TermCategory.Shop, type.ToString(), amount));
			}
		}

		public string typeLocalizedString => LocalizeUtils.GetPlural(TermCategory.Shop, type, (int)floatAmount);

		public CurrencyItem() : this(ItemType.Coin, 1)
		{
		}

		public CurrencyItem(int itemTypeInt, float floatAmount)
			: this((ItemType)itemTypeInt, floatAmount)
		{
		}

		public CurrencyItem(ItemType itemType)
			: this(itemType, 1)
		{
		}

		public CurrencyItem(ItemType type, int amount)
		{
			this.type = type;
			this.amount = amount;
			floatAmount = (float)amount;
		}

		public CurrencyItem(ItemType itemType, float floatAmount)
		{
			type = itemType;
			amount = Mathf.RoundToInt(floatAmount);
			this.floatAmount = floatAmount;
		}

		public CurrencyItem(Item item)
		{
			type = item.type;
			amount = item.amount;
			floatAmount = (float)amount;
		}

		public void AddFloatAmount(float amount)
		{
			SetFloatAmount(floatAmount + amount);
		}

		public override void SetAmount(int amount = 1)
		{
			if (amount < 0) return;
			this.amount = amount;
			floatAmount = amount;
		}

		public void SetFloatAmount(float floatAmount)
		{
			amount = (int)floatAmount;
			this.floatAmount = floatAmount;
		}

		public override bool isObtainable => true; // Override and make sure it's obtainable

		public override bool Obtain()
		{
			CurrencyManager.AddCurrency(this);
			obtained?.Invoke(this);
			return true;
		}

		public override bool Obtain(string eventName)
		{
			CurrencyManager.AddCurrency(this, eventName);
			obtained?.Invoke(this);
			return true;
		}

		public override void Spend(UnityAction onSucceed, UnityAction onFailed = null)
		{
			CurrencyManager.SpendCurrency(this, onSucceed, onFailed);
		}

		public override string ToAmountString()
		{
			return amount.ToString();
		}

		public override string ToDisplayString()
		{
			return amountFullString;
		}

		public override string ToString()
		{
			return string.Format("[CurrencyItem: itemType={0}, floatAmount={1}]", type, floatAmount);
		}

	}

}