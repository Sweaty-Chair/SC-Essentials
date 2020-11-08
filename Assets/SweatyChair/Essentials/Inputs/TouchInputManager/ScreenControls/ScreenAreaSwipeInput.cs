#if CROSS_PLATFORM_INPUT

using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace SweatyChair.InputSystem {

public class ScreenAreaSwipeInput : ScreenAreaControl
{

#region Enum

	public enum PresetSwipeDirection
	{
		RawSwipeDir = -1,   //When our swipe will be read from the raw value in the inspector
		North = 0,
		East = 90,
		South = 180,
		West = 270
	}

#endregion

#region Variables

	[Header("Button Event")]
	public StateChangedEvent buttonDownEvent;
	public StateChangedEvent buttonUpEvent;

	[Header("Swipe Variables")]
	//The Cardinal direction that we will be checking swipes against
	public PresetSwipeDirection overriddenSwipeDirection;
	[Tooltip("The angle direction the swipe will be checking for. (0f = North Swipe, 90f = East Swipe, 180f = South Swipe, 270f = West Swipe)")]
	[Range(0, 360)] public float swipeDirection;

	[Tooltip("The minimum distance the user must swipe in a single frame to register a swipe.")]
	public float minSwipeVelocity = 50f;
	[Tooltip("The angle in degrees that the swipe can be off by and still register a swipe.")]
	public float swipeAngleThreshold = 45f;

	//Internal
	protected List<int> validSwipeFingers = new List<int>();
	private bool lastFrameButtonDown;

#endregion

#region OnEnable / OnDisable

	protected override void OnEnable() {
		base.OnEnable();
		validSwipeFingers = new List<int>();
	}

	protected override void OnDisable() {
		base.OnDisable();

		ReleaseAllButtonsDown();
	}

#endregion

#region Update

	private void Update() {
		//First we Register all our valid touches for this frame
		RegisterAllValidTouches();

		//Release all button downs from last frame
		ReleaseAllButtonsDown();

		//Then we check for any swipes among our registered Finger IDsC
		CheckForSwipe();
	}

#endregion

#region Swipe

	private void CheckForSwipe() {
		//If we are using the raw swipe direction, use the raw swipe direction. Otherwise use our enum as the swipe direction
		float usedSwipeDirection = (overriddenSwipeDirection == PresetSwipeDirection.RawSwipeDir) ? swipeDirection : (int)overriddenSwipeDirection;

		//Go through each of our valid swipes and check if they qualify for our swipe
		for (int i = 0; i < validTouchIDs.Count; i++) {

			//Get a reference to our current touch for this loop
			Touch currentTouch;
			if (!TouchInputManager.GetCachedTouch(validTouchIDs[i], out currentTouch)) { continue; }

			//Check if our swipe is above the threshold for a valid swipe
			if (currentTouch.deltaPosition.magnitude >= minSwipeVelocity) {
				//Get our Normalized Direction angle
				float normalizedAngle = Vector2.SignedAngle(Vector2.down, currentTouch.deltaPosition.normalized);
				//Minus 180 then Abs it so our numbers go in the direction we want
				normalizedAngle -= 180;
				normalizedAngle = Mathf.Abs(normalizedAngle);

				//If our swipe is also within our threshold
				float angleDelta = Mathf.DeltaAngle(usedSwipeDirection, normalizedAngle);
				if (angleDelta >= -swipeAngleThreshold && angleDelta <= swipeAngleThreshold) {

					//If our swipe is valid. Call our swipe method
					OnValidSwipe(currentTouch.fingerId);
				}
			}

		}
	}

	protected virtual void OnValidSwipe(int fingerID) {
		ExecuteButtonState(true);

		//Add our fingers to the valid Swipe
		validSwipeFingers.Add(fingerID);
	}

	protected virtual void ReleaseAllButtonsDown() {
		//If we have any fingers that performed a valid swipe last frame,
		if (validSwipeFingers.Count == 0) { return; }

		//Clear our list, and set our button back up
		validSwipeFingers.Clear();
		ExecuteButtonState(false);
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
	}

#endregion

}

}

#endif