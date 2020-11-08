using System;
using System.Reflection;
using UnityEngine;

namespace SweatyChair.ScriptableCSV
{

	public class ScriptableCSVFieldData
	{

		#region Constant

		private static readonly Type TOOLTIP_ATTRIBUTE = typeof(TooltipAttribute);
		private static readonly Type COLUMN_DATA_ATTRIBUTE = typeof(ScriptableCSVColumnAttribute);

		#endregion

		#region Variables

		public FieldInfo fieldInfo { get; protected set; }

		public bool hasTooltipData => tooltipData != null;
		public TooltipAttribute tooltipData { get; protected set; }

		public bool hasColumnDataAttribute => columnData != null;
		public ScriptableCSVColumnAttribute columnData { get; protected set; }

		#endregion

		#region Constructor

		public ScriptableCSVFieldData(FieldInfo fieldInfo)
		{
			// Set our field Info
			this.fieldInfo = fieldInfo;

			// Then check and get our attributes from our field
			tooltipData = (TooltipAttribute)this.fieldInfo.GetCustomAttribute(TOOLTIP_ATTRIBUTE);
			columnData = (ScriptableCSVColumnAttribute)this.fieldInfo.GetCustomAttribute(COLUMN_DATA_ATTRIBUTE);
		}

		#endregion

	}

}
