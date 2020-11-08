using UnityEngine.Events;

namespace SweatyChair.InputSystem
{

	/// <summary>
	/// A wrapper class around button input that allows for event based input rather than polling for states. Used by InputManager.
	/// </summary>
	public class InputButton
	{

		#region Variables

		public string buttonName { get; private set; }                          //Name of the button defined in Unity Input

		public bool isDisabled { get; private set; }                            //Whether our button is disabled or not

		public bool lastFrameDown { get; private set; }                         //Cached state of button
		public bool currentFrameDown { get; private set; }                      //Current state of our button

		public float holdTime { get; private set; }                             //How long we have currently been holding our button for
		public float unscaledHoldTime { get; private set; }                     //How long we have currently been holding our button for, ignoring timescale

		public ButtonState currentButtonState { get; private set; }             //The current state our button is in

		private ButtonStateChangeEvent stateChangeEvent { get; set; }   //Event for button state change
		private ButtonValueChangeEvent valueChangeEvent { get; set; }   //Event for button bool value change
		private UnityEvent buttonDownEvent { get; set; }                //Event for button down
		private UnityEvent buttonUpEvent { get; set; }                  //Event for button up


		//Get a modified version of our button input which takes into account whether or not our object has been disabled
		private bool curFrameButtonInput { get { return (isDisabled) ? false : UnityEngine.Input.GetButton(buttonName); } }

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a class to store button data to handle events related to Input.
		/// </summary>
		/// <param name="axisName">The name of the button we would like to track</param>
		public InputButton(string buttonName) {
			InitToDefault(buttonName);
			InitializeState();
		}

		private void InitToDefault(string buttonName) {
			this.buttonName = buttonName;
			lastFrameDown = false;
			currentFrameDown = false;
			currentButtonState = ButtonState.Released;
			stateChangeEvent = new ButtonStateChangeEvent();
			valueChangeEvent = new ButtonValueChangeEvent();
			buttonDownEvent = new UnityEvent();
			buttonUpEvent = new UnityEvent();

			//Disabled
			isDisabled = false;
		}

		private void InitializeState() {
			if (UnityEngine.Input.GetButtonDown(buttonName)) {  //If our button was just pressed, set our data to be the onpress
				lastFrameDown = false;
				currentFrameDown = false;

			} else if (UnityEngine.Input.GetButtonUp(buttonName)) { //If our button was just released, initialize into the release state
				lastFrameDown = true;
				currentFrameDown = true;

			} else {
				lastFrameDown = UnityEngine.Input.GetButton(buttonName);    //if our button is currently held or released, initialize to correct stateS
				currentFrameDown = lastFrameDown;
			}

			RefreshState();
		}

		#endregion

		#region Disable

		public void DisableButton(bool isDisabled) {
			//If our disabled state is the same, return
			if (this.isDisabled == isDisabled) { return; }

			//Set our disabled state
			this.isDisabled = isDisabled;
		}

		#endregion

		#region Update

		public void RefreshState() {
			bool stateChanged = false; // Hold a reference to if our state changed this frame

			lastFrameDown = currentFrameDown;   //Refresh our lastFrameDownState
			currentFrameDown = curFrameButtonInput; //Get our current frame data

			if (currentFrameDown != lastFrameDown) {    //if our state has changed, update our button state
				currentButtonState = (currentFrameDown) ? ButtonState.OnHold : ButtonState.OnRelease;  //If our button state is down, it means we just pressed the button down, else we just released it
				stateChanged = true;

			} else {
				if (currentButtonState == ButtonState.OnHold) { currentButtonState = ButtonState.Holding; }         //If our button state is the same, we are now holding.
				else if (currentButtonState == ButtonState.OnRelease) { currentButtonState = ButtonState.Released; }    //If our button state is the same, we have released the button

			}
			//Update our holdtime before any of our events are called
			UpdateHoldTime();

			if (stateChanged) { stateChangeEvent?.Invoke(currentButtonState); }               //Invoke our event with our current button state
			if (stateChanged) { valueChangeEvent?.Invoke(currentButtonState == ButtonState.OnHold || currentButtonState == ButtonState.Holding); }
			if (currentButtonState == ButtonState.OnHold) { buttonDownEvent?.Invoke(); }    //Invoke the button down event
			if (currentButtonState == ButtonState.OnRelease) { buttonUpEvent?.Invoke(); }    //Invoke the button up event
		}

		private void UpdateHoldTime() {
			//Reset our Hold State if we begin our hold
			if (currentButtonState == ButtonState.OnHold) {
				holdTime = 0;
				unscaledHoldTime = 0;
			}

			//If we are holding, or if we have released this frame update our 
			if (currentButtonState == ButtonState.Holding || currentButtonState == ButtonState.OnRelease) {
				holdTime += TimeScaleManager.modifiedDeltaTime;
				unscaledHoldTime += TimeScaleManager.modifiedUnscaledDeltaTime;
			}
		}

		#endregion

		#region GetState

		/// <summary>
		/// Returns the current state of our button.
		/// </summary>
		/// <returns>An enum describing the button state</returns>
		public ButtonState GetButtonState() {
			return currentButtonState;
		}

		/// <summary>
		/// Returns the On/Off state of the current button.
		/// </summary>
		/// <returns>Bool where 'True' means the button is held down</returns>
		public bool GetButton() {
			return (currentButtonState == ButtonState.OnHold || currentButtonState == ButtonState.Holding);    //return true if our button has just been pressed or is currently pressing down
		}

		/// <summary>
		/// Returns whether the button was pressed down this frame
		/// </summary>
		/// <returns>Bool for whether or not the button was pressed down that frame</returns>
		public bool GetButtonDown() {
			return (currentButtonState == ButtonState.OnHold); //Return true if user just pressed button
		}

		/// <summary>
		/// Returns whether the button was released this frame
		/// </summary>
		/// <returns>Bool for whether or not the button was released that frame</returns>
		public bool GetButtonUp() {
			return (currentButtonState == ButtonState.OnRelease); //Return true if user just released button
		}

		/// <summary>
		/// Returns the current hold time for this current button.
		/// </summary>
		/// <returns></returns>
		public float GetHoldTime() {
			return holdTime;
		}

		/// <summary>
		/// Returns the current unscaled hold time for this current button
		/// </summary>
		/// <returns></returns>
		public float GetUnscaledHoldTime() {
			return unscaledHoldTime;
		}

		#endregion

		#region Subscribe / Unsubscribe

		#region State Change

		/// <summary>
		/// Subscribes a delegate to recieve updates each time the state of the button changes
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddStateChangeListener(UnityAction<ButtonState> action) {
			stateChangeEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the state of the button changes
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveStateChangeListener(UnityAction<ButtonState> action) {
			stateChangeEvent.RemoveListener(action);
		}

		#endregion

		#region Value Change

		/// <summary>
		/// Subscribes a delegate to recieve updates each time the state of the button changes
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddValueChangeListener(UnityAction<bool> action) {
			valueChangeEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the state of the button changes
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveValueChangeListener(UnityAction<bool> action) {
			valueChangeEvent.RemoveListener(action);
		}

		#endregion

		#region Button Down

		/// <summary>
		/// Subscribes a delegate to recieve updates each time the button goes down
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddButtonDownListener(UnityAction action) {
			buttonDownEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the button goes down
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveButtonDownListener(UnityAction action) {
			buttonDownEvent.RemoveListener(action);
		}

		#endregion

		#region Button Up

		/// <summary>
		/// Subscribes a delegate to recieve updates each time the button goes up
		/// </summary>
		/// <param name="action">The delegate to recieve the event</param>
		public void AddButtonUpListener(UnityAction action) {
			buttonUpEvent.AddListener(action);
		}

		/// <summary>
		/// Unsubscribes a delegate from recieving updates each time the button goes up
		/// </summary>
		/// <param name="action">The delegate to unsubscribe from the event</param>
		public void RemoveButtonUpListener(UnityAction action) {
			buttonUpEvent.RemoveListener(action);
		}

		#endregion

		#endregion

	}
}
