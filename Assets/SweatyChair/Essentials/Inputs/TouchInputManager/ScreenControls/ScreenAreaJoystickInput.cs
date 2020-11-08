#if CROSS_PLATFORM_INPUT

using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace SweatyChair.InputSystem
{

	public class ScreenAreaJoystickInput : ScreenAreaControl
	{

#region Class

		public class FingerJoystickData
		{
			public Vector2 initialFingerPosition;   //The position the finger was at on initial interaction
			public Vector2 currentFingerPosition;   //The position our finger was at on this frame
		}

#endregion

#region Variables

		[Header("Joystick Event")]
		public AxisValueEvent horizontalAxisEvent;
		public AxisValueEvent verticalAxisEvent;

		[Header("Joystick Variables")]
		public Joystick.AxisOption axesToUse = Joystick.AxisOption.Both;

		[Space()]
		[Tooltip("The minimum distance the user has to move away from the initial position to register as movement on the axis")]
		public float deadzone = 5f;
		[Tooltip("The maximum distance the user has to move away from the initial position to register as a full tilt")]
		public float movementRange = 100f;

		//Internal
		protected Dictionary<int, FingerJoystickData> validJoystickData = new Dictionary<int, FingerJoystickData>();

#endregion

#region OnEnable / OnDisable

		protected override void OnEnable()
		{
			base.OnEnable();
			validJoystickData = new Dictionary<int, FingerJoystickData>();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

#endregion

#region Update

		private void Update()
		{
			//First Register all our valid touches for this frame
			RegisterAllValidTouches();

			//Update our joystick state
			UpdateJoystickState();

			//Then depending on our data, execute our input
			TryExecuteJoystickEvent();
		}

#endregion

#region Joystick

		private void UpdateJoystickState()
		{
			//Go through each valid touch, and check if it qualifies for a hold
			for (int i = 0; i < validTouchIDs.Count; i++) {

				Touch currentTouch;
				if (!TouchInputManager.GetCachedTouch(validTouchIDs[i], out currentTouch)) { continue; }

				//Set our current joystick position to our finger position
				validJoystickData[currentTouch.fingerId].currentFingerPosition = currentTouch.position;
			}
		}

		private void TryExecuteJoystickEvent()
		{
			//Go through each of our valid touches and check if it 
			float horizontalAxisDelta = 0;
			float verticalAxisDelta = 0;

			//If we have any fingers currently in the hold state
			if (validJoystickData.Count >= 0) {

				//Go through each finger and check if any are over our deadzone
				foreach (FingerJoystickData finger in validJoystickData.Values) {
					//Get our finger Delta
					Vector2 movementDelta = finger.currentFingerPosition - finger.initialFingerPosition;
					//If the magnitude is larger than the deadzone
					if (Mathf.Abs(movementDelta.magnitude) > deadzone) {


						//Calculate our Horizontal axis Delta
						float xAxisDelta = Mathf.InverseLerp(deadzone, movementRange, Mathf.Abs(movementDelta.x));
						xAxisDelta *= Mathf.Sign(movementDelta.x);
						//Add our xAxis delta to our horizontal axis
						horizontalAxisDelta += xAxisDelta;


						//Calculate our Vertical axis Delta
						float yAxisDelta = Mathf.InverseLerp(deadzone, movementRange, Mathf.Abs(movementDelta.y));
						yAxisDelta *= Mathf.Sign(movementDelta.y);

						verticalAxisDelta += yAxisDelta;
					}
				}
			}

			//If we are recieving input on the horizontal axis, set our axis to our horizontal delta
			if (axesToUse == Joystick.AxisOption.Both || axesToUse == Joystick.AxisOption.OnlyHorizontal) {
				if (horizontalAxisEvent != null) {
					horizontalAxisEvent.Invoke(horizontalAxisDelta);
				}
			}
			//If we are recieving input on the vertical axis, set our axis to our vertical delta
			if (axesToUse == Joystick.AxisOption.Both || axesToUse == Joystick.AxisOption.OnlyVertical) {
				if (verticalAxisEvent != null) {
					verticalAxisEvent.Invoke(verticalAxisDelta);
				}
			}
		}

#endregion


#region Valid Touch Registration Override

		protected override void RegisterValidTouch(Touch currentTouch)
		{
			//Register our Joystick
			RegisterValidJoystick(currentTouch);
			//Then call our base
			base.RegisterValidTouch(currentTouch);
		}

		protected override void UnRegisterValidTouch(int currentTouch)
		{
			//UnRegister our joystick
			UnRegisterValidJoystick(currentTouch);

			//Then call our base
			base.UnRegisterValidTouch(currentTouch);
		}

#endregion

#region Register Valid Joystick

		protected virtual void RegisterValidJoystick(Touch currentTouch)
		{
			//If we dont already contain that key, then return
			if (!validJoystickData.ContainsKey(currentTouch.fingerId)) {

				//Create new hold data for our new Finger
				FingerJoystickData holdData = new FingerJoystickData() {
					initialFingerPosition = currentTouch.position,
				};

				//Add our hold data to our dictionary
				validJoystickData.Add(currentTouch.fingerId, holdData);
			}
		}

		protected virtual void UnRegisterValidJoystick(int fingerID)
		{
			//Only unregister if we contain the key
			if (validJoystickData.ContainsKey(fingerID)) {
				//Remove our finger from the hold dictionary
				validJoystickData.Remove(fingerID);
			}
		}

#endregion

	}

}

#endif