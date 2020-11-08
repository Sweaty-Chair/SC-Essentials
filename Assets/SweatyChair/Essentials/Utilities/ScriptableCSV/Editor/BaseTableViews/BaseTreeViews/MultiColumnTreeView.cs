using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SweatyChair.TreeView
{
	public class MultiColumnTreeView<T> : TreeViewWithTreeModel<T> where T : TableViewElement
	{
		#region Constants

		const float kRowHeights = 20f;
		const float kToggleWidth = 18f;

		#endregion

		#region Constructor

		public MultiColumnTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TableViewModel<T> model) : base(state, multicolumnHeader, model)
		{
			// Custom setup
			rowHeight = kRowHeights;
			columnIndexForTreeFoldouts = Mathf.Min(1, multiColumnHeader.state.columns.Length - 1);
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = kToggleWidth;
			multicolumnHeader.sortingChanged += OnSortingChanged;

			Reload();
		}

		#endregion

		#region Event Callbacks

		protected virtual void OnSortingChanged(MultiColumnHeader multiColumnHeader)
		{
			SortIfNeeded(rootItem, GetRows());
		}

		#endregion

		#region Build Tree / Views

		// Note we We only build the visible rows, only the backend has the full tree information. 
		// The treeview only creates info for the row list.
		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = base.BuildRows(root);
			SortIfNeeded(root, rows);
			return rows;
		}

		#endregion

		#region OnGUI

		protected override void RowGUI(RowGUIArgs args)
		{
			var item = (TableViewItem<T>)args.item;

			for (int i = 0; i < args.GetNumVisibleColumns(); ++i) {
				CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
			}
		}

		protected virtual void CellGUI(Rect cellRect, TableViewItem<T> item, int columnIndex, ref RowGUIArgs args)
		{
			// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
			CenterRectUsingSingleLineHeight(ref cellRect);
		}

		#endregion

		#region Tree <--> List

		public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
		{
			if (root == null)
				throw new NullReferenceException("root");
			if (result == null)
				throw new NullReferenceException("result");

			result.Clear();

			if (root.children == null)
				return;

			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
			for (int i = root.children.Count - 1; i >= 0; i--)
				stack.Push(root.children[i]);

			while (stack.Count > 0) {
				TreeViewItem current = stack.Pop();
				result.Add(current);

				if (current.hasChildren && current.children[0] != null) {
					for (int i = current.children.Count - 1; i >= 0; i--) {
						stack.Push(current.children[i]);
					}
				}
			}
		}

		#endregion

		#region Sorting

		void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
		{
			if (rows.Count <= 1)
				return;

			//if (multiColumnHeader.sortedColumnIndex == -1) {
			//	return; // No column to sort for (just use the order the data are in)
			//}

			// Sort the roots of the existing tree items
			AttemptSortByMultipleColumns();
			TreeToList(root, rows);
			Repaint();
		}

		protected void AttemptSortByMultipleColumns()
		{
			var sortedColumns = multiColumnHeader.state.sortedColumns;

			// Enforce default [internal id] sorting
			if (sortedColumns.Length == 0) {
				rootItem.children = rootItem.children.OrderBy(child => child.id).ToList();
				return;
			}

			var myTypes = rootItem.children.Cast<TableViewItem<T>>();
			var orderedQuery = GetInitialSortOrder(myTypes, sortedColumns);
			for (int i = 1; i < sortedColumns.Length; i++) {

				bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);
				orderedQuery = GetSortOrder(orderedQuery, sortedColumns[i], ascending);

			}

			rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
		}

		protected IOrderedEnumerable<TableViewItem<T>> GetInitialSortOrder(IEnumerable<TableViewItem<T>> columnData, int[] history)
		{
			bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
			IOrderedEnumerable<TableViewItem<T>> unsortedColumnData = columnData.OrderBy(a => 1);
			return GetSortOrder(unsortedColumnData, history[0], ascending);
		}

		#region Virtual Sort Methods

		protected virtual IOrderedEnumerable<TableViewItem<T>> GetSortOrder(IOrderedEnumerable<TableViewItem<T>> existingQuery, int columnIndex, bool ascending)
		{
			return existingQuery.ThenBy(l => l.data.id, ascending);
		}

		#endregion

		#endregion

		#region Renaming

		protected override bool CanRename(TreeViewItem item)
		{
			// Only allow rename if we can show the rename overlay with a certain width (label might be clipped by other columns)
			Rect renameRect = GetRenameRect(treeViewRect, 0, item);
			return renameRect.width > 30;
		}

		protected override void RenameEnded(RenameEndedArgs args)
		{
			// Set the backend name and reload the tree to reflect the new model
			if (args.acceptedRename) {
				var element = treeModel.Find(args.itemID);
				element.name = args.newName;
				Reload();
			}
		}

		protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
		{
			Rect cellRect = GetCellRectForTreeFoldouts(rowRect);
			CenterRectUsingSingleLineHeight(ref cellRect);
			return base.GetRenameRect(cellRect, row, item);
		}

		#endregion

		#region MultiSelect

		protected override bool CanMultiSelect(TreeViewItem item)
		{
			return true;
		}

		#endregion

	}
}
