using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

	/// <summary>
	/// A base item class that all item classes inherented to.
	/// Author: Richard
	/// </summary>
	public abstract class Item
	{

		// Generic amount number
		public virtual int amount { get; protected set; }

		// The amount string, override this for more accurate amount text
		public virtual string amountString => amount.ToString();

		// Try to convert class name to ItemType, e.g. ChestItem to ItemType.Chest.
		// Override this if you want to use class name different to ItemType, or avoid conversion every time, or using serialization (NOT RECOMMENDED!)
		// Note: Try to avoid using itemType if possible, e.g. use "if (myItem is ChestItem)" instead
		public virtual ItemType type {
			get {
				string itemTypeStr = GetType().ToString().Replace("Item", string.Empty).Replace("SweatyChair.", string.Empty);
				if (EnumUtils.IsDefined<ItemType>(itemTypeStr))
					return EnumUtils.Parse<ItemType>(itemTypeStr);
				return ItemType.None;
			}
			protected set {
			}
		}

		// The type string, override this if the item has its own type, e.g. consumables
		public virtual string typeString => type.ToString();

		// Try to convert class to a localized string, e.g. 5 Coins, 9 Cards
		// Override this if there's a custom localized string, or avoid conversion every time.
		public virtual string localizedString {
			get {
				TermCategory tc = TermCategory.Default;
				string termString = GetType().ToString().Replace("Item", string.Empty).Replace("SweatyChair.", string.Empty);
				if (EnumUtils.IsDefined<TermCategory>(termString))
					tc = EnumUtils.Parse<TermCategory>(termString);
				return LocalizeUtils.GetFormat("{0} {1}", amount, LocalizeUtils.GetPlural(tc, termString, amount));
			}
		}

		// Overrider this parameter if the item has icon sprite (used in leaderboard reward, redeem code, etc)
		public virtual Sprite iconSprite => null;

		// Overrider this parameter if the item has icon prefab, where a sprite is not enough for the item, such as Character, Pet
		public virtual GameObject iconPrefab => null;

		// Override this parameter if the item can be doubled
		public virtual bool canBeDoubled => false;

		#region Try Parse from given data values

		// Try parse an Item by given ItemType and amount
		public static bool TryParse(ItemType itemType, int amount, out Item bItem)
		{
			try {
				// Try get the Item from SerializableItem
				SerializableItem item = new SerializableItem(itemType, amount);
				bItem = item.ToItem();
				return true;
			} catch (Exception e) {
				Debug.LogFormat("Item:TryParse({0},{1}) - Error={2}", itemType, amount, e);
				bItem = null;
				return false;
			}
		}

		// Try parse an Item by given ItemType, amount and ID (can be parse to sub-type such as ResourceType)
		public static bool TryParse(ItemType itemType, int amount, int itemId, out Item item)
		{
			try {
				// Try get the Item from SerializableItem
				SerializableItem sItem = new SerializableItem(itemId, itemType, amount);
				item = sItem.ToItem();
				return true;
			} catch (Exception e) {
				Debug.LogFormat("Item:TryParse({0},{1},{2}) - Error={3}", itemType, amount, itemId, e);
				item = null;
				return false;
			}
		}

		// Try parse an Item by given ItemType, amount and ID (can be parse to sub-type such as ResourceType)
		public static bool TryParse(Hashtable ht, out Item item)
		{
			try {
				// Try get the Item from SerializableItem
				SerializableItem sItem = new SerializableItem(ht);
				item = sItem.ToItem();
				return true;
			} catch (Exception e) {
				Debug.LogFormat("Item:TryParse({0}) - Error={1}", ht, e);
				item = null;
				return false;
			}
		}

		#endregion

		#region Try Obtain from given data values

		public static bool TryObtain(ItemType itemType, int amount)
		{
			try {
				if (TryParse(itemType, amount, out Item item))
					return item.Obtain();
				return false;
			} catch (Exception e) {
				Debug.LogFormat("Item:TryObtain({0},{1}) - Error={2}", itemType, amount, e);
				return false;
			}
		}

		public static bool TryObtain(ItemType itemType, int amount, int itemId)
		{
			try {
				if (TryParse(itemType, amount, itemId, out Item item))
					return item.Obtain();
				return false;
			} catch (Exception e) {
				Debug.LogFormat("Item:TryObtain({0},{1},{2}) - Error={3}", itemType, amount, itemId, e);
				return false;
			}
		}

		#endregion

		#region Try Spend from given data values

		public static void TrySpend(ItemType itemType, int amount, UnityAction onSucceed, UnityAction onFailed = null)
		{
			try {
				if (TryParse(itemType, amount, out Item item))
					item.Spend(onSucceed, onFailed);
			} catch (Exception e) {
				Debug.LogFormat("Item:TrySpend({0},{1}) - Error={2}", itemType, amount, e);
			}
		}

		public static void TrySpend(ItemType itemType, int amount, int itemId, UnityAction onSucceed, UnityAction onFailed = null)
		{
			try {
				if (TryParse(itemType, amount, itemId, out Item item))
					item.Spend(onSucceed, onFailed);
			} catch (Exception e) {
				Debug.LogFormat("Item:TrySpend({0},{1},{2}) - Error={3}", itemType, amount, itemId, e);
			}
		}

		#endregion

		public Item Clone()
		{
			return (Item)MemberwiseClone();
		}

		// Override if item has ID and do the set logic here, used mostly by serialized item.
		public virtual void SetId(int id)
		{
		}

		// Override if item has ID string and do the set logic here, used mostly by rewarded video item.
		public virtual void SetId(string id)
		{
		}

		// Override to set the item data from a given hashtable.
		public virtual void Set(Hashtable ht)
		{
		}

		public virtual void AddAmount(int amount = 1)
		{
			if (amount < 0)
				return;
			SetAmount(this.amount + amount);
		}

		public virtual void SubtractAmount(int amount = 1)
		{
			if (amount < 0)
				return;
			SetAmount(this.amount - amount);
		}


		public virtual void SetAmount(int amount = 1)
		{
			this.amount = Mathf.Max(0, amount);
		}

		// Check if the item is obtainable, used mostly by serialized item.
		public virtual bool isObtainable {
			get { return GetType().GetMethod("Obtain").DeclaringType == GetType(); } // Return true only when Obtain() is overrided
		}

		// Check if the item is obtainable with failed message, used mostly by serialized item.
		public virtual bool CheckObtainable()
		{
			return isObtainable;
		}

		// Override this method if the item is obtainable.
		public virtual bool Obtain()
		{
			return true;
		}

		// Override this method if the item is obtainable, with event name for antlytics.
		public virtual bool Obtain(string eventName)
		{
			return true;
		}

		// Override this method if the item is spendable.
		public virtual void Spend(UnityAction onSucceed, UnityAction onFailed = null)
		{
		}

		// Override this method if the item can be converted to real money, mainly for displaying pack total value.
		public virtual float GetRealMoneyValue()
		{
			return 0;
		}

		// Overrider this method if the item can be display as a string (used in leaderboard reward, redeem code, etc).
		public virtual string ToDisplayString()
		{
			return string.Empty;
		}

		// Overrider this method if the item's amount can be display as a string (used in leaderboard reward, redeem code, etc).
		public virtual string ToAmountString()
		{
			return string.Empty;
		}


		// Override this method to print this item out for debugging.
		public override string ToString()
		{
			return string.Format("[Item: amount={0}]", amount);
		}

		public override bool Equals(object obj)
		{
			// Check for null and compare our run-time types
			if ((obj == null) || !this.GetType().Equals(obj.GetType()))
				return false;
			else {
				Item castItem = obj as Item;
				return castItem.type == type;
			}
		}

		public override int GetHashCode()
		{
			return type.GetHashCode();  // Since our item type is an int enum, we can directly use that as our hashcode, since there should never be a clash
		}

	}

}