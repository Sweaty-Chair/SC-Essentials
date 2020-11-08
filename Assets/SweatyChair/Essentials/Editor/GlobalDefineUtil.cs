using System.Collections.Generic;
using UnityEditor;

namespace SweatyChair
{

	public static class GlobalDefineUtil
	{

		#region Set Defines

		/// <summary>
		/// Sets value of a define for the current build target
		/// </summary>
		/// <param name="defineName"></param>
		/// <param name="isEnabled"></param>
		public static void SetDefine(string defineName, bool isEnabled)
		{
			SetDefine(EditorUserBuildSettings.selectedBuildTargetGroup, defineName, isEnabled);
		}

		/// <summary>
		/// Sets value of a define for the defined build target
		/// </summary>
		/// <param name="group"></param>
		/// <param name="defineName"></param>
		/// <param name="isEnabled"></param>
		public static void SetDefine(BuildTargetGroup group, string defineName, bool isEnabled)
		{
			// Grab our defines list and iterate through the list
			List<string> defines = GetDefinesList(group);

			// If our Define is required to be enabled
			if (isEnabled) {
				//Check if our defines already contains the name, if so we return
				if (defines.Contains(defineName)) { return; }
				//Then in case we did not return. Add our new define
				defines.Add(defineName);


			} else {
				// If our define is required to be removed
				// Check if we already dont have it
				if (!defines.Contains(defineName)) { return; }

				// Then while we still have that define by name, we remove the instance of it
				while (defines.Contains(defineName)) {
					defines.Remove(defineName);
				}
			}

			// Finally, return our defines back to a string. And set our symbols back
			string definesString = string.Join(";", defines.ToArray());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
		}

		#endregion

		#region Is Define Defined?

		public static bool HasDefine(string defineName)
		{
			return HasDefine(EditorUserBuildSettings.selectedBuildTargetGroup, defineName);
		}
		public static bool HasDefine(BuildTargetGroup group, string defineName)
		{
			// Grab our defines list and iterate through the list
			List<string> defines = GetDefinesList(group);

			// Return whether we contain the define or not
			return defines.Contains(defineName);
		}

		#endregion

		#region GetDefines

		/// <summary>
		/// Returns the defines list for the currently active build target group
		/// </summary>
		/// <returns></returns>
		public static List<string> GetDefinesList()
		{
			return GetDefinesList(EditorUserBuildSettings.selectedBuildTargetGroup);
		}
		/// <summary>
		/// Returns the defines list for a specific build group
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public static List<string> GetDefinesList(BuildTargetGroup group)
		{
			return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
		}

		#endregion

	}

}