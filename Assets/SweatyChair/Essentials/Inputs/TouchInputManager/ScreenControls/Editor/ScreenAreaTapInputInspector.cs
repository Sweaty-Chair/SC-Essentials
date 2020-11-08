using UnityEditor;
using UnityEngine;

namespace SweatyChair.InputSystem
{

	[CustomEditor(typeof(ScreenAreaTapInput), true)]
	public class ScreenAreaTapInputInspector : ScreenAreaControlInspector
	{

		#region Variables

		protected override string iconLocation {
			get {
				return "Assets/Editor Default Resources/TouchControls/Editor_TapIcon.png";
			}
		}

		#endregion

	}

}
