using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.RectTransform;

namespace UnityEngine.UI
{

	[SelectionBase]
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class PullTabView : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, ICanvasElement, ILayoutElement, ILayoutGroup
	{

		#region Enum

		public enum PullDirection
		{
			LeftToRight,
			RightToLeft,
			BottomToTop,
			TopToBottom
		}

		#endregion

		#region Event

		[Serializable]
		public class PullTabEvent : UnityEvent<float> { }

		#endregion

		#region Variables

		[SerializeField] private PullTabEvent _onValueChanged = new PullTabEvent();
		public PullTabEvent onValueChanged { get { return _onValueChanged; } set { _onValueChanged = value; } }

		[SerializeField] private UnityEvent _onPullTabMinimized = new UnityEvent();
		public UnityEvent onPullTabMinimized { get { return _onPullTabMinimized; } set { _onPullTabMinimized = value; } }

		[SerializeField] private UnityEvent _onPullTabMaximized = new UnityEvent();
		public UnityEvent onPullTabMaximized { get { return _onPullTabMaximized; } set { _onPullTabMaximized = value; } }



		[SerializeField] private RectTransform _content;
		public RectTransform content { get { return _content; } set { _content = value; } }

		[SerializeField] private PullDirection _pullDirection;
		public PullDirection pullDirection { get { return _pullDirection; } set { _pullDirection = value; } }
		Axis axis { get { return (_pullDirection == PullDirection.LeftToRight || _pullDirection == PullDirection.RightToLeft) ? Axis.Horizontal : Axis.Vertical; } }

		[SerializeField] private RectTransform _pullTab;
		public RectTransform pullTab { get { return _pullTab; } set { _pullTab = value; } }

		[SerializeField] private RectTransform _selfRectTF;
		public RectTransform selfRectTF { get { return _selfRectTF; } set { _selfRectTF = value; } }

		private float _value;

		#endregion

		#region Private Vars

		private DrivenRectTransformTracker _tracker;
		private Vector2 _dragOffset;
		private bool _isDragging;

		#endregion

		#region OnEnable / OnDisable

		#endregion

		#region Set

		public void Set(float input, bool sendCallback = true) {
			float currentValue = _value;
			_value = Mathf.Clamp01(input);

			if (currentValue == _value) { return; }

			UpdateVisuals();

			if (sendCallback) {
				_onValueChanged?.Invoke(_value);

				if (_value == 0) { _onPullTabMinimized?.Invoke(); }
				if (_value == 1) { _onPullTabMaximized?.Invoke(); }
			}
		}

		public void UpdateVisuals() {

			_tracker.Clear();

			if (_selfRectTF != null) {
				_tracker.Add(this, _pullTab, DrivenTransformProperties.Anchors);
				Vector2 anchorMin = Vector2.zero;
				Vector2 anchorMax = Vector2.one;

				float childSize = axis == 0 ? _pullTab.rect.width : _pullTab.rect.height;
				float movement = _value * (1 - childSize);

				anchorMin[(int)axis] = movement;
				anchorMax[(int)axis] = movement + childSize;

				_pullTab.anchorMin = anchorMin;
				_pullTab.anchorMax = anchorMax;
			}
		}

		#endregion

		#region Drag Interface Implementations

		public void OnInitializePotentialDrag(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left) { return; }
		}

		public void OnBeginDrag(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left) { return; }

			if (!IsActive()) { return; }

			//Get if our mouse is within our pull tab rect
			if (RectTransformUtility.RectangleContainsScreenPoint(_pullTab, eventData.position, eventData.pressEventCamera)) {

				//Get our offset within our local point
				Vector2 localMousePos;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_pullTab, eventData.position, eventData.pressEventCamera, out localMousePos)) {
					_dragOffset = localMousePos - _pullTab.rect.center;
					_isDragging = true;
				}
			}
		}

		public void OnDrag(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left) { return; }

			if (!IsActive()) { return; }

			Vector2 localCursor;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_selfRectTF, eventData.position, eventData.pressEventCamera, out localCursor))
				return;

			Vector2 handleCenterRelativeToContainerCorner = localCursor - _dragOffset - _selfRectTF.rect.position;
			Vector2 handleCorner = handleCenterRelativeToContainerCorner - (_pullTab.rect.size - _pullTab.sizeDelta) * 0.5f;

			float parentSize = axis == 0 ? _selfRectTF.rect.width : _selfRectTF.rect.height;
			float childSize = axis == 0 ? _pullTab.rect.width : _pullTab.rect.height;
			float remainingSize = parentSize * (1 - childSize);
			if (remainingSize <= 0)
				return;

			switch (_pullDirection) {
			case PullDirection.LeftToRight:
				Set(handleCorner.x / remainingSize);
				break;
			case PullDirection.RightToLeft:
				Set(1f - (handleCorner.x / remainingSize));
				break;
			case PullDirection.BottomToTop:
				Set(handleCorner.y / remainingSize);
				break;
			case PullDirection.TopToBottom:
				Set(1f - (handleCorner.y / remainingSize));
				break;
			}
		}

		public void OnEndDrag(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left) { return; }

			_isDragging = false;
		}

		#endregion

		#region Layout Interface Implementation

		public float minWidth => throw new NotImplementedException();

		public float preferredWidth => throw new NotImplementedException();

		public float flexibleWidth => throw new NotImplementedException();

		public float minHeight => throw new NotImplementedException();

		public float preferredHeight => throw new NotImplementedException();

		public float flexibleHeight => throw new NotImplementedException();

		public int layoutPriority => throw new NotImplementedException();

		public void Rebuild(CanvasUpdate executing) {
			throw new NotImplementedException();
		}

		public void LayoutComplete() {
			throw new NotImplementedException();
		}

		public void GraphicUpdateComplete() {
			throw new NotImplementedException();
		}

		public void CalculateLayoutInputHorizontal() {
			throw new NotImplementedException();
		}

		public void CalculateLayoutInputVertical() {
			throw new NotImplementedException();
		}

		public void SetLayoutHorizontal() {
			throw new NotImplementedException();
		}

		public void SetLayoutVertical() {
			throw new NotImplementedException();
		}

		#endregion

	}

}
