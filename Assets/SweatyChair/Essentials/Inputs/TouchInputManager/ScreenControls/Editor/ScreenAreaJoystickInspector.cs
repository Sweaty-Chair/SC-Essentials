#if CROSS_PLATFORM_INPUT

using UnityEditor;

namespace SweatyChair.InputSystem
{

	[CustomEditor(typeof(ScreenAreaJoystickInput), true)]
	public class ScreenAreaJoystickInspector : ScreenAreaControlInspector
	{

#region Variables

		protected override string iconLocation {
			get {
				return "Assets/Editor Default Resources/TouchControls/Editor_DPadIcon.png";
			}
		}

#endregion

	}

}

#endif