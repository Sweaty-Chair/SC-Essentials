using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SweatyChair
{

	/// <summary>
	/// A serialzable item used in Editor, scriptable object settings, etc.
	/// It will be parsed to corresponding item class using type.
	/// </summary>
	[System.Serializable]
	public class SerializableItem : Item
	{

		[SerializeField] protected int _id;
		[SerializeField] protected ItemType _type = ItemType.Coin;
		[SerializeField] protected int _amount;
		[SerializeField] protected float _floatAmount;

		protected Hashtable _ht;

		public int id {
			get { return _id; }
			protected set { _id = value; }
		}

		public override ItemType type {
			get { return _type; }
			protected set { _type = value; }
		}

		public override int amount {
			get { return _amount; }
			protected set { _amount = value; }
		}

		public float floatAmount {
			get { return _floatAmount; }
			protected set { _floatAmount = value; }
		}

		public override Sprite iconSprite {
			get {
				try {
					Item item = ToItem();
					if (item != null) // Ignore if cannot be parsed to other
						return item.iconSprite;
				} catch (Exception e) {
					Debug.LogFormat("SerializableItem:iconSprite - Error={0}", e);
				}
				return null;
			}
		}

		public override GameObject iconPrefab {
			get {
				try {
					Item item = ToItem();
					if (item != null) // Ignore if cannot be parsed to other
						return item.iconPrefab;
				} catch (Exception e) {
					Debug.LogFormat("SerializableItem:iconPrefab - Error={0}", e);
				}
				return null;
			}
		}

		/// <summary>
		/// Can the item be double, used for watch ad doubling rewards.
		/// </summary>
		public override bool canBeDoubled {
			get {
				Item item = ToItem();
				return item != null && item.canBeDoubled;
			}
		}

		public override string amountString
		{
			get {
				try {
					Item item = ToItem();
					if (item != null) // Ignore if cannot be parsed to other
						return item.amountString;
				}
				catch (Exception e) {
					Debug.LogFormat("SerializableItem:iconPrefab - Error={0}", e);
				}
				return null;
			}
		}

		public SerializableItem() : this(ItemType.Coin, 1)
		{
		}

		public SerializableItem(ItemType itemType, int amount) : this(0, itemType, amount)
		{
		}

		public SerializableItem(int id, ItemType itemType = ItemType.Inventory, int amount = 1)
		{
			_id = id;
			_type = itemType;
			_amount = amount;
			_floatAmount = amount;
		}

		public SerializableItem(Hashtable ht)
		{
			_id = ht.ContainsKey("id") ? DataUtils.GetInt(ht["id"]) : 0;
			_type = ht.ContainsKey("itemType") ? EnumUtils.Parse<ItemType>((string)ht["itemType"]) : ItemType.None;
			_amount = ht.ContainsKey("amount") ? DataUtils.GetInt(ht["amount"]) : 0;
			_floatAmount = ht.ContainsKey("floatAmount") ? DataUtils.GetInt(ht["floatAmount"]) : 0;
			_ht = ht;
		}

		public override void SetId(int id)
		{
			_id = id;
		}

		public void SetItemType(ItemType type)
		{
			this.type = type;
		}

		// Check if the item is obtainable, used mostly by serialized item
		public override bool isObtainable {
			get {
				try {
					Item item = ToItem();
					if (item != null) // Ignore if cannot be parsed to other
						return item.isObtainable;
				} catch (Exception e) {
					Debug.LogFormat("SerializableItem:isObtainable - Error={0}", e);
				}
				return false;
			}
		}

		public override bool CheckObtainable()
		{
			try {
				Item item = ToItem();
				if (item != null) // Ignore if cannot be parsed to another Item class
					return item.CheckObtainable();
			} catch (Exception e) {
				Debug.LogFormat("SerializableItem:CheckObtainable - Error={0}", e);
			}
			return false;
		}

		public override bool Obtain()
		{
			try {
				Item item = ToItem();
				if (item != null) // Ignore if cannot be parsed to another Item class
					item.Obtain();
			} catch (Exception e) {
				Debug.LogFormat("SerializableItem:Obtain - Error={0}", e);
			}
			return false;
		}
		public override bool Obtain(string eventName)
		{
			try {
				Item item = ToItem();
				if (item != null) // Ignore if cannot be parsed to another Item class
					item.Obtain(eventName);
			} catch (Exception e) {
				Debug.LogFormat("SerializableItem:Obtain - Error={0}", e);
			}
			return false;
		}

		public override string ToDisplayString()
		{
			try {
				Item item = ToItem();
				if (item != null) // Ignore if cannot be parsed to another Item class
					return item.ToDisplayString();
			} catch (Exception e) {
				Debug.LogFormat("SerializableItem:ToDisplayString - Error={0}", e);
			}
			return string.Empty;
		}

		public override string ToAmountString()
		{
			try {
				Item item = ToItem();
				if (item != null) // Ignore if cannot be parsed to another Item class
					return item.ToAmountString();
			} catch (Exception e) {
				Debug.LogFormat("SerializableItem:ToDisplayString - Error={0}", e);
			}
			return string.Empty;
		}

		/// <summary>
		/// Parse the currenct class to another Item class, using item type. E.g. CoinItem if type=Coin, EneryItem if type=Energy.
		/// </summary>
		public Item ToItem()
		{
			// Add SweatyChair namespace
			string typeName = "SweatyChair." + this.type.ToString() + "Item";
			Type type = Type.GetType(typeName, false);

			// Try without SweatyChair namespace
			if (type == null) {
				typeName = this.type.ToString() + "Item";
				type = Type.GetType(typeName, false);
			}

			// Try use type key in hashtable
			if (type == null && _ht != null) {
				typeName = _ht.ContainsKey("type") ? (string)_ht["type"] : string.Empty;
				type = Type.GetType(typeName, false);
			}

			// Not valid type found, cannot parse to another Item
			if (type == null) {
				//throw new InvalidOperationException(string.Format("{0} is not a known ItemType", typeName));
				Debug.LogError(string.Format("{0} is not a known ItemType", typeName));
				return null;
			}

			if (!typeof(Item).IsAssignableFrom(type)) {
				//throw new InvalidOperationException(string.Format("{0} does not inherit from Item", typeName));
				Debug.LogError(string.Format("{0} does not inherit from Item", typeName));
				return null;
			}

			Item item = (Item)Activator.CreateInstance(type);

			if (item != null) {
				if (_ht != null) {
					item.Set(_ht);
				} else {
					item.SetId(id);
					item.SetAmount(amount);
				}
			}

			return item;
		}

		public override string ToString()
		{
			return string.Format("[Item: id={0}, itemType={1}, amount={2}]", id, type, amount);
		}

	}

	public static class SerializableItemExt
	{

		/// <summary>
		/// Converts an array of SerializableItem to an array of Item classes.
		/// </summary>
		public static Item[] ToItems(this SerializableItem[] serializableItems)
		{
			Item[] items = new Item[serializableItems.Length];
			for (int i = 0, iMax = serializableItems.Length; i < iMax; i++)
				items[i] = serializableItems[i].ToItem();
			return items;
		}

		/// <summary>
		/// Converts a list of SerializableItem to a list of Item classes.
		/// </summary>
		public static List<Item> ToItems(this List<SerializableItem> serializableItemList)
		{
			List<Item> itemList = new List<Item>();
			foreach (SerializableItem serializableItem in serializableItemList)
				itemList.Add(serializableItem.ToItem());
			return itemList;
		}

	}

}