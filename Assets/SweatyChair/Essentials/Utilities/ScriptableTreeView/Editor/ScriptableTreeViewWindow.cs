using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SweatyChair.ScriptableTreeView
{

	public class ScriptableTreeViewWindow : EditorWindow
	{

		#region Constants

		private static readonly Type SCRIPTABLE_TREE_VIEW_ATTRIBUTE = typeof(ScriptableTreeViewAttribute);

		#endregion

		#region Variables

		private List<ScriptableTreeViewPanelData> _scriptableTreeViewTrees = new List<ScriptableTreeViewPanelData>();

		private int _activeCSVIndex = 0;

		#endregion

		#region Show Window

		[MenuItem("Window/Sweaty Chair/Scriptable Tree View Editor")]
		public static ScriptableTreeViewWindow GetWindow()
		{
			// Get existing open window or if none, make a new one:
			ScriptableTreeViewWindow window = GetWindow<ScriptableTreeViewWindow>();
			window.titleContent = new GUIContent("Scriptable Tree View Editor", EditorGUIUtility.FindTexture("UnityEditor.SceneHierarchyWindow"));
			window.Focus();
			window.Repaint();
			return window;
		}

		#endregion

		#region OnEnable / OnDisable

		private void OnEnable()
		{
			InitializeAllTreeViews();
		}

		private void OnDisable()
		{
			ResetAllTreeViews();
		}

		#endregion

		#region Editor Callbacks

		private void OnSelectionChange()
		{
			EditorReflection.ScriptAttributeUtility.ClearGlobalCache();
		}

		#endregion

		#region Init

		private void InitializeAllTreeViews()
		{
			// Enumerate all classes with our attribute
			List<Type> classesWithTreeAttribute = ReflectionUtils.GetAllClassesWithAttribute(SCRIPTABLE_TREE_VIEW_ATTRIBUTE);

			// Then Go through and add all of our valid types to our list
			AddAllTreeViewsToList(classesWithTreeAttribute);
		}

		private void AddAllTreeViewsToList(List<Type> classList)
		{
			for (int i = 0; i < classList.Count; i++) {
				// Get our tree View item.
				ScriptableTreeViewPanelData viewItem = InitializeTreeViewItem(classList[i]);
				if (viewItem != null) {

					// Then enforce that we are not adding any data more than once
					if (!_scriptableTreeViewTrees.Any(data => data.type == viewItem.type))
						_scriptableTreeViewTrees.Add(viewItem);
					else
						Debug.LogErrorFormat("[Scriptable Tree View Window] - Attempting to add type '{0}' to our list, even though we already have it?", viewItem.type.FullName);
				}
			}

			// Once all of our data has been added, Lets sort our list
			_scriptableTreeViewTrees = _scriptableTreeViewTrees.OrderByDescending(treeView => treeView.attribute.order).ToList();
		}

		private ScriptableTreeViewPanelData InitializeTreeViewItem(Type type)
		{
			var attribute = type.GetCustomAttribute(SCRIPTABLE_TREE_VIEW_ATTRIBUTE) as ScriptableTreeViewAttribute;
			if (attribute == null)
				return null;

			// Validate whether our type is a child of ScriptableObjectSingleton
			if (!ReflectionUtils.IsSubClassOfGeneric(type, typeof(ScriptableObjectSingleton<>))) {
				Debug.LogErrorFormat("[Scriptable Tree View Window] - Unable to add type '{0}' to our window. Currently we only support scripts which extend off 'ScriptableObjectSingleton'", type.FullName);
				return null;
			}

			// Generate our Tree view info
			return new ScriptableTreeViewPanelData(type, attribute);
		}

		#endregion

		#region Reset

		private void ResetAllTreeViews()
		{
			_scriptableTreeViewTrees.Clear();
		}

		#endregion

		#region OnGUI

		private void OnGUI()
		{
			if (_scriptableTreeViewTrees != null && _scriptableTreeViewTrees.Count > 0) {

				// Reserve our entire area for our Rect
				Rect fullDisplayRect = GUILayoutUtility.GetRect(position.width, position.height);

				Rect toolbarRect = new Rect(fullDisplayRect.position, new Vector2(fullDisplayRect.width, EditorGUIUtility.singleLineHeight));
				Rect treeviewRect = new Rect(fullDisplayRect);
				treeviewRect.yMin = toolbarRect.yMax + EditorGUIUtility.standardVerticalSpacing;

				string[] tabNames = _scriptableTreeViewTrees.Select(data => data.GetDisplayName()).ToArray();
				_activeCSVIndex = GUI.Toolbar(toolbarRect, _activeCSVIndex, tabNames, EditorStyles.miniButton);

				// Then display our selected layout
				_scriptableTreeViewTrees[_activeCSVIndex].OnGUI(treeviewRect);

			} else
				EditorGUILayout.LabelField(new GUIContent(string.Format("Unable to find any classes which inherit from {0}", SCRIPTABLE_TREE_VIEW_ATTRIBUTE.Name)), EditorStyles.boldLabel);
		}



		#endregion

	}

}
