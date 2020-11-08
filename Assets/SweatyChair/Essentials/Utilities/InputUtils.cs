#if CROSS_PLATFORM_INPUT

using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace SweatyChair
{

	/// <summary>
	/// A utility that provides handy functions for inputs.
	/// </summary>
	public static class InputUtils
	{

		public static bool IsAxisAvailable(string axisName)
		{
			try {
				Input.GetAxis(axisName);
				return true;
			} catch (UnityException e) {
				return false;
			}
		}

		public static bool IsButtonAvailable(string btnName)
		{
			try {
				Input.GetButton(btnName);
				return true;
			} catch (UnityException e) {
				return false;
			}
		}

#region Create Virtual Axis

#if UNITY_ANDROID || UNITY_IOS

		public static void CreateVirtualAxis(string axisName)
		{
			if (!CrossPlatformInputManager.AxisExists(axisName))
				CrossPlatformInputManager.RegisterVirtualAxis(new CrossPlatformInputManager.VirtualAxis(axisName));
		}

		public static void CreateVirtualButton(string axisName)
		{
			if (!CrossPlatformInputManager.ButtonExists(axisName))
				CrossPlatformInputManager.RegisterVirtualButton(new CrossPlatformInputManager.VirtualButton(axisName));
		}

#endif

#endregion

	}

}

#endif