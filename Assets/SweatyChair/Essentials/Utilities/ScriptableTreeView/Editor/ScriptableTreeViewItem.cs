using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SweatyChair.ScriptableTreeView
{

	public class ScriptableTreeViewPanelData
	{

		#region Variables

		public bool initialized { get; private set; }

		// Data
		public Type type;
		public ScriptableTreeViewAttribute attribute;

		// Tree Data
		public UnityEditor.IMGUI.Controls.TreeView treeView;
		public TreeViewState treeViewState;

		// Search GUI
		public SearchField searchField;

		#endregion

		#region Constructor

		public ScriptableTreeViewPanelData(Type type, ScriptableTreeViewAttribute attribute)
		{
			this.type = type;
			this.attribute = attribute;
		}

		#endregion

		#region Generate Tree

		public void GenerateTree()
		{
			// Dont re-initialize if we have already initialized once
			if (initialized)
				return;

			// Init our state if it does not yet exist
			if (treeViewState == null)
				treeViewState = new TreeViewState();

			// Initialize and Generate our tree view
			treeView = new ScriptableTreeView(type, treeViewState);

			// Generate our Search Field
			searchField = new SearchField();
			searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;

			// Set our flags
			initialized = true;

		}

		#endregion

		#region OnGUI

		public void OnGUI(Rect position)
		{
			// Initialize our tree if Required
			if (!initialized)
				GenerateTree();

			// Finally check if we have an initialized tree view at all
			if (initialized) {

				// Generate our Rects
				Rect toolbarRect = new Rect(position.xMin + 10, position.y + 10, position.width - 20f, 20f);

				// Initialize our treeviewRect
				Rect treeViewRect = new Rect();
				treeViewRect.xMin = position.xMin + 10;
				treeViewRect.xMax = position.xMax - 10;
				treeViewRect.yMin = toolbarRect.yMax + EditorGUIUtility.standardVerticalSpacing;
				treeViewRect.yMax = position.yMax - EditorGUIUtility.standardVerticalSpacing;

				// Then We Generate our Searchfield
				treeView.searchString = searchField.OnGUI(toolbarRect, treeView.searchString);
				treeView.OnGUI(treeViewRect);

			} else {
				string errorMessage = string.Format("Unable to display Editor for Type '{0}', We have no serializable variables to show...", type.Name);

				GUIStyle centeredErrorStyle = new GUIStyle(EditorStyles.boldLabel);
				centeredErrorStyle.alignment = TextAnchor.MiddleCenter;
				centeredErrorStyle.wordWrap = true;
				centeredErrorStyle.padding = new RectOffset(30, 30, 0, 0);

				EditorGUI.LabelField(position, new GUIContent(errorMessage), centeredErrorStyle);
			}
		}

		#endregion

		#region Utility

		public string GetDisplayName()
		{
			// If we have no display name, return our full type name, Otherwise display our display name provided
			if (string.IsNullOrWhiteSpace(attribute.displayName))
				return type.Name;
			else
				return attribute.displayName;
		}

		#endregion

	}

}
