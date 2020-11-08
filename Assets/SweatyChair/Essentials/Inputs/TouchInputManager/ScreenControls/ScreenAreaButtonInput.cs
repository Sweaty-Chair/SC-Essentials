#if CROSS_PLATFORM_INPUT

using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace SweatyChair.InputSystem
{

	public class ScreenAreaButtonInput : ScreenAreaControl
	{

#region Class

		public class FingerHoldData
		{
			public float fingerHoldTime;    //How long this finger has been held on the screen
		}

#endregion

#region Variables

		[Header("Button Event")]
		public StateChangedEvent buttonDownEvent;
		public StateChangedEvent buttonUpEvent;
		public ButtonHeldEvent buttonHeldEvent;

		[Header("Button Variables")]
		[Tooltip("How long the user has to hold their finger in the same location to register a touch")]
		public float holdTimeThreshold = 1f;
		[Tooltip("The maximum distance the user can move in a single frame to cancel the interaction")]
		public float maxMoveThreshold = 50f;

		//Internal
		protected Dictionary<int, FingerHoldData> validHoldData = new Dictionary<int, FingerHoldData>();
		protected bool lastFrameButtonDown;

#endregion

#region OnEnable / OnDisable

		protected override void OnEnable() {
			base.OnEnable();
			validHoldData = new Dictionary<int, FingerHoldData>();
		}

		protected override void OnDisable() {
			base.OnDisable();

			ReleaseAllButtonsDown();
		}

#endregion

#region Update

		private void Update() {
			//First Register all our valid touches for this frame
			RegisterAllValidTouches();

			//Then check for any holding data
			UpdateHoldState();

			//Then depending on our hold data, execute our input
			TryExecuteHoldEvent();
		}

#endregion

#region Hold

		private void UpdateHoldState() {
			//Iterate through all of last frames holds, and increment the hold time by deltaTime
			List<int> holdFingerIDs = new List<int>(validHoldData.Keys);
			for (int i = 0; i < holdFingerIDs.Count; i++) {
				//Get our finger id
				int fingerID = holdFingerIDs[i];
				//Increment our hold time for all valid fingers
				validHoldData[fingerID].fingerHoldTime += Time.deltaTime;
			}


			//Go through each valid touch, and check if it qualifies for a hold
			for (int i = 0; i < validTouchIDs.Count; i++) {

				Touch currentTouch;
				if (!TouchInputManager.GetCachedTouch(validTouchIDs[i], out currentTouch)) { continue; }

				//Check if our movement is above the threshold, and our finger hold started during the first touch down
				if (currentTouch.deltaPosition.magnitude < maxMoveThreshold && (currentTouch.phase == TouchPhase.Began || holdFingerIDs.Contains(currentTouch.fingerId))) {
					//Register our hold 
					RegisterValidHold(currentTouch.fingerId);
				} else {
					//Unregister our hold if it moved too much
					UnRegisterValidHold(currentTouch.fingerId);
				}
			}
		}

		protected virtual void ReleaseAllButtonsDown() {
			//If we have any fingers that are performing a hold
			if (validHoldData.Count == 0) { return; }

			//Clear our list, and Set our button back up
			validHoldData.Clear();
			ExecuteButtonState(false);
		}

#endregion

#region Register Valid Hold

		protected virtual void RegisterValidHold(int fingerID) {
			//If we dont already contain that key, then return
			if (!validHoldData.ContainsKey(fingerID)) {

				//Create new hold data for our new Finger
				FingerHoldData holdData = new FingerHoldData() {
					fingerHoldTime = 0,
				};

				//Add our hold data to our dictionary
				validHoldData.Add(fingerID, holdData);
			}
		}

		protected virtual void UnRegisterValidHold(int fingerID) {
			//Only unregister if we contain the key
			if (validHoldData.ContainsKey(fingerID)) {
				//Remove our finger from the hold dictionary
				validHoldData.Remove(fingerID);
			}
		}

#endregion

#region UnRegister Valid Touch Override

		protected override void UnRegisterValidTouch(int fingerID) {
			//Unregister our hold if our finger is taken off the screen or goes out of affect area
			UnRegisterValidHold(fingerID);

			base.UnRegisterValidTouch(fingerID);
		}

#endregion

#region Execute Hold Event

		private void TryExecuteHoldEvent() {
			//Variable which will store whether we will release or press our virtual axis
			bool executeHoldEvent = false;

			//If we have any fingers currently in the hold state,
			if (validHoldData.Count >= 0) {

				//Go through each finger and check if any are over our hold time threshold
				foreach (FingerHoldData finger in validHoldData.Values) {
					if (finger.fingerHoldTime >= holdTimeThreshold) {
						//Set our bool to execute our event, and break our of our loop
						executeHoldEvent = true;
						break;
					}
				}
			}

			//If we execute our hold event. Set our button down.
			if (executeHoldEvent) {
				ExecuteButtonState(true);
			} else {
				ExecuteButtonState(false);
			}
		}

#endregion

#region Button State Change

		protected virtual void ExecuteButtonState(bool buttonDown) {

			//if our state changed since last frame, call our event
			if (lastFrameButtonDown != buttonDown) {
				//If our state is button down, call our button down event,
				if (buttonDown) {
					if (buttonDownEvent != null) {
						buttonDownEvent.Invoke();
					}

					//Otherwise, call our button up event
				} else {
					if (buttonUpEvent != null) {
						buttonUpEvent.Invoke();
					}
				}
			}

			//Set our last frame button state
			lastFrameButtonDown = buttonDown;

			//Call our button Held event
			if (buttonHeldEvent != null) {
				buttonHeldEvent.Invoke(buttonDown);
			}
		}

#endregion

	}

}


#endif