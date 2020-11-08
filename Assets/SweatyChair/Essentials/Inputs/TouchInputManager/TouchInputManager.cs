using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair.InputSystem
{

	public class TouchInputManager : Singleton<TouchInputManager>
	{

		#region OnEnable / On Disable

		private void OnEnable()
		{
			InitTouchCache();
		}

		private void OnDisable()
		{
			ResetTouchCache();
		}

		#endregion

		#region Update

		private void Update()
		{
			UpdateTouchCache();
		}

		#endregion

		#region Touch Caching

		#region Vars

		private List<int> activeFingerIDs;                  //All our active finger IDs for this current frame
		private Dictionary<int, Touch> activeTouches;       //List to store all our active Touch data for this current frame

		private List<int> unReservedFingerIDs;              //Get our unreserved finger IDs, which any control can access

		private List<int> reservedFingerIDs;                //Get our reserved finger IDs, which stops other controls from using those fingers

		#endregion

		#region Init / Reset

		private void InitTouchCache()
		{
			// Initialize our base vars
			activeFingerIDs = new List<int>();
			activeTouches = new Dictionary<int, Touch>();
			unReservedFingerIDs = new List<int>();
			reservedFingerIDs = new List<int>();
		}

		private void ResetTouchCache()
		{
			// Release all our touches on disable
			ReleaseAllTouches();
		}

		#endregion

		#region Update Touch Cache

		private void UpdateTouchCache()
		{
			// Go through each touch on our device and check for any valid touches we may or may not have
			for (int i = 0; i < Input.touchCount; i++) {
				Touch currentTouch = Input.GetTouch(i);
				// If the system has cancelled or the user ended touch interaction with this finger, make sure to remove this touch.
				if (currentTouch.phase == TouchPhase.Canceled || currentTouch.phase == TouchPhase.Ended) {
					// Unregister our touch, and go to next iteration
					UnRegisterActiveTouch(currentTouch);
				} else {
					// Otherwise our touch is valid, we register it to use
					RegisterActiveTouch(currentTouch);
				}
			}
		}

		#endregion

		#region Register / UnRegister Touches

		private void RegisterActiveTouch(Touch touch)
		{
			// Reference our fingerID
			int fingerID = touch.fingerId;
			// Update our active Touch Dictionary
			if (!activeTouches.ContainsKey(fingerID))
				activeTouches.Add(fingerID, touch);
			else
				activeTouches[fingerID] = touch;
			// If we dont contain our finger ID, add it to our list
			if (!activeFingerIDs.Contains(fingerID)) {
				// Add our finger ID
				activeFingerIDs.Add(fingerID);
				// Add our finger ID to our unreserved finger ID list
				unReservedFingerIDs.Add(fingerID);
			}
		}

		private void UnRegisterActiveTouch(Touch touch)
		{
			UnRegisterActiveTouch(touch.fingerId);
		}
		private void UnRegisterActiveTouch(int fingerID)
		{
			// Make sure our touch list exists
			if (activeFingerIDs == null) { activeFingerIDs = new List<int>(); }
			// If our dictionary contains this touch data, remove it
			if (activeTouches.ContainsKey(fingerID)) { activeTouches.Remove(fingerID); }
			// If we contain this touch, remove our data
			if (activeFingerIDs.Contains(fingerID)) {
				// UnReserve our current touch
				UnReserveTouch(fingerID);
				// Remove our finger ID
				activeFingerIDs.Remove(fingerID);
				// Remove our fingerID from our reserved List
				unReservedFingerIDs.Remove(fingerID);
			}
		}

		private void ReleaseAllTouches()
		{
			// Create a duplicate of our touch array
			List<int> activeFingerIDBackup = new List<int>(activeFingerIDs);
			// Go through our list and deregister all our active touches
			for (int i = 0; i < activeFingerIDBackup.Count; i++)
				UnRegisterActiveTouch(activeFingerIDBackup[i]);
			// Then clear our touch array
			activeFingerIDBackup.Clear();
		}

		#endregion

		#region Get Cached Touch

		public static bool GetCachedTouch(int fingerID, out Touch touch)
		{
			return instance.GetCachedTouchInternal(fingerID, out touch);
		}

		private bool GetCachedTouchInternal(int fingerID, out Touch touch)
		{
			// Return whether we can access that finger data from our array. With our touch as an out
			return activeTouches.TryGetValue(fingerID, out touch);
		}

		#endregion

		#region Unreserved Touches

		/// <summary>
		/// Gets all active touches currently on screen which are not already reserved
		/// </summary>
		/// <returns></returns>
		public static List<Touch> GetUnreservedTouches()
		{
			return instance.GetUnreservedTouchesInternal();
		}
		private List<Touch> GetUnreservedTouchesInternal()
		{
			// Create a list of all our touches
			List<Touch> activeTouches = new List<Touch>();
			// Go through all our touches, and access our finger ids
			for (int i = 0; i < unReservedFingerIDs.Count; i++) {
				// If we do have an active touch for that finger ID
				if (GetCachedTouch(unReservedFingerIDs[i], out Touch currentTouch))
					activeTouches.Add(currentTouch);
			}
			// Return our active touches
			return activeTouches;
		}

		public static List<int> GetUnReservedFingerIDs()
		{
			return instance.GetUnreservedFingerIDsInternal();
		}

		private List<int> GetUnreservedFingerIDsInternal()
		{
			return unReservedFingerIDs;
		}

		#endregion

		#region Reserve Touch

		/// <summary>
		/// Reserve a touch, which stops this fingerID from showing up when polling for touches
		/// </summary>
		/// <param name="fingerID"></param>
		/// <returns>Returns a key used to reference this touch between frames</returns>
		public static int ReserveTouch(int fingerID)
		{
			// Get our touch from fingerID
			if (GetCachedTouch(fingerID, out Touch outTouch)) {
				// Reserve our touch
				return ReserveTouch(outTouch);
			} else {
				// Otherwise return a default finger value
				return -1;
			}
		}

		public static int ReserveTouch(Touch touch)
		{
			//Get our finger ID, and register that
			return instance.ReserveTouchInternal(touch);
		}

		private int ReserveTouchInternal(Touch touch)
		{
			// Make sure our touch list exists
			if (reservedFingerIDs == null) { reservedFingerIDs = new List<int>(); }
			//I f we already contain this touch, return our ID
			if (!reservedFingerIDs.Contains(touch.fingerId)) {
				// Remove our fingerID from our unreserved list
				if (unReservedFingerIDs.Contains(touch.fingerId)) { unReservedFingerIDs.Remove(touch.fingerId); }
				// Add our finger ID to our List, using the finger ID as our key as well
				reservedFingerIDs.Add(touch.fingerId);
				// Return our key in the dictionary
				return touch.fingerId;
			} else {
				return touch.fingerId;
			}
		}

		/// <summary>
		/// UnReserves a touch, which allows this fingerID to show up when polling for active touches
		/// </summary>
		/// <param name="fingerID"></param>
		public static void UnReserveTouch(int fingerID)
		{
			instance.UnReserveTouchInternal(fingerID);
		}
		private void UnReserveTouchInternal(int fingerID)
		{
			// Make sure our touch list exists
			if (reservedFingerIDs == null) { reservedFingerIDs = new List<int>(); }
			// If we dont contain this touch, return
			if (reservedFingerIDs.Contains(fingerID)) {
				// Return our finger ID to our unreserved list
				if (!unReservedFingerIDs.Contains(fingerID)) { unReservedFingerIDs.Add(fingerID); }
				// Remove our finger ID
				reservedFingerIDs.Remove(fingerID);
			}
		}


		/// <summary>
		/// Gets the reserved touch by key
		/// </summary>
		public static int GetReservedTouch(int reserveKey)
		{
			return instance.GetReservedTouchInternal(reserveKey);
		}
		private int GetReservedTouchInternal(int reserveKey)
		{
			// Make sure our touch list exists
			if (reservedFingerIDs == null) { reservedFingerIDs = new List<int>(); }
			// If we dont contain this touch, return an empty list
			if (reservedFingerIDs.Contains(reserveKey)) {
				//Return our Internal Touch
				return reserveKey;
			} else {
				return -1;
			}
		}

		#endregion

		#endregion

		#region Get Touch Basic

		public static bool GetSingleTouchDown()
		{
			return instance.GetSingleTouchDownInternal();
		}

		public bool GetSingleTouchDownInternal()
		{
			//Make sure our touch count is 0
			if (Input.touchCount == 0) { return false; }

			Touch currentTouch = Input.GetTouch(0);

			return currentTouch.phase == TouchPhase.Began;
		}

		public static bool GetSingleTouch()
		{
			return instance.GetSingleTouchInternal();
		}

		public bool GetSingleTouchInternal()
		{
			// Make sure our touch count is 0
			if (Input.touchCount == 0) { return false; }
			Touch currentTouch = Input.GetTouch(0);
			return (currentTouch.phase == TouchPhase.Began || currentTouch.phase == TouchPhase.Moved || currentTouch.phase == TouchPhase.Stationary);
		}

		public static bool GetSingleTouchUp()
		{
			return instance.GetSingleTouchUpInternal();
		}
		public bool GetSingleTouchUpInternal()
		{
			// Make sure our touch count is 0
			if (Input.touchCount == 0) { return false; }
			Touch currentTouch = Input.GetTouch(0);
			return (currentTouch.phase == TouchPhase.Canceled || currentTouch.phase == TouchPhase.Ended);
		}

		#endregion

		#region Gestures

		#region Pinch

		/// <summary>
		/// Returns the pinch delta of our pinch gesture. Returns 0 if no pinch is occuring
		/// </summary>
		/// <returns></returns>
		public static float GetPinchGesture()
		{
			return instance.GetPinchGestureInternal();
		}

		public float GetPinchGestureInternal()
		{
			// If our touch count is not 2 fingers. We are not doing a pinch gesture
			if (Input.touchCount != 2) { return 0; }

			// Get both of our active touches
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			// Find the position in the previous frame of each touch
			Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			return prevTouchDeltaMag - touchDeltaMag;
		}

		#endregion

		#region Two Finger Drag

		/// <summary>
		/// Returns the delta of our drag gesture. Returns 0 if no drag is occuring
		/// </summary>
		/// <returns></returns>
		public static Vector2 GetTwoFingerDrag()
		{
			return instance.GetTwoFingerDragInternal();
		}

		public Vector2 GetTwoFingerDragInternal()
		{
			// If our touch count is not 2 fingers. We are not doing a drag gesture
			if (Input.touchCount != 2) { return Vector2.zero; }
			// Get both of our active touches
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			// Get whether both of our drags are in the same position
			if (Vector2.Dot(touchZero.deltaPosition, touchOne.deltaPosition) <= 0) { return Vector2.zero; }
			// Return the average of our two drag positions
			return (touchZero.deltaPosition + touchOne.deltaPosition) * 0.5f;
		}

		#endregion

		#region MultiTap

		/// <summary>
		/// Returns whether our finger has tapped a specific amount of times
		/// </summary>
		public static bool GetMultiTap(int tapAmount)
		{
			return instance.GetMultiTapInternal(tapAmount);
		}

		public bool GetMultiTapInternal(int tapAmount)
		{
			// If we have more than one touch on the screen return false
			if (Input.touchCount != 1) { return false; }
			// Get our touch
			Touch touchZero = Input.GetTouch(0);
			// Return if we matched our match count
			return touchZero.tapCount == tapAmount;
		}

		#endregion

		#endregion

		#region Utility

		/// <summary>
		/// Converts a vector from pixels to viewport. E.g. (160, 200) on resultion (640, 400) becames (0.25, 0.5).
		/// </summary>
		public static Vector2 ConvertFromPixelToViewport(Vector2 pixelVector)
		{
			return new Vector2(pixelVector.x / Screen.width, pixelVector.y / Screen.height);
		}

		/// <summary>
		/// Converts a value from pixels to viewport, relative to the longer axis. E.g. 160 on resultion (640, 400) becames 0.25.
		/// </summary>
		public static float ConvertFromPixelToViewport(float pixelValue)
		{
			return pixelValue / Mathf.Max(Screen.width, Screen.height);
		}

		#endregion

	}

}
