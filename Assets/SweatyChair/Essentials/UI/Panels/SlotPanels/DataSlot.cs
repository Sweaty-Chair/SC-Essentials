using UnityEngine;

namespace SweatyChair.UI
{

	/// <summary>
	/// A generic UI slot that display a BaseData class.
	/// </summary>
	/// <typeparam name="T">BaseData</typeparam>
	public abstract class DataSlot<T> : Slot<T> where T : BaseData
	{

		// Simply mapping item in base Slot class to data
		public T data {
			get { return item; }
			set { item = value; }
		}

#if UNITY_EDITOR

		[ContextMenu("Print Data")]
		public void PrintData()
		{
			Debug.Log(item);
		}

#endif

	}

}