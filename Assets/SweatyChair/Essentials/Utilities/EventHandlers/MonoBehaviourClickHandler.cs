using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SweatyChair.Events
{

	[Flags]
	public enum MonoClickType
	{
		onPointerClick = 1 << 0,
		onPointerDown = 1 << 1,
		onPointerUp = 1 << 2,
		onPointerEnter = 1 << 3,
		onPointerExit = 1 << 4,
		onDrag = 1 << 5
	}
	public class MonoBehaviourClickHandler : MonoBehaviour,
											IDragHandler,
											IPointerClickHandler,
											IPointerDownHandler,
											IPointerEnterHandler,
											IPointerExitHandler,
											IPointerUpHandler
	{

		[EnumFlag("Events")]
		public MonoClickType selectedClickEvents;

		public UnityEvent onDrag;
		public UnityEvent onPointerClick;
		public UnityEvent onPointerUp;
		public UnityEvent onPointerDown;
		public UnityEvent onPointerEnter;
		public UnityEvent onPointerExit;

		#region Pointer Events

		public void OnDrag(PointerEventData pointerData)
		{
			if (onDrag != null && selectedClickEvents == (selectedClickEvents | (MonoClickType.onDrag)))
				onDrag.Invoke();
		}

		public void OnPointerClick(PointerEventData pointerData)
		{
			if (onPointerClick != null && selectedClickEvents == (selectedClickEvents | (MonoClickType.onPointerClick)))
				onPointerClick.Invoke();
		}

		public void OnPointerUp(PointerEventData pointerData)
		{
			if (onPointerUp != null && selectedClickEvents == (selectedClickEvents | (MonoClickType.onPointerUp)))
				onPointerUp.Invoke();
		}

		public void OnPointerDown(PointerEventData pointerData)
		{
			if (onPointerDown != null && selectedClickEvents == (selectedClickEvents | (MonoClickType.onPointerDown)))
				onPointerDown.Invoke();
		}

		public void OnPointerEnter(PointerEventData pointerData)
		{
			if (onPointerEnter != null && selectedClickEvents == (selectedClickEvents | (MonoClickType.onPointerEnter)))
				onPointerEnter.Invoke();
		}

		public void OnPointerExit(PointerEventData pointerData)
		{
			if (onPointerExit != null && selectedClickEvents == (selectedClickEvents | (MonoClickType.onPointerExit)))
				onPointerExit.Invoke();
		}

		#endregion
	}

}