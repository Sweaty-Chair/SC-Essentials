using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SweatyChair.InputSystem
{

	public static class InputManagerUtility
	{

		#region	Variables

		private static Dictionary<InputAxes, string> axesDictionary = new Dictionary<InputAxes, string>();

		#endregion

		#region Constructor

		static InputManagerUtility()
		{
			// Get all our axes in our enum
			List<InputAxes> axesList = EnumUtils.GetValues<InputAxes>().ToList();
			// Then go through each axes in our list and add to dictionary
			for (int i = 0; i < axesList.Count; i++) {
				// If our list already contains our axes, skip this iteration
				if (axesDictionary.ContainsKey(axesList[i])) { continue; }
				// Then add our axes to the dictionary
				string axesName = axesList[i].ToString().Replace('_', ' ');
				axesDictionary.Add(axesList[i], axesName);

			}
		}

		#endregion

		#region Enum To Axes

		public static string AxesNameFromEnum(InputAxes axes)
		{
			// If our Dictionary contains our key
			if (axesDictionary.ContainsKey(axes)) {
				// Return our name
				return axesDictionary[axes];
			} else {
				// Otherwise log, and return the string version of our axes instead
				Debug.LogWarningFormat("Could not get axes for provided enum '{0}'. This shouldn't be possible. Look into this.", axes);
				return axes.ToString();
			}
		}

		#endregion

	}

}
