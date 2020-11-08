using UnityEngine;
using System.Collections.Generic;
using SweatyChair.UI;

namespace SweatyChair
{

	// A class holding a list of items, and contrains function to reward them.
	[System.Serializable]
	public class Reward
	{

		[SerializeField] private SerializableItem[] _items;

		public SerializableItem[] items => _items;

		public int count {
			get { return _items == null ? 0 : _items.Length; }
			set { _items = items.Resize(value); }
		}

		public Reward()
		{
			_items = new SerializableItem[1];
		}

		public Reward(SerializableItem[] items)
		{
			_items = items;
		}

		public Reward(List<SerializableItem> items)
			: this(items.ToArray())
		{
		}

		public Reward(SerializableItem item)
			: this(new SerializableItem[]{ item })
		{
		}

		public void AddItem(SerializableItem item)
		{
			List<SerializableItem> itemList = new List<SerializableItem>(_items) {
				item
			};
			_items = itemList.ToArray();
		}

		/// <summary>
		/// Output if the reward can be obtained, e.g. chest reward is not obtainable while chest slots is full
		/// </summary>
		public bool isObtainable {
			get {
				foreach (SerializableItem item in _items) {
					if (!item.isObtainable)
						return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Check if the reward can be obtain with message
		/// </summary>
		public bool CheckObtainable()
		{
			foreach (SerializableItem item in _items) {
				if (!item.CheckObtainable())
					return false;
			}
			return true;
		}

		/// <summary>
		/// Obtain all reward items with a message.
		/// </summary>
		public void Claim(string messageTitle = "")
		{
			string rewardString = "";
			List<Item> itemList = new List<Item>();
			foreach (SerializableItem item in _items) {
				item.Obtain();
				itemList.Add(item.ToItem());
				string displayString = item.ToDisplayString();
				if (!string.IsNullOrEmpty(displayString))
					rewardString += LocalizeUtils.GetFormat(TermCategory.Shop, "Received {0}.", displayString) + "\n";
			}
			new Message {
				format = MessageFormat.Reward,
				title = messageTitle,
				content = rewardString,
				rewardItemList = itemList
			}.Show();
		}

		/// <summary>
		/// Obtain all reward items without showing message.
		/// </summary>
		public void Obtain()
		{
			foreach (SerializableItem item in _items)
				item.Obtain();
		}
		public void Obtain(string eventName)
		{
			foreach (SerializableItem item in _items)
				item.Obtain(eventName);
		}

		/// <summary>
		/// Obtain all reward items that can be doubled only. This is used before watch ads for undoubled item, e.g. character.
		/// </summary>
		public void ObtainCanBeDouble()
		{
			foreach (SerializableItem item in _items) {
				if (item.canBeDoubled)
					item.Obtain();
			}
		}
		public void ObtainCanBeDouble(string eventName)
		{
			foreach (SerializableItem item in _items) {
				if (item.canBeDoubled)
					item.Obtain(eventName);
			}
		}

		/// <summary>
		/// Obtain double amount of the reward items, used with rewarded video ads.
		/// </summary>
		public void ObtainDouble()
		{
			foreach (SerializableItem item in _items) {
				item.SetAmount(item.amount * 2);
				item.Obtain();
			}
		}
		public void ObtainDouble(string eventName)
		{
			foreach (SerializableItem item in _items) {
				item.SetAmount(item.amount * 2);
				item.Obtain(eventName);
			}
		}

		/// <summary>
		/// Can the reward be double, true if any of the reward items can be doubled.
		/// </summary>
		public bool canBeDoubled {
			get {
				foreach (var item in items) {
					if (item.canBeDoubled)
						return true;
				}
				return false;
			}
		}

		public override string ToString()
		{
			return string.Format("[Reward: items={0}]", StringUtils.ArrayToString(items));
		}

	}

}