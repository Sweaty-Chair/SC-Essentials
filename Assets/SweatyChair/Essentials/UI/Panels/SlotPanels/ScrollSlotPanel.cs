using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	/// <summary>
	/// A scrollable panel that holds a number of slots in T ojects.
	/// </summary>
	/// <typeparam name="T">Slot</typeparam>
	public abstract class ScrollSlotPanel<T> : Panel
	{

		[SerializeField] protected GameObject _slotPrefab;
		[SerializeField] protected ScrollRect _scrollRect;

		protected List<Slot<T>> _slotList = new List<Slot<T>>();

		/// <summary>
		/// Initializes a number of T objects, call this in Awake or wherever init taken.
		/// </summary>
		/// <param name="items">A array of T</param>
		protected virtual void InitSlots(params T[] items)
		{
			for (int i = 0, imax = items.Length; i < imax; i++) {
				var slot = GetSlot(i);
				if (slot != null)
					slot.Set(items[i]);
			}
			RemoveUnusedSlots(items.Length);
		}

		/// <summary>
		/// Initializes a number of T objects, call this in Awake or wherever init taken.
		/// </summary>
		/// <param name="itemList">A list of T</param>
		protected virtual void InitSlots(List<T> itemList)
		{
			InitSlots(itemList.ToArray());
		}

		/// <summary>
		/// Gets the slot by index, instantiate one if not yet exists
		/// </summary>
		/// <param name="index"></param>
		/// <returns>Slot of T</returns>
		protected Slot<T> GetSlot(int index)
		{
			if (_slotList.Count > index)
				return _slotList[index];
			Slot<T> slot = AddSlot();
			if (slot == null) {
				Debug.LogErrorFormat("{0}:GetSlot - Slot prefab doesn't contain Slot<{1}>", GetType(), typeof(T));
				_slotList.Add(slot);
			}
			return slot;
		}

		/// <summary>
		/// Adds a slot manually.
		/// </summary>
		/// <returns>Slot of T</returns>
		public Slot<T> AddSlot()
		{
			if (_scrollRect == null) {
				Debug.LogErrorFormat("{0}:GetSlot - Please set the ScrollRect", GetType());
				return null;
			}
			if (_scrollRect.content == null) {
				Debug.LogErrorFormat("{0}:GetSlot - Please set the Content on ScrollRect", GetType());
				return null;
			}
			GameObject slotGO = _scrollRect.content.gameObject.AddChild(_slotPrefab, false);
			if (slotGO != null) { // Just in case
				slotGO.name = _slotPrefab.name + _scrollRect.content.childCount;
				Slot<T> slot = slotGO.GetComponent<Slot<T>>();
				_slotList.Add(slot);
				return slot;
			}
			return null;
		}

		/// <summary>
		/// Removes all unused slots, given a used slot count.
		/// </summary>
		/// <param name="count">Number of used slots</param>
		protected void RemoveUnusedSlots(int count)
		{
			while (_slotList.Count > count) {
				Destroy(_slotList[_slotList.Count - 1].gameObject);
				_slotList.RemoveAt(_slotList.Count - 1);
			}
		}

		/// <summary>
		/// Destroys all slots.
		/// </summary>
		public void DestroyAllSlots()
		{
			for (int i = _slotList.Count - 1; i >= 0; i--)
				Destroy(_slotList[i].gameObject);
			_slotList.Clear();
		}

		/// <summary>
		/// Center on a given slot.
		/// </summary>
		/// <param name="slot">The slot to be centered</param>
		public void CenterSlot(Slot<T> slot)
		{
			StartCoroutine(CenterSlotCoroutine(slot));
		}

		private IEnumerator CenterSlotCoroutine(Slot<T> slot)
		{
			/// Note: Wait 2 frames to make sure the scroll view and children slots are positioned
			yield return null;
			yield return null;
			if (_scrollRect.horizontal)
				_scrollRect.ScrollToCenter(slot.GetComponent<RectTransform>(), RectTransform.Axis.Horizontal);
			else if (_scrollRect.vertical)
				_scrollRect.ScrollToCenter(slot.GetComponent<RectTransform>(), RectTransform.Axis.Vertical);
		}

	}

}