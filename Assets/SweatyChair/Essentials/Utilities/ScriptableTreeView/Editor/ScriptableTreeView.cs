using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SweatyChair.ScriptableTreeView
{

	public class ScriptableTreeView : UnityEditor.IMGUI.Controls.TreeView
	{

		#region Variables

		private Type treeType;
		private SerializedObject _scriptableObjectInstance;

		#endregion

		#region Constructor

		public ScriptableTreeView(Type type, TreeViewState state) : base(state)
		{
			treeType = type;

			showAlternatingRowBackgrounds = true;

			Reload();
		}

		#endregion

		#region Build Tree

		protected override TreeViewItem BuildRoot()
		{
			ScriptableTreeViewElement root = new ScriptableTreeViewElement("root", -1, 0);

			// Some meme reflection stuff
			var property = treeType.GetProperty("current", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			var value = property.GetValue(null);
			var castedValue = value as UnityEngine.Object;
			_scriptableObjectInstance = new SerializedObject(castedValue);

			root.children = CreateTreeViewFromData();

			SetupDepthsFromParentsAndChildren(root);

			return root;
		}

		#endregion

		#region Enumerate and Sort Inspector

		private List<TreeViewItem> CreateTreeViewFromData()
		{
			// Create our Root header data which will hold all of our data
			ScriptableTreeViewElement rootData = new ScriptableTreeViewElement(string.Empty, -1, -1);

			int index = 1;
			string cachedHeader = string.Empty;
			TreeViewItem currentViewItem = rootData;

			// Use reflection to find all of the fields in our class
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			// Sort our field info to preserve our display order
			IEnumerable<FieldInfo> sortedFieldInfo = treeType.GetFields(flags).OrderBy(item => item.MetadataToken);
			foreach (FieldInfo field in sortedFieldInfo) {
				bool isSystemSerialized = !field.IsNotSerialized;
				bool isUnitySerialized = !field.IsPrivate || Attribute.IsDefined(field, typeof(SerializableAttribute));

				if (isSystemSerialized && isUnitySerialized) {

					// Now we check if we have to update our header data
					HeaderAttribute header = field.GetCustomAttribute<HeaderAttribute>();
					if (header != null) {
						cachedHeader = header.header;
						// And reset our starting point
						currentViewItem = rootData;

						// Then We attempt to navigate to our new point to add our data
						// Get our part string array for our header
						string[] headerSegments = cachedHeader.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
						if (headerSegments != null || headerSegments.Length > 0) {

							// Go through each of our segments and attempt to get to our header data
							for (int i = 0; i < headerSegments.Length; i++) {

								TreeViewItem foundData = currentViewItem.children?.FirstOrDefault(item => item.displayName == headerSegments[i]);
								if (foundData == null) {
									foundData = new ScriptableTreeViewElement(headerSegments[i], -1, index++);
									currentViewItem.AddChild(foundData);
								}
								currentViewItem = foundData;
							}
						}
					}

					// Then we just need to get our serialized property from our data
					SerializedProperty currentProp = _scriptableObjectInstance.FindProperty(field.Name);

					TreeViewItem newItem = new ScriptableTreeViewElement(currentProp, -1, index++);
					currentViewItem.AddChild(newItem);
				}
			}

			// If we have no children, create a default header row with our error
			if (rootData.children == null || rootData.children.Count <= 0)
				rootData.AddChild(new ScriptableTreeViewElement("There are no public variables available in our class. Add one to have it show up.", -1, 1));

			// Before we return, recursively sort our children to be in a neater order
			SortChildrenRecursively(rootData);

			return rootData.children;
		}

		#region Linq Sorting

		private void SortChildrenRecursively(TreeViewItem parentData)
		{
			// If we have no children, return
			if (parentData.children == null)
				return;

			// Sort our list
			parentData.children = parentData.children.OrderBy(InitialSort).ToList();

			// Then go through each of our children and sort again
			for (int i = 0; i < parentData.children.Count; i++) {
				SortChildrenRecursively(parentData.children[i]);
			}
		}

		private object InitialSort(TreeViewItem data)
		{
			ScriptableTreeViewElement element = data as ScriptableTreeViewElement;
			if (element != null)
				return element.objectProperty != null;
			else
				return data;
		}

		#endregion

		#endregion

		#region On GUI

		public override void OnGUI(Rect rect)
		{
			// Update our Scriptable object representation
			_scriptableObjectInstance.Update();

			// Set wide mode for our vector 2 and stuff
			bool wideMode = EditorGUIUtility.wideMode;
			EditorGUIUtility.wideMode = true;

			// Draw our defualt GUI
			base.OnGUI(rect);

			// Revert wide Mode
			EditorGUIUtility.wideMode = wideMode;

			// Apply all of our changes
			_scriptableObjectInstance.ApplyModifiedProperties();
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			// Now we want to draw each of our rows correctly
			Event evt = Event.current;

			// Attempt to get our treeviewitem
			ScriptableTreeViewElement balanceItem = args.item as ScriptableTreeViewElement;
			if (balanceItem == null) {
				base.RowGUI(args);
				return;
			}

			// Now if we have our tree item, we can try to get our data and work with it
			bool isHeaderOnly = balanceItem.objectProperty == null;
			// if we are header only, draw our default GUI
			if (isHeaderOnly) {

				// Bootleg shit to allow us to click on entire header to expand
				if (evt.type == EventType.MouseDown && args.rowRect.Contains(evt.mousePosition)) {
					SetExpanded(args.item.id, !IsExpanded(args.item.id));
					evt.Use();
				}
				base.RowGUI(args);
				return;
			}

			// Finally if we have a serialized property, draw our default gui
			Rect indentRect = new Rect(args.rowRect);

			// Ensure row is selected before using the control (usability)
			if (evt.type == EventType.MouseDown && indentRect.Contains(evt.mousePosition)) {
				SelectionClick(args.item, false);
			}

			// Check if our expanded data changed
			bool cachedPropertyExpanded = balanceItem.objectProperty.isExpanded;

			float indentAmount = GetContentIndent(args.item);
			EditorGUIUtility.labelWidth = (indentRect.width / 2) - indentAmount;
			indentRect.xMin += indentAmount;

			bool newPropertyExpanded = CustomPropertyField(indentRect, balanceItem.objectProperty, null, true, false);

			// Bool get if our expanded state changed
			if (cachedPropertyExpanded != newPropertyExpanded)
				RefreshCustomRowHeights();
		}

		protected override float GetCustomRowHeight(int row, TreeViewItem item)
		{
			ScriptableTreeViewElement balanceItem = item as ScriptableTreeViewElement;

			return 4 + ((balanceItem != null && balanceItem.objectProperty != null) ? GetPropertyHeightReal(balanceItem.objectProperty, GUIContent.none) : base.GetCustomRowHeight(row, item));
		}

		#endregion

		#region Searching

		protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
		{
			ScriptableTreeViewElement balanceItem = item as ScriptableTreeViewElement;
			return base.DoesItemMatchSearch(item, search) && (balanceItem == null || balanceItem.objectProperty != null);
		}


		#endregion

		#region Utility

		public static bool CustomPropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren, bool includePropertyDrawers)
		{
			return SweatyChair.EditorReflection.ScriptAttributeUtility.GetHandler(property).OnGUI(position, property, label, includeChildren, includePropertyDrawers);
		}
		private static float GetPropertyHeightReal(SerializedProperty property, GUIContent label)
		{
			return SweatyChair.EditorReflection.ScriptAttributeUtility.GetHandler(property).GetHeight(property, label, true, false);
		}

		#endregion

	}

}
