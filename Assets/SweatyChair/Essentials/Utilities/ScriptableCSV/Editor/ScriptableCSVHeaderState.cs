using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SweatyChair.ScriptableCSV
{

	public class ScriptableCSVHeaderState : MultiColumnHeaderState
	{

		#region Constructor

		public ScriptableCSVHeaderState(List<ScriptableCSVFieldData> sortedDisplayData) : base(GetDefaultColumns(sortedDisplayData)) { }

		#endregion

		#region Utility

		private static Column[] GetDefaultColumns(List<ScriptableCSVFieldData> columnData)
		{
			Column[] columns = new Column[columnData.Count];

			// Then Create all of our headers
			for (int i = 0; i < columnData.Count; i++) {

				// Get our data from our List
				ScriptableCSVFieldData curData = columnData[i];
				FieldInfo curFieldInfo = curData.fieldInfo;

				// Get our Display name from our data
				string displayName = ObjectNames.NicifyVariableName(curData.fieldInfo.Name);

				// Get our Longer data
				float labelWidth = GUI.skin.label.CalcSize(new GUIContent(curData.fieldInfo.Name)).x + 10;
				bool supportsSorting = curData.fieldInfo.FieldType.IsValueType || curData.fieldInfo.FieldType.IsPrimitive || typeof(IComparable).IsAssignableFrom(curData.fieldInfo.FieldType);

				string tooltipText = (curData.hasTooltipData) ? curData.tooltipData?.tooltip : string.Empty;

				// Finally Generate our column data
				columns[i] = new Column {
					// Header + Context
					headerContent = new GUIContent(displayName, tooltipText),
					headerTextAlignment = TextAlignment.Center,

					// Size of Columns
					autoResize = true,
					width = labelWidth,
					minWidth = (curData.columnData?.minWidth).GetValueOrDefault(labelWidth),
					maxWidth = (curData.columnData?.maxWidth).GetValueOrDefault(10000000f),

					// Sorting
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					canSort = (curData?.columnData?.allowSorting).GetValueOrDefault(true) && supportsSorting,

					// Assorted
					allowToggleVisibility = (curData?.columnData?.allowToggleVisibility).GetValueOrDefault(true),
				};
			}

			return columns;
		}

		private static Column CreateEmptyHeader()
		{
			return new Column {
				headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByLabel")),
				headerTextAlignment = TextAlignment.Center,
				canSort = false,
				width = 30,
				minWidth = 30,
				maxWidth = 30,
				autoResize = false,
				allowToggleVisibility = false
			};
		}

		#endregion

	}

}
