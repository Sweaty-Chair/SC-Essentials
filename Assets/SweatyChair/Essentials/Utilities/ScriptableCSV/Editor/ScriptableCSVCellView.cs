using SweatyChair.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SweatyChair.ScriptableCSV
{

	public class ScriptableCSVCellView : MultiColumnTreeView<ScriptableCSVCellItem>
	{

		#region Const 

		public static readonly RectOffset CELL_PADDING = new RectOffset(0, 0, 2, 2);

		#endregion

		#region Variables

		public List<FieldInfo> currentFieldInfo;

		#endregion

		#region Constructor

		public ScriptableCSVCellView(List<FieldInfo> fieldInfo, TreeViewState state, MultiColumnHeader multicolumnHeader, TableViewModel<ScriptableCSVCellItem> model) : base(state, multicolumnHeader, model)
		{
			currentFieldInfo = fieldInfo;
			RefreshCustomRowHeights();

			// Then sub to our column change event
			multicolumnHeader.visibleColumnsChanged += OnVisibleColumnsChanged;
		}

		#endregion

		#region Event Callbacks

		protected override void DoubleClickedItem(int id)
		{
			// Ping our object in the project view on double click of an item
			ScriptableCSVCellItem tableViewElement = treeModel.Find(id) as ScriptableCSVCellItem;
			EditorGUIUtility.PingObject(tableViewElement.scriptableObject);
		}

		protected override void OnSortingChanged(MultiColumnHeader multiColumnHeader)
		{
			base.OnSortingChanged(multiColumnHeader);

			// Force a refresh of our search
			ForceSearchStringUpdate();
		}

		protected virtual void OnVisibleColumnsChanged(MultiColumnHeader multiColumnHeader)
		{
			// Refresh our row heights
			RefreshCustomRowHeights();
		}

		#endregion

		#region OnGUI

		protected override float GetCustomRowHeight(int row, TreeViewItem item)
		{
			base.GetCustomRowHeight(row, item);
			var viewElements = (TableViewItem<ScriptableCSVCellItem>)item;

			float maxRowHeight = EditorGUIUtility.singleLineHeight;

			// Get the max size heights of all of our data
			if (currentFieldInfo != null) {
				for (int i = 0; i < currentFieldInfo.Count; i++) {

					// Check if this current column is even showing
					if (!multiColumnHeader.IsColumnVisible(i))
						continue;

					FieldInfo curInfo = currentFieldInfo[i];
					if (curInfo != null) {

						SerializedProperty curCellProperty = viewElements.data.GetSerializedProperty(curInfo.Name);

						// Fix for issue where expandable data isnt correctly showing because of foldout
						float bonusHeight = curCellProperty.isExpanded ? EditorGUIUtility.singleLineHeight : 0;

						//Then get our Height
						maxRowHeight = Mathf.Max(GetPropertyHeightReal(curCellProperty, GUIContent.none) + bonusHeight, maxRowHeight);
					}
				}
			}

			return maxRowHeight + CELL_PADDING.vertical;
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			args.label = "";

			base.RowGUI(args);
		}

		protected override void CellGUI(Rect cellRect, TableViewItem<ScriptableCSVCellItem> item, int columnIndex, ref RowGUIArgs args)
		{
			base.CellGUI(cellRect, item, columnIndex, ref args);

			// Update our properties
			item.data.serializedObject.Update();

			// Get if the user clicked on our element. Because of an issue with the control eating our event, we have to set our bool before drawing our GUI
			bool forceRefresh = Event.current.type == EventType.MouseUp && cellRect.Contains(Event.current.mousePosition);

			// Now we just Display our UI for our current column data
			FieldInfo curInfo = currentFieldInfo[columnIndex];
			if (curInfo != null) {

				// Then we slightly shrink our cell heights, so its easier to see our selected row
				cellRect.yMin += CELL_PADDING.top;
				cellRect.yMax -= CELL_PADDING.bottom;

				// Cell GameData for Height
				SerializedProperty curCellProperty = item.data.GetSerializedProperty(curInfo.Name); // Is it because we are getting our property every frame too?

				cellRect.height = GetPropertyHeightReal(curCellProperty, GUIContent.none);
				CustomPropertyField(cellRect, curCellProperty, GUIContent.none, true, false);

			} else
				EditorGUI.LabelField(cellRect, GUIContent.none);

			item.data.serializedObject.ApplyModifiedProperties();

			// Force Refresh if we get a mouse event
			if (forceRefresh) {
				RefreshCustomRowHeights();
				SelectionClick(item, Event.current.shift);
			}
		}

		#endregion

		#region Sorting

		protected override IOrderedEnumerable<TableViewItem<ScriptableCSVCellItem>> GetSortOrder(IOrderedEnumerable<TableViewItem<ScriptableCSVCellItem>> existingQuery, int columnIndex, bool ascending)
		{
			// Then lets just finally use reflection to order our query
			if (currentFieldInfo[columnIndex] != null)
				return existingQuery.ThenBy(i => currentFieldInfo[columnIndex].GetValue(i.data.scriptableObject), ascending);
			else
				return existingQuery;
		}

		#endregion

		#region Searching

		protected override bool GetSearchPredicate(string searchString, ScriptableCSVCellItem current)
		{
			// Get our Currently Selected Column, and do our search on that. If we have no selected column, do our default search
			FieldInfo columnFieldInfo = null;
			if (multiColumnHeader.sortedColumnIndex >= 0)
				columnFieldInfo = currentFieldInfo[multiColumnHeader.sortedColumnIndex];

			if (columnFieldInfo != null)
				// Check if our item contains the string value
				return columnFieldInfo.GetValue(current.scriptableObject).ToString().IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;

			else {
				// Otherwise lets search all of our visible fields for any matching text
				for (int i = 0; i < currentFieldInfo.Count; i++) {

					// First check if our field is visible
					if (!multiColumnHeader.state.visibleColumns.Contains(i))
						continue;

					// Then check for matching string
					bool matchingString = currentFieldInfo[i].GetValue(current.scriptableObject).ToString().IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
					if (matchingString)
						return true;
				}
				return false;
			}
		}

		#endregion

		#region Renaming

		protected override bool CanRename(TreeViewItem item)
		{
			return false;
		}

		protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
		{
			return Rect.zero;
		}

		protected override void RenameEnded(RenameEndedArgs args)
		{
		}

		#endregion

		#region Utility

		public static bool CustomPropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren, bool includePropertyDrawers)
		{
			return EditorReflection.ScriptAttributeUtility.GetHandler(property).OnGUI(position, property, label, includeChildren, includePropertyDrawers);
		}
		public static bool CustomPropertyFieldLayout(SerializedProperty property, GUIContent label, bool includeChildren, bool includePropertyDrawers, params GUILayoutOption[] options)
		{
			return EditorReflection.ScriptAttributeUtility.GetHandler(property).OnGUILayout(property, label, includeChildren, includePropertyDrawers, options);
		}
		private static float GetPropertyHeightReal(SerializedProperty property, GUIContent label)
		{
			return EditorReflection.ScriptAttributeUtility.GetHandler(property).GetHeight(property, label, true, false);
		}

		#endregion

	}

}
