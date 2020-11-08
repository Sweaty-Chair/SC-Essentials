using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair.InputSystem
{

	public class ScreenAreaTapInput : ScreenAreaControl
	{

		#region Variables

		[Header("Button Event")]
		public StateChangedEvent tapEvent;
		public intCountEvent tapCountEvent;

		[Header("Tap Variables")]
		[Tooltip("The minimum number of touches before our event starts sending. (Inclusive)")]
		public int minTapCount = -1;
		[Tooltip("The maximum number of touches before our event stops sending. (Inclusive)")]
		public int maxTapCount = -1;

		#endregion

		#region Update

		private void Update() {
			//First Register all our valid touches for this frame
			RegisterAllValidTouches();

			//Then check for any holding data
			UpdateTapState();
		}

		#endregion

		#region Hold

		private void UpdateTapState() {
			//Go through each valid touch, and check if it qualifies for a hold
			for (int i = 0; i < validTouchIDs.Count; i++) {

				Touch currentTouch;
				if (!TouchInputManager.GetCachedTouch(validTouchIDs[i], out currentTouch)) { continue; }

				//Check if our movement is above the threshold, and our finger hold started during the first touch down
				if (currentTouch.tapCount >= minTapCount && currentTouch.tapCount <= maxTapCount) {
					tapEvent?.Invoke();
					tapCountEvent?.Invoke(currentTouch.tapCount);
				}
			}
		}

		#endregion

	}

}
