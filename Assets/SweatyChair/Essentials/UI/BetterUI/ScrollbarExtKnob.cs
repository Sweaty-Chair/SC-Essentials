using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ScrollbarExtKnob : Selectable, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler, ICanvasElement {

	#region Variables

	public ScrollbarExt parentScrollbar;

	#endregion

	#region Enable / Disable

	protected override void OnEnable() {
		base.OnEnable();
	}

	#endregion

	#region DragHandlers

	public virtual void OnBeginDrag(PointerEventData eventData) {
		parentScrollbar.OnBeginDrag(eventData);
	}

	public virtual void OnDrag(PointerEventData eventData) {
		parentScrollbar.OnDrag(eventData);
	}

	public virtual void OnInitializePotentialDrag(PointerEventData eventData) {
		eventData.useDragThreshold = false;
	}

	#endregion

	#region Interface Implementations

	public virtual void Rebuild(CanvasUpdate executing) { }

	public virtual void LayoutComplete() { }

	public virtual void GraphicUpdateComplete() { }

	#endregion

}
