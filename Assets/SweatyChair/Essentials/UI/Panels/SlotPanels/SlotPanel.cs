using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair.UI
{

	public abstract class SlotPanel<T> : Panel
	{

		#region Variables

		[Header("Slot UI")]
		[SerializeField] protected RectTransform _slotHolder;
		[SerializeField] protected GameObject _slotPrefab;


		protected List<Slot<T>> _slotList = new List<Slot<T>>();

		#endregion

		#region Initialize

		/// <summary>
		/// Initializes a collection of slots. 
		/// </summary>
		/// <param name="items"></param>
		protected virtual void InitSlots(IEnumerable<T> items)
		{
			int indexer = 0;

			// Create UI
			foreach (var item in items) {

				// Init slot
				var slot = GetSlot(indexer);
				if (slot != null)
					InitSlot(indexer, slot, item);

				// Increment our indexer
				indexer++;
			}

			// Remove all slots after our index
			RemoveUnusedSlots(indexer);
		}

		/// <summary>
		/// Sets the current slot with the data.
		/// Override to inject custom functionality without having to rewrite the whole slot info
		/// </summary>
		/// <param name="index"></param>
		/// <param name="itemSlot"></param>
		/// <param name="item"></param>
		protected virtual void InitSlot(int index, Slot<T> itemSlot, T item)
		{
			// Set our item
			itemSlot.Set(item);
		}

		#endregion

		#region Get / Add / Remove Slot

		protected Slot<T> GetSlot(int index)
		{
			if (_slotList.Count > index)
				return _slotList[index];

			// Create a new slot
			return AddSlot();
		}

		public Slot<T> AddSlot()
		{
			if (_slotHolder == null) {
				Debug.LogError($"{GetType()}:GetSlot - Please assign a transform to hold all slots");
				return null;
			}

			GameObject slotGO = _slotHolder.gameObject.AddChild(_slotPrefab, false);

			if (slotGO == null) {
				Debug.LogError($"{GetType()}:GetSlot - Please assign a prefab for your slots");
				return null;
			}

			// Then Initialize our Slots
			slotGO.name = _slotPrefab.name + _slotHolder.childCount;

			// Get our Slot component to add to our list
			Slot<T> slot = slotGO.GetComponent<Slot<T>>();
			if (slot == null)
				Debug.LogError($"{GetType()}: GetSlot - Slot prefab doesn't contain Slot<{typeof(T)}>", this);

			_slotList.Add(slot);
			return slot;
		}

		protected void RemoveUnusedSlots(int count)
		{
			while (_slotList.Count > count) {
				Destroy(_slotList[_slotList.Count - 1].gameObject);
				_slotList.RemoveAt(_slotList.Count - 1);
			}
		}

		public void DestroyAllSlots()
		{
			for (int i = _slotList.Count - 1; i >= 0; i--)
				Destroy(_slotList[i].gameObject);
			_slotList.Clear();
		}

		#endregion

		#region Find Slot

		public Slot<T> FindSlot(T item)
		{
			// Go through all of our slots
			for (int i = 0; i < _slotList.Count; i++) {
				// Attempt to find our slot
				if (item.Equals(_slotList[i].item))     // https://stackoverflow.com/questions/8982645/how-to-solve-operator-cannot-be-applied-to-operands-of-type-t-and-t
					return _slotList[i];
			}

			return null;
		}

		public bool TryFindSlot(T item, out Slot<T> slot)
		{
			slot = FindSlot(item);

			return slot != null;
		}

		#endregion

	}

}
