using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.Serialization;

namespace SweatyChair.UI
{

	/*****
	 * Panel that holds a number of collections of ItemSlots, optionally with scroll views.
	 * Author: RV 
	 *****/
	public class MultipleItemSlotPanel<T> : Panel where T : Item
	{

		[Tooltip("Slot prefab")]
		[SerializeField] protected GameObject _slotPrefab;
		[Tooltip("Slot parend transform, in most cases to ScroollView/Viewpoint/Content")]
		[SerializeField] protected Transform[] _slotParentTFs;
		[Tooltip("Optional, occlusiong scroll rects, for performance being lot of slots")]
		[SerializeField] protected ScrollRectOcclusion[] _scrollRectOcclusions;
		[Tooltip("Instantiates the slots by batch, 0 means no batching")]
		[SerializeField] protected int _batchInstantiateCount = 0;

		protected Dictionary<int, List<ItemSlot<T>>> _slotDict = new Dictionary<int, List<ItemSlot<T>>>();

		// Given a list of item, replace the slot with existing same item or create new one.
		public virtual void Set(int rowIndex, List<T> itemList)
		{
			if (_slotParentTFs.Length <= rowIndex) {
				Debug.LogErrorFormat("MultipleItemSlotPanel:Set - Invalid rowIndex={0}", rowIndex);
				return;
			}
			TimeManager.Start(SetCoroutine(rowIndex, itemList));
		}

		protected virtual IEnumerator SetCoroutine(int rowIndex, List<T> itemList)
		{
			PreSet(rowIndex);

			if (!_slotDict.ContainsKey(rowIndex))
				_slotDict.Add(rowIndex, new List<ItemSlot<T>>());
			         
			for (int i = 0; i < itemList.Count; i++) {
				T item = itemList[i];
				if (item == null) {
					Debug.LogErrorFormat("MultipleItemSlotPanel:SetCoroutine - item=null at i={1}", i);
					continue;
				}

				// Try to get an existing slot
				ItemSlot<T> slot = GetSlot(rowIndex, i);
				if (slot == null)
					slot = CreateSlot(rowIndex, item);
				else
					slot.Set((T)item);

				if (slot != null)
					_slotDict[rowIndex].Add(slot);

				if (_batchInstantiateCount > 0 && i % _batchInstantiateCount == 0)
					yield return null;
			}

			//RemoveUnusedSlots(rowIndex, itemList.Count); // Remove unused slots, just in case

			yield return null;

			PostSet(rowIndex);
		}

		protected virtual void PreSet(int rowIndex)
		{
			if (_scrollRectOcclusions.Length > rowIndex && _scrollRectOcclusions[rowIndex] != null)
				_scrollRectOcclusions[rowIndex].Reset();
		}

		protected virtual void PostSet(int rowIndex)
		{
			if (_scrollRectOcclusions.Length > rowIndex && _scrollRectOcclusions[rowIndex] != null)
				_scrollRectOcclusions[rowIndex].Init();
		}

		protected virtual ItemSlot<T> CreateSlot(int rowIndex, T item)
		{
			GameObject slotGO = Instantiate<GameObject>(_slotPrefab, _slotParentTFs[rowIndex]);
			ItemSlot<T> slot = null;
			if (slotGO != null) {
				slot = slotGO.GetComponent<ItemSlot<T>>();
				if (slot != null)
					slot.Set((T)item);
			}
			return slot;
		}

		protected virtual void RemoveSlot(int rowIndex, int slotIndex)
		{
			if (_slotDict.ContainsKey(rowIndex)) {
				if (_slotDict[rowIndex].Count > slotIndex && _slotDict[rowIndex][slotIndex] != null) {
					Destroy(_slotDict[rowIndex][slotIndex].gameObject);
					_slotDict[rowIndex].RemoveAt(slotIndex);
				}
			}
		}

		public ItemSlot<T> GetSlot(T item)
		{
			if (item != null) {
				foreach (var kvp in _slotDict) {
					foreach (ItemSlot<T> slot in kvp.Value) {
						if (item == slot.item)
							return slot;
					}
				}
			}
			return null;
		}

		public virtual ItemSlot<T> GetSlot(int rowIndex, int slotIndex)
		{
			if (_slotDict.ContainsKey(rowIndex)) {
				if (_slotDict[rowIndex].Count > slotIndex)
					return _slotDict[rowIndex][slotIndex];
			}
			return null;
		}

		public virtual Dictionary<int, List<ItemSlot<T>>> GetAllSlots()
		{
			return _slotDict;
		}

		protected virtual void DestroyAllSlots(int rowIndex)
		{
			if (_slotDict.ContainsKey(rowIndex)) {
				foreach (ItemSlot<T> slot in _slotDict[rowIndex]) {
					if (slot != null) // Just in case
						Destroy(slot.gameObject);
				}
				_slotDict[rowIndex].Clear();
			}
		}

		protected virtual void DestroyAllSlots()
		{
			foreach (var kvp in _slotDict) {
				foreach (ItemSlot<T> slot in kvp.Value) {
					if (slot != null) // Just in case
						Destroy(slot.gameObject);
				}
			}
			_slotDict.Clear();
		}

		// Remove all unused slots, given a used slot count
		protected void DestroyUnusedSlots(int rowIndex, int count)
		{
			if (_slotDict.ContainsKey(rowIndex)) {
				while (_slotDict[rowIndex].Count > count) {
					Destroy(_slotDict[rowIndex][_slotDict[rowIndex].Count - 1].gameObject);
					_slotDict[rowIndex].RemoveAt(_slotDict[rowIndex].Count - 1);
				}
			}
		}

	}

}