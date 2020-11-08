using UnityEngine.Events;

namespace SweatyChair.InputSystem {

	/// <summary>
	/// A wrapper class around axis input that allows for event based input rather than polling for states. Used by InputManager
	/// </summary>
	public class InputAxis {

		#region Variables

		public string axisName { get; private set; }                    //Name of the axis defined in Unity Input

		private bool isDisabled { get; set; }                           //Whether our axis is disabled or not

		private float lastFrameValue { get; set; }                      //Cached value of our axis
		private float currentFrameValue { get; set; }            //Current value of our axis

		private float lastFrameRawValue { get; set; }                      //Cached value of our Raw axis
		private float currentFrameRawValue { get; set; }            //Current value of our Raw axis

		private AxisValueChangeEvent valueChangeEvent { get; set; }     //Event for axis value change
		private AxisValueChangeEvent rawValueChangeEvent { get; set; }     //Event for axis value change

		private AxisValueChangeEvent valueEvent { get; set; }
		private AxisValueChangeEvent rawValueEvent { get; set; }

		private float axisDelta { get { return currentFrameValue - lastFrameValue; } }
		private float axisRawDelta { get { return currentFrameRawValue - lastFrameRawValue; } }


		//Get a modified version of our button input which takes into account whether or not our object has been disabled
		private float curFrameAxisInput { get { return (isDisabled) ? 0 : UnityEngine.Input.GetAxis(axisName); } }
		private float curFrameAxisRawInput { get { return (isDisabled) ? 0 : UnityEngine.Input.GetAxisRaw(axisName); } }

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a class to store axis data to handle events related to Input.
		/// </summary>
		/// <param name="axisName">The name of the axis we would like to track</param>
		public InputAxis(string axisName) {
			InitToDefault(axisName);
			InitializeState();
		}

		private void InitToDefault(string axisName) {
			this.axisName = axisName;
			lastFrameValue = 0;
			currentFrameValue = 0;
			valueChangeEvent = new AxisValueChangeEvent();
			rawValueChangeEvent = new AxisValueChangeEvent();
			valueEvent = new AxisValueChangeEvent();
			rawValueEvent = new AxisValueChangeEvent();
		}

		private void InitializeState() {
			lastFrameValue = UnityEngine.Input.GetAxis(axisName);
			currentFrameValue = lastFrameValue;

			lastFrameRawValue = UnityEngine.Input.GetAxisRaw(axisName);
			currentFrameRawValue = currentFrameRawValue;

			RefreshState();
		}

		#endregion

		#region Disable

		public void DisableAxis(bool isDisabled) {
			//If our disabled state is the same, return
			if (this.isDisabled == isDisabled) { return; }

			//Set our disabled state
			this.isDisabled = isDisabled;
		}

		#endregion

		#region Update

		public void RefreshState() {
			//Get Axis
			lastFrameValue = currentFrameValue;   //Refresh our lastFrameValue
			currentFrameValue = curFrameAxisInput; //Get our current frame data

			if (currentFrameValue != lastFrameValue) {    //if our value has changed, call our event
				valueChangeEvent?.Invoke(currentFrameValue);
			}
			valueEvent?.Invoke(currentFrameValue);

			//Get Axis Raw
			lastFrameRawValue = currentFrameRawValue;   //Refresh our lastFrameRawValue
			currentFrameRawValue = curFrameAxisRawInput; //Get our current raw frame data

			if (currentFrameRawValue != lastFrameRawValue) {    //if our value has changed, call our event
				rawValueChangeEvent?.Invoke(currentFrameRawValue);
			}
			rawValueEvent?.Invoke(currentFrameRawValue);
		}

		#endregion

		#region Get Axis

		/// <summary>
		/// Returns the value for this axis for the current frame.
		/// </summary>
		/// <returns>Returns a float between -1 and 1</returns>
		public float GetAxis() {
			return currentFrameValue;    //return out current axis value
		}

		/// <summary>
		/// Returns the value for this axis for the current frame, with no smoothing applied.
		/// </summary>
		/// <returns></returns>
		public float GetAxisRaw() {
			return currentFrameRawValue;    //return out current axis raw value
		}

		/// <summary>
		/// Returns the axis delta from last frame compared to this frame
		/// </summary>
		/// <returns></returns>
		public float GetAxisDelta() {
			return axisDelta;
		}

		/// <summary>
		/// Returns the axis delta from last frame compared to this frame, with no smoothing applied.
		/// </summary>
		/// <returns></returns>
		public float GetAxisRawDelta() {
			return axisRawDelta;
		}

		#endregion

		#region Subscribe / Unsubscribe

		#region Get Axis Change

		/// <summary>
		/// Subscribes the delegate to recieve updates each time the axis changes.
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddAxisChangeListener(UnityAction<float> action) {
			valueChangeEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the axis changes.
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveAxisChangeListener(UnityAction<float> action) {
			valueChangeEvent.RemoveListener(action);
		}

		#endregion

		#region Get Axis

		/// <summary>
		/// Subscribes the delegate to recieve updates each time the axis changes.
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddAxisListener(UnityAction<float> action) {
			valueEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the axis changes.
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveAxisListener(UnityAction<float> action) {
			valueEvent.RemoveListener(action);
		}

		#endregion

		#region Get Axis Change raw

		/// <summary>
		/// Subscribes the delegate to recieve updates each time the raw axis changes.
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddRawAxisChangeListener(UnityAction<float> action) {
			rawValueChangeEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the raw axis changes.
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveRawAxisChangeListener(UnityAction<float> action) {
			rawValueChangeEvent.RemoveListener(action);
		}

		#endregion

		#region Get Axis Change raw

		/// <summary>
		/// Subscribes the delegate to recieve updates each time the raw axis changes.
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddRawAxisListener(UnityAction<float> action) {
			rawValueEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the raw axis changes.
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveRawAxisListener(UnityAction<float> action) {
			rawValueEvent.RemoveListener(action);
		}

		#endregion

		#endregion
	}

}