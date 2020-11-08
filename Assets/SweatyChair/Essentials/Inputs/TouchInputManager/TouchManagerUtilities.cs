#if CROSS_PLATFORM_INPUT

using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair.InputSystem
{

	public static class TouchManagerUtilities
	{

#region Get Closest Touch To Point

		public static bool GetClosestTouchToScreenPoint(Vector2 screenPoint, out Touch outTouch, float maxDistance = -1, TouchPhase phase = TouchPhase.Began)
		{
			//Create a new outTouch, incase our method does not find a valid touch
			outTouch = new Touch();

			//Get all our active touches
			List<Touch> activeTouches = TouchInputManager.GetUnreservedTouches();

			bool hasValidTouch = false;
			float closestTouchDistance = Mathf.Infinity;

			//Iterate through all our touches for a match
			for (int i = 0; i < activeTouches.Count; i++) {
				Touch currentTouch = activeTouches[i];

				//If we are not in the same phase, continue
				if (currentTouch.phase != phase) { continue; }

				//Get our distance between our two points
				float distance = Vector2.Distance(screenPoint, currentTouch.position);

				//If we are checking max Distance
				bool passMaxDistanceCheck = (((maxDistance >= 0) && distance < maxDistance) || maxDistance < 0);
				if (!passMaxDistanceCheck) { continue; }

				//If our distance is less than our max distance
				if (passMaxDistanceCheck && distance < closestTouchDistance) {
					outTouch = currentTouch;
					closestTouchDistance = distance;
					hasValidTouch = true;
				}
			}

			//Return whether we found a match or not
			return hasValidTouch;
		}

		public static bool GetClosestTouchToWorldPoint(Vector3 worldPoint, out Touch outTouch, out Vector3 worldTouchPos, float maxDistance = -1, TouchPhase phase = TouchPhase.Began)
		{
			//Create a new outTouch, incase our method does not find a valid touch
			outTouch = new Touch();
			worldTouchPos = Vector3.zero;

			//Get all our active touches
			List<Touch> activeTouches = TouchInputManager.GetUnreservedTouches();

			bool hasValidTouch = false;
			float closestTouchDistance = Mathf.Infinity;

			//Get our reference to our touch position in world space
			Camera renderCamera = Camera.main;
			Plane perspectivePlane = new Plane(renderCamera.transform.forward, 10f);

			//Project our world point onto our newly created plane
			worldPoint = perspectivePlane.ClosestPointOnPlane(worldPoint);

			//Iterate through all our touches for a match
			for (int i = 0; i < activeTouches.Count; i++) {
				Touch currentTouch = activeTouches[i];

				//If we are not in the same phase, continue
				if (currentTouch.phase != phase) { continue; }

				//Get our TouchPosition in world space
				Vector3 touchWorldPoint = renderCamera.ScreenToWorldPoint(new Vector3(currentTouch.position.x, currentTouch.position.y, 10f));
				touchWorldPoint = perspectivePlane.ClosestPointOnPlane(touchWorldPoint);

				//Get our distance between our two points
				float distance = Vector3.Distance(worldPoint, touchWorldPoint);

				//If we are checking max Distance
				bool passMaxDistanceCheck = (((maxDistance >= 0) && distance < maxDistance) || maxDistance < 0);
				if (!passMaxDistanceCheck) { continue; }

				//If our distance is less than our max distance
				if (passMaxDistanceCheck && distance < closestTouchDistance) {
					outTouch = currentTouch;
					worldTouchPos = touchWorldPoint;
					closestTouchDistance = distance;
					hasValidTouch = true;
				}
			}

			//Return whether we found a match or not
			return hasValidTouch;
		}

#endregion

#region DetectDoubleTap

		public static bool GetDoubleTap(out Touch touch)
		{
			//Get how many matching taps
			return GetMultipleTap(2, out touch);
		}

		public static bool GetMultipleTap(int tapCount, out Touch touch)
		{
			touch = new Touch();

			//Get all of our active, non reserved touches
			List<Touch> activeTouches = TouchInputManager.GetUnreservedTouches();

			//Go through each, and check their tap count
			for (int i = 0; i < activeTouches.Count; i++) {
				if (activeTouches[i].tapCount == tapCount) {
					touch = activeTouches[i];
					return true;
				}
			}
			return false;
		}

#endregion

	}

}

#endif