using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.Serialization;

namespace SweatyChair.UI
{

	/*****
	 * Panel that holds a number of ItemSlots, optionally with scroll view.
	 * Author: RV 
	 *****/
	public class ItemSlotPanel<T> : Panel where T : Item
	{

		[Tooltip("Slot prefab")]
		[SerializeField] protected GameObject _slotPrefab;
		[Tooltip("Slot parend transform, in most cases to ScroollView/Viewpoint/Content")]
		[FormerlySerializedAs("_panelHolderTF")] // TODO: remove
		[SerializeField] protected Transform _slotParentTF;
		[Tooltip("Optional, occlusiong scroll rect, for performance having lot of slots")]
		[SerializeField] protected ScrollRectOcclusion _scrollRectOcclusion;
		[Tooltip("Instantiates the slots by batch, 0 means no batching")]
		[SerializeField] protected int _batchInstantiateCount = 0;

		protected RectTransform _contentRectTF;
		protected Vector2 _defaultScrollPosition = Vector2.zero; // Used to reset the scroll rect to top
		protected List<ItemSlot<T>> _slotList = new List<ItemSlot<T>>();

		public override void Init()
		{
			base.Init();
			_contentRectTF = _slotParentTF.GetComponent<RectTransform>();
			_defaultScrollPosition = _contentRectTF.anchoredPosition;
		}

		// Given a list of item, replace the slot with existing same item or create new one.
		public virtual void Set(List<T> itemList)
		{
			TimeManager.Start(SetCoroutine(itemList));
		}

		protected virtual IEnumerator SetCoroutine(List<T> itemList)
		{
			PreSet();

			_contentRectTF.anchoredPosition = _defaultScrollPosition;

			int usedSlotCount = 0;
			// Loop through all of our items, and collate all of our used slots
			for (int i = 0; i < itemList.Count; i++) {

				// Get and validate our item
				T item = itemList[i];
				if (item == null)
					continue;

				// Try to get an existing slot, if not, generate a new slot
				ItemSlot<T> slot = GetOrCreateSlot(i);

				// Then set our slot
				if (slot != null)
					slot.Set((T)item);

				usedSlotCount++;

				if (_batchInstantiateCount > 0 && i % _batchInstantiateCount == 0)
					yield return null;
			}

			// Go through and destroy all of our un-used slots
			for (int i = _slotList.Count - 1; i >= usedSlotCount; i--)
			{
				Destroy(_slotList[i].gameObject);
				_slotList.RemoveAt(i);
			}

			yield return null;

			PostSet();
		}

		// Given an array of item, replace all slots one-by-one with index.
		public virtual void Set(T[] itemArray)
		{
			TimeManager.Start(SetCoroutine(itemArray));
		}

		// Optimize later
		protected virtual IEnumerator SetCoroutine(T[] itemArray)
		{
			PreSet();

			_contentRectTF.anchoredPosition = _defaultScrollPosition;

			int i = 0;

			for (; i < itemArray.Length; i++) {
				T item = itemArray[i];
				if (item == null)
					continue;

				// Try to get an existing slot
				if (i < _slotList.Count && _slotList[i] != null) {
					_slotList[i].Set((T)item);
				} else {
					while (i >= _slotList.Count) {
						_slotList.Add(CreateSlot());
						_slotList[i].Set((T)item);
					}
				}

				if (_batchInstantiateCount > 0 && i % _batchInstantiateCount == 0)
					yield return null;
			}

			for (int j = _slotList.Count - 1; j >= i; j++)
				RemoveSlot(j);

			yield return null;

			PostSet();
		}

		protected virtual void PreSet()
		{
			if (_scrollRectOcclusion != null)
				_scrollRectOcclusion.Reset();
		}

		protected virtual void PostSet()
		{
			if (_scrollRectOcclusion != null)
				_scrollRectOcclusion.Init();
		}

		protected virtual ItemSlot<T> GetOrCreateSlot(int index)
		{
			if (index >= _slotList.Count)
				_slotList.Add(CreateSlot());

			return GetSlot(index);
		}

		protected virtual ItemSlot<T> CreateSlot()
		{
			GameObject slotGO = Instantiate<GameObject>(_slotPrefab, _slotParentTF);

			if (slotGO != null)
				return slotGO.GetComponent<ItemSlot<T>>();

			return null;
		}

		protected virtual void RemoveSlot(int index)
		{
			if (index >= _slotList.Count || _slotList[index] == null)
				return;
			Destroy(_slotList[index].gameObject);
			_slotList.RemoveAt(index);
		}

		public ItemSlot<T> GetSlot(T item)
		{
			if (item != null) {
				foreach (ItemSlot<T> slot in _slotList) {
					if (item == slot.item)
						return slot;
				}
			}
			return null;
		}

		public virtual ItemSlot<T> GetSlot(int index)
		{
			if (_slotList.Count > index)
				return _slotList[index];
			return null;
		}

		public virtual List<ItemSlot<T>> GetAllSlots()
		{
			return _slotList;
		}

		protected virtual void DestroyAllSlots()
		{
			foreach (ItemSlot<T> slot in _slotList) {
				if (slot != null) // Just in case
					Destroy(slot.gameObject);
			}
			_slotList.Clear();
		}

		// Remove all unused slots, given a used slot count
		protected void RemoveUnusedSlots(int count)
		{
			while (_slotList.Count > count) {
				Destroy(_slotList[_slotList.Count - 1].gameObject);
				_slotList.RemoveAt(_slotList.Count - 1);
			}
		}

	}

}