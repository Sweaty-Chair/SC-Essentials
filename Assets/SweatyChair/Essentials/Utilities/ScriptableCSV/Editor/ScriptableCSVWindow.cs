using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SweatyChair.ScriptableCSV
{

	public class ScriptableCSVWindow : EditorWindow
	{

		#region Constants

		private static readonly Type SCRIPTABLE_CSV_ATTRIBUTE = typeof(ScriptableCSVAttribute);

		#endregion

		#region Variables

		private List<ScriptableCSVTreeItem> _scriptableCSVTrees = new List<ScriptableCSVTreeItem>();

		private int _activeCSVIndex = 0;

		#endregion

		#region Show Window

		[MenuItem("Window/Sweaty Chair/Scriptable CSV Window")]
		public static ScriptableCSVWindow GetWindow()
		{
			var window = GetWindow<ScriptableCSVWindow>();
			window.titleContent = new GUIContent("ScriptableCSV Viewer", Resources.Load<Texture>("ScriptableCSVIcon"));
			window.Focus();
			window.Repaint();
			return window;
		}

		#endregion

		#region OnEnable / OnDisable

		private void OnEnable()
		{
			// On Enable we want to enumerate all of our classes which use the Listable Table Attribute, and then assign them to our Dictionary for displaying
			InitializeAllCSVDatabases();

			// Sub to our events
			EditorApplication.projectChanged += OnProjectChanged;
		}

		private void OnDisable()
		{
			// On Disable we want to remove all of our cached data to save memory
			ResetAllCSVDatabases();

			// Sub to our events
			EditorApplication.projectChanged -= OnProjectChanged;
		}

		#endregion

		#region Editor Callbacks

		private void OnProjectChanged()
		{
			// For all initialized windows, we want to force an asset refresh, in the case we added or removed some scriptable objects
			ForceRefreshTreeAssets();
		}

		private void OnSelectionChange()
		{
			// Force our cache to refresh?
			EditorReflection.ScriptAttributeUtility.ClearGlobalCache();
		}

		#endregion

		#region Init

		private void InitializeAllCSVDatabases()
		{
			// Get all of our classes which use our listable attribute type
			List<Type> classesWithListableAttribute = ReflectionUtils.GetAllClassesWithAttribute(SCRIPTABLE_CSV_ATTRIBUTE);

			// Now we go through and Add all valid types to our Dictionary
			AddClassesToTreeDictionary(classesWithListableAttribute);
		}

		private void AddClassesToTreeDictionary(List<Type> types)
		{
			// Go through each of our types, and validate whether they can be added. And if so, add them to our dict
			for (int i = 0; i < types.Count; i++) {

				// Once again, validate whether our type has our correct attribute, and scrape our metadata from it
				var attribute = types[i].GetCustomAttribute(SCRIPTABLE_CSV_ATTRIBUTE) as ScriptableCSVAttribute;
				if (attribute == null)
					continue;

				// Validate whether our type is a scriptable object or not
				if (!types[i].IsSubclassOf(typeof(ScriptableObject))) {
					Debug.LogErrorFormat("[Listable Table Window] - Unable to add type '{0}' to our table window. Currently we only support scripts which extend off 'ScriptableObject'", types[i].FullName);
					continue;
				}

				// Generate our Listable Tree Info
				ScriptableCSVTreeItem curTreeInfo = new ScriptableCSVTreeItem();
				curTreeInfo.type = types[i];
				curTreeInfo.attributeData = attribute;

				// Then just add our data to our dictionary
				if (!_scriptableCSVTrees.Any(data => data.type == types[i]))
					_scriptableCSVTrees.Add(curTreeInfo);
				else
					Debug.LogErrorFormat("[Listable Table Window] - Attempting to add type '{0}' to our list, even though we already have it?", types[i].FullName);
			}

			// Then force sort our list, ordered by our order
			_scriptableCSVTrees = _scriptableCSVTrees.OrderByDescending(treeData => treeData.attributeData.order).ToList();
		}

		#endregion

		#region Reset

		private void ResetAllCSVDatabases()
		{
			_scriptableCSVTrees.Clear();
		}

		#endregion

		#region OnGUI

		private void OnGUI()
		{
			if (_scriptableCSVTrees != null && _scriptableCSVTrees.Count > 0) {

				// Reserve our entire area for our Rect
				Rect fullDisplayRect = GUILayoutUtility.GetRect(position.width, position.height);

				Rect toolbarRect = new Rect(fullDisplayRect.position, new Vector2(fullDisplayRect.width, EditorGUIUtility.singleLineHeight));
				Rect treeviewRect = new Rect(fullDisplayRect);
				treeviewRect.yMin = toolbarRect.yMax + EditorGUIUtility.standardVerticalSpacing;

				string[] tabNames = _scriptableCSVTrees.Select(data => data.GetDisplayName()).ToArray();
				_activeCSVIndex = GUI.Toolbar(toolbarRect, _activeCSVIndex, tabNames, EditorStyles.miniButton);

				// Then display our selected layout
				_scriptableCSVTrees[_activeCSVIndex].OnGUI(treeviewRect);

			} else
				EditorGUILayout.LabelField(new GUIContent("Unable to find any classes which inherit from ListableTableWindow"), EditorStyles.boldLabel);
		}

		#endregion

		#region Utility

		private void ForceRefreshTreeAssets()
		{
			// Go through all of our Tree Assets
			foreach (ScriptableCSVTreeItem CSVTree in _scriptableCSVTrees) {

				if (CSVTree.initialized)
					CSVTree.RefreshTreeModelAssetData();
			}
		}

		#endregion

	}

}
