#if CROSS_PLATFORM_INPUT

using UnityEditor;

namespace SweatyChair.InputSystem
{

	[CustomEditor(typeof(ScreenAreaSwipeInput), true)]
	public class ScreenAreaSwipeInputInspector : ScreenAreaControlInspector
	{

#region Variables

		protected override string iconLocation {
			get {
				return "Assets/Editor Default Resources/TouchControls/Editor_SwipeIcon.png";
			}
		}

#endregion

	}

}

#endif