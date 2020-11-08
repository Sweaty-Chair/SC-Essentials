using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair.InputSystem
{

	public class ScreenAreaControl : MonoBehaviour
	{

		#region UnityEvent

		[System.Serializable]
		public class StateChangedEvent : UnityEvent { }

		[System.Serializable]
		public class ButtonHeldEvent : UnityEvent<bool> { }

		[System.Serializable]
		public class AxisValueEvent : UnityEvent<float> { }

		[System.Serializable]
		public class intCountEvent : UnityEvent<int> { }

		#endregion

		#region Variables

		[Header("Viewport Settings")]
		public Vector2 viewportRegionMin = Vector2.zero;
		public Vector2 viewportRegionMax = Vector2.one;

		protected List<int> validTouchIDs;

		#endregion

		#region OnEnable / OnDisable

		protected virtual void OnEnable() {
			//Initialize our base Vars
			validTouchIDs = new List<int>();
		}

		protected virtual void OnDisable() {
			ReleaseAllTouches();
		}

		#endregion

		#region Register Valid Touches

		protected void RegisterAllValidTouches() {
			List<Touch> activeTouches = TouchInputManager.GetUnreservedTouches();

			//Deregister all our touches which are no longer active
			List<int> touchesToRemove = validTouchIDs.Except(TouchInputManager.GetUnReservedFingerIDs()).ToList();
			for (int i = 0; i < touchesToRemove.Count; i++) {
				UnRegisterValidTouch(touchesToRemove[i]);
			}

			//Go through each touch on our device and check for any valid touches we may or may not have
			for (int i = 0; i < activeTouches.Count; i++) {

				Touch currentTouch = activeTouches[i];

				//Get our viewspace touch position for this touch
				Vector2 viewportTouchPos = Vector2.Scale(currentTouch.position, new Vector2(1f / Screen.width, 1f / Screen.height));
				//If our touch is within our viewport position
				if (viewportTouchPos.x >= viewportRegionMin.x && viewportTouchPos.y >= viewportRegionMin.y &&
					viewportTouchPos.x <= viewportRegionMax.x && viewportTouchPos.y <= viewportRegionMax.y) {

					RegisterValidTouch(currentTouch);

				} else {

					UnRegisterValidTouch(currentTouch.fingerId);

				}
			}
		}

		protected virtual void RegisterValidTouch(Touch currentTouch) {
			//Make sure our touch list exists
			if (validTouchIDs == null) { validTouchIDs = new List<int>(); }

			//If we already contain this touch, return
			if (validTouchIDs.Contains(currentTouch.fingerId)) { return; }

			//Add our finger ID
			validTouchIDs.Add(currentTouch.fingerId);
		}

		protected virtual void UnRegisterValidTouch(int fingerID) {
			//Make sure our touch list exists
			if (validTouchIDs == null) { validTouchIDs = new List<int>(); }

			//If we dont contain this touch, return
			if (!validTouchIDs.Contains(fingerID)) { return; }

			//Remove our finger ID
			validTouchIDs.Remove(fingerID);
		}

		#endregion

		#region Force Release All Touches

		protected virtual void ReleaseAllTouches() {
			//Go through each of our valid touches.
			for (int i = 0; i < validTouchIDs.Count; i++) {
				//And unregister this touch
				UnRegisterValidTouch(validTouchIDs[i]);
			}
		}

		#endregion

	}

}
