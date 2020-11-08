using UnityEngine;

namespace SweatyChair.InputSystem {

	public class RebindableInputDataSO : ScriptableObject {

		#region Variables

		//Identifiers
		public string AccessKeyName;
		public string Category;

		//Input axis
		public RebindableType RebindableDataType;

		#region Button Specific GameData

		//CHANGE THIS TO AN ENUM FOR EVERY POSSIBLE INPUT CONFIG
		public string[] ButtonListenAxes;

		#endregion

		#region Axis Specific GameData
		//CHANGE THIS TO AN ENUM FOR EVERY POSSIBLE INPUT CONFIG
		public string[] AxisListenAxes;
		public bool IsAxisInverted;

		#endregion

		#endregion

	}

}
