#if CROSS_PLATFORM_INPUT

using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair.InputSystem
{

	public class DisableInputOnEnable : MonoBehaviour
	{

		[Header("Settings")]
		[SerializeField] private bool disableInput;

		[Header("Axes For Disable")]
		[SerializeField] private List<InputAxes> buttonsToDisable = new List<InputAxes>();
		[SerializeField] private List<InputAxes> axisToDisable = new List<InputAxes>();
		[SerializeField] private List<MouseButton> dragTypesToDisable = new List<MouseButton>();

		#region OnEnable / Disable

		private void OnEnable()
		{

			//Disable all of our buttons
			DisableButtons(buttonsToDisable, disableInput);

			//Disable all of our Axis
			DisableAxis(axisToDisable, disableInput);

			//Disable all of our MouseButtons
			DisableMouseDrags(dragTypesToDisable, disableInput);
		}

		private void OnDisable()
		{
			//Re-Enable all of our buttons
			DisableButtons(buttonsToDisable, !disableInput);

			//Re-Enable all of our Axis
			DisableAxis(axisToDisable, !disableInput);

			//Re-Enable all of our MouseButtons
			DisableMouseDrags(dragTypesToDisable, !disableInput);
		}

		#endregion

		#region Disablers

		private void DisableButtons(List<InputAxes> buttons, bool isDisabled)
		{
			//If our list is not null
			if (buttons != null) {
				//Go through each of our buttons and set them to whatever state required
				for (int i = 0; i < buttons.Count; i++)
					InputManager.SetButtonDisabled(buttons[i], isDisabled);
			}
		}

		private void DisableAxis(List<InputAxes> axis, bool isDisabled)
		{
			//If our list is not null
			if (axis != null) {
				//Go through each of our buttons and set them to whatever state required
				for (int i = 0; i < axis.Count; i++) {
					InputManager.SetAxisDisabled(axis[i], isDisabled);
				}

			}
		}

		private void DisableMouseDrags(List<MouseButton> mouseDrags, bool isDisabled)
		{
			//If our list is not null
			if (mouseDrags != null) {
				//Go through each of our buttons and set them to whatever state required
				for (int i = 0; i < mouseDrags.Count; i++) {
					InputManager.SetDragDisabled(mouseDrags[i], isDisabled);
				}

			}
		}

		#endregion

	}

}

#endif