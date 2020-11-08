using UnityEngine;

namespace SweatyChair.UI
{

	/*****
	 * A abstract UI slot that display a Item class.
	 *****/
	public abstract class ItemSlot<T> : Slot<T> where T : Item
	{

		// Set using speicified Item class
		public virtual void Set<Item>(T item)
		{
			Set(item);
		}

		#region Debug

#if UNITY_EDITOR

		[ContextMenu("Debug/Log Current Item")]
		private void EDITOR_DebugCurrentItem()
		{
			Debug.Log($"{GetType()} - Item = {item.ToString()}");
		}

#endif

		#endregion

	}

}