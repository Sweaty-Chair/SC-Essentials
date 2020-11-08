using UnityEngine.EventSystems;

namespace SweatyChair.UI
{

	/// <summary>
	/// A generic UI slot that display a generic T class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Slot<T> : UIBehaviour
	{

		public T item { get; protected set; }

		public virtual void Set(T t)
		{
			item = t;
		}

		public virtual void Toggle(bool show)
		{
			gameObject.SetActive(show);
		}

		public virtual void Refresh() { }

		public virtual void CopyFrom(object obj) { }

	}

}