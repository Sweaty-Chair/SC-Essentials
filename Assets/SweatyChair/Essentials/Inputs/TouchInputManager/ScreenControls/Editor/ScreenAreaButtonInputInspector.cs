#if CROSS_PLATFORM_INPUT

using UnityEditor;

namespace SweatyChair.InputSystem
{

	[CustomEditor(typeof(ScreenAreaButtonInput), true)]
	public class ScreenAreaButtonInputInspector : ScreenAreaControlInspector
	{

#region Variables

		protected override string iconLocation {
			get {
				return "Assets/Editor Default Resources/TouchControls/Editor_ButtonIcon.png";
			}
		}

#endregion

	}

}

#endif