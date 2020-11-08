using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SweatyChair.InputSystem
{

	public class InputDrag
	{

		#region Variables

		private MouseButton mouseButton { get; set; }

		private bool isDisabled { get; set; }                           //Whether our axis is disabled or not

		private bool startedDragOverUI { get; set; }
		private Vector2 initialClickPos { get; set; }
		private Vector2 lastFrameMousePos { get; set; }
		private Vector2 currentMousePos { get; set; }
		private Vector2 dragDelta { get; set; }

		private ClickDragState currentDragState { get; set; }

		private bool isDragging { get { return currentDragState != ClickDragState.None && currentDragState != ClickDragState.MouseDown && currentDragState != ClickDragState.WaitingForDrag; } }

		private DragPayloadEvent potentialDragEvent { get; set; }
		private DragPayloadEvent draggingEvent { get; set; }
		private DragPayloadEvent endDragEvent { get; set; }

		//Get a modified version of our button input which takes into account whether or not our object has been disabled
		private bool curFrameMouseDownInput { get { return (isDisabled) ? false : UnityEngine.Input.GetMouseButtonDown((int)mouseButton); } }
		private bool curFrameMouseUpInput { get { return (isDisabled) ? false : UnityEngine.Input.GetMouseButtonUp((int)mouseButton); } }

		#endregion

		#region Constructor

		public InputDrag(MouseButton mouseButton)
		{
			InitToDefault(mouseButton);
			InitializeState();
		}

		private void InitToDefault(MouseButton mouseButton)
		{
			this.mouseButton = mouseButton;

			initialClickPos = -Vector2.one;
			lastFrameMousePos = -Vector2.one;
			currentMousePos = -Vector2.one;
			dragDelta = Vector2.zero;

			currentDragState = ClickDragState.None;

			potentialDragEvent = new DragPayloadEvent();
			draggingEvent = new DragPayloadEvent();
			endDragEvent = new DragPayloadEvent();
		}

		private void InitializeState()
		{
			if (Input.GetMouseButtonDown((int)mouseButton) || Input.GetMouseButton((int)mouseButton)) {
				initialClickPos = Input.mousePosition;
				lastFrameMousePos = Input.mousePosition;
				currentMousePos = Input.mousePosition;
				dragDelta = Vector2.zero;

			} else {
				initialClickPos = -Vector2.one;
				lastFrameMousePos = Input.mousePosition;
				currentMousePos = -Vector2.one;
				dragDelta = Vector2.zero;
			}

			RefreshState();
		}

		#endregion

		#region Disable

		public void DisableDrag(bool isDisabled)
		{
			//If our disabled state is the same, return
			if (this.isDisabled == isDisabled) { return; }

			//Set our disabled state
			this.isDisabled = isDisabled;

			//Simulate a drag with parameters to end a drag if one is in progress
			RefreshState(Input.mousePosition, false, true);
		}

		#endregion

		#region Update

		public void RefreshState()
		{
			RefreshState(Input.mousePosition, curFrameMouseDownInput, curFrameMouseUpInput);
		}
		public void RefreshState(Vector2 mousePos, bool mouseDown, bool mouseUp)
		{
			lastFrameMousePos = currentMousePos;
			currentMousePos = mousePos;
			dragDelta = lastFrameMousePos - currentMousePos;

			//If we have moused down for one frame, set us to be waiting for a drag
			if (currentDragState == ClickDragState.MouseDown) {
				currentDragState = ClickDragState.WaitingForDrag;

				startedDragOverUI = EventSystem.current.IsPointerOverGameObject();
				potentialDragEvent?.Invoke(new InputDragPayload(initialClickPos, currentMousePos, Vector2.zero, startedDragOverUI));
			}

			//If our drag finished last frame, set our drag state to none
			if (currentDragState == ClickDragState.DragEnd) {
				currentDragState = ClickDragState.None;
				endDragEvent?.Invoke(new InputDragPayload(initialClickPos, currentMousePos, Vector2.zero, startedDragOverUI));
			}

			//Get mouse down while we have no drag state
			if (currentDragState == ClickDragState.None && mouseDown) {
				initialClickPos = mousePos;
				currentDragState = ClickDragState.MouseDown;
			}

			//If we are waiting for drag, check our drag threshold
			if (currentDragState == ClickDragState.WaitingForDrag) {
				if (Vector3.Distance(mousePos, initialClickPos) > 0.1) {
					currentDragState = ClickDragState.MouseDragging;
				}
			}

			//If the user lets go of the mouse, set our state
			if (mouseUp) {
				if (currentDragState == ClickDragState.MouseDragging) {
					currentDragState = ClickDragState.DragEnd;
				} else {
					currentDragState = ClickDragState.None;
				}
			}

			if (currentDragState == ClickDragState.MouseDragging) {                      //While our mouse is dragging, update our current mouse position
				draggingEvent?.Invoke(new InputDragPayload(initialClickPos, currentMousePos, dragDelta, startedDragOverUI));

			}

		}

		#endregion

		#region Get State

		public ClickDragState GetDragState()
		{
			return currentDragState;
		}

		public bool IsDragging()
		{
			return isDragging;
		}

		public InputDragPayload GetDragData()
		{
			if (isDragging) {
				return new InputDragPayload(initialClickPos, currentMousePos, dragDelta, startedDragOverUI);
			} else {
				return new InputDragPayload();
			}
		}

		#endregion

		#region Subscribe / Unsubscribe

		#region PotentialDrag

		/// <summary>
		/// Subscribes a delegate to recieve updates each time the state of the button changes
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddPotentialDragListener(UnityAction<InputDragPayload> action)
		{
			potentialDragEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the state of the button changes
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemovePotentialDragListener(UnityAction<InputDragPayload> action)
		{
			potentialDragEvent.RemoveListener(action);
		}

		#endregion

		#region Drag Update

		/// <summary>
		/// Subscribes a delegate to recieve updates each time the button goes down
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddDraggingListener(UnityAction<InputDragPayload> action)
		{
			draggingEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the button goes down
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveDraggingListener(UnityAction<InputDragPayload> action)
		{
			draggingEvent.RemoveListener(action);
		}

		#endregion

		#region Drag End

		/// <summary>
		/// Subscribes a delegate to recieve updates each time the button goes up
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddDragEndListener(UnityAction<InputDragPayload> action)
		{
			endDragEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the button goes up
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveDragEndListener(UnityAction<InputDragPayload> action)
		{
			endDragEvent.RemoveListener(action);
		}

		#endregion

		#endregion

	}

	/// <summary>
	/// Payload GameData for drag events
	/// </summary>
	public struct InputDragPayload
	{

		public Vector2 initialMousePos { get; private set; }
		public Vector2 currentMousePos { get; private set; }
		public Vector2 dragDelta { get; private set; }
		public bool startedDragOverUI { get; private set; }

		public InputDragPayload(Vector2 initialClickPos, Vector2 currentClickPos, Vector2 dragDelta, bool startedDragOverUI)
		{
			this.initialMousePos = initialClickPos;
			this.currentMousePos = currentClickPos;
			this.dragDelta = dragDelta;
			this.startedDragOverUI = startedDragOverUI;
		}

		public override string ToString()
		{
			return string.Format("Initial Mouse Pos : {0}, Current Mouse Pos : {1}, dragDelta : {2}, StartedDrag on UI : {3}", initialMousePos, currentMousePos, dragDelta, startedDragOverUI);
		}
	}

}
