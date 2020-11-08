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

	public class ScriptableCSVTreeItem
	{

		#region Constant

		private static readonly Type SERIALIZE_FIELD_ATTRIBUTE = typeof(SerializeField);
		private static readonly Type HIDE_IN_CSV_ATTRIBUTE = typeof(HideInScriptableCSVAttribute);
		private static readonly Type NON_SERIALIZED_ATTRIBUTE = typeof(HideInScriptableCSVAttribute);

		#endregion

		#region Variables

		public bool initialized;
		public bool isPendingReload;    // Keeps track of whether we need to reload all of our scriptable object data

		// Object GameData
		public Type type;
		public ScriptableCSVAttribute attributeData;

		// Tree GameData
		public ScriptableCSVCellView treeView;
		public ScriptableCSVState viewState;
		public ScriptableCSVHeaderState headerView;

		// Other GUI
		public SearchField searchField;

		#endregion

		#region Generate Tree

		public void InitializeTreeInfo()
		{
			// If we are not yet initialized, simply return
			if (initialized)
				return;

			// Initialize our state if it does not already exist. (Deserialized from window layout file or scriptable object)
			if (viewState == null)
				viewState = new ScriptableCSVState();

			// Generate our Attribute data for all of our field
			List<ScriptableCSVFieldData> fieldColumnData = GetCSVFieldInfo();

			// If at this point our list is empty, we return to avoid throwing any errors when constructing our tree
			if (fieldColumnData.Count <= 0)
				return;

			// According to 'https://stackoverflow.com/questions/9062235/get-properties-in-order-of-declaration-using-reflection'
			// This should never be done. But hey, its for an editor meme, so it should be fine
			fieldColumnData = fieldColumnData.OrderBy(item => item.fieldInfo.MetadataToken).ToList();

			// Do some funky stuff with our header state. Don't actually understand why we do this tbh
			bool firstInit = headerView == null;
			ScriptableCSVHeaderState headerState = new ScriptableCSVHeaderState(fieldColumnData);
			if (MultiColumnHeaderState.CanOverwriteSerializedFields(headerView, headerState))
				MultiColumnHeaderState.OverwriteSerializedFields(headerView, headerState);
			headerView = headerState;

			// Initialize our header
			var multiColumnHeader = new ScriptableCSVMutliColumnHeader(headerState);
			if (firstInit)
				multiColumnHeader.ResizeToFit();

			// We separate all the other data, and return just the field info, since all the column information is no longer needed
			List<FieldInfo> columnFieldInfos = fieldColumnData.Select(item => item.fieldInfo).ToList();

			// Generate our tree model and our tree view
			var treeModel = GetTreeViewModel();
			treeView = new ScriptableCSVCellView(columnFieldInfos, viewState, multiColumnHeader, treeModel);

			// Initialize our Search field last
			searchField = new SearchField();
			searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;

			initialized = true;
			isPendingReload = false;
		}

		private TableViewModel<ScriptableCSVCellItem> GetTreeViewModel()
		{
			// For the moment we only are going to pull our scriptable objects from Resources

			// Then return our data
			return new TableViewModel<ScriptableCSVCellItem>(GetAllAssetInstances());
		}
		private List<ScriptableCSVCellItem> GetAllAssetInstances()
		{
			List<ScriptableCSVCellItem> outData = new List<ScriptableCSVCellItem>();
			var scriptableObjectData = Resources.LoadAll("", type);

			// Add our null root Element
			outData.Add(new ScriptableCSVCellItem("Root", -1, 0));

			// Then Go through and add all of our new elements
			for (int i = 0; i < scriptableObjectData.Length; i++) {
				outData.Add(new ScriptableCSVCellItem(scriptableObjectData[i] as ScriptableObject, 0, i + 1));
			}

			return outData;
		}

		private List<ScriptableCSVFieldData> GetCSVFieldInfo()
		{
			List<ScriptableCSVFieldData> outData = new List<ScriptableCSVFieldData>();

			// Use reflection to find all of the fields in our class which have our attribute on them
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			// First we get a list of all field info in our class with our attribute
			foreach (FieldInfo fieldInfo in type.GetFields(flags)) {

				bool isSystemSerialized = !fieldInfo.IsNotSerialized;
				bool isUnitySerialized = !fieldInfo.IsPrivate || Attribute.IsDefined(fieldInfo, SERIALIZE_FIELD_ATTRIBUTE);
				bool hasNoHideAttribute = !Attribute.IsDefined(fieldInfo, HIDE_IN_CSV_ATTRIBUTE);

				// Check if we want to hide the field or we are not serialized
				if (isSystemSerialized && isUnitySerialized && hasNoHideAttribute)
					outData.Add(new ScriptableCSVFieldData(fieldInfo));
			}

			return outData;
		}

		#endregion

		#region OnGUI

		public void OnGUI(Rect position)
		{
			// Initialize our tree if required
			if (!initialized)
				InitializeTreeInfo();

			if (isPendingReload)
				ForceRefreshTreeModelAssetData();

			// Finally check if we have an initialized tree view at all
			if (initialized) {

				// Generate our Rects
				Rect toolbarRect = new Rect(position.xMin + 20, position.y + 10, position.width - 40f, 20f);
				Rect treeViewRect = new Rect(position.x + 20, toolbarRect.yMax + EditorGUIUtility.standardVerticalSpacing, position.width - 40, position.height - 60);
				Rect footerRect = new Rect(treeViewRect.x, treeViewRect.yMax + EditorGUIUtility.standardVerticalSpacing-1, treeViewRect.width, EditorGUIUtility.singleLineHeight);

				// Then We Generate our Searchfield
				treeView.searchString = searchField.OnGUI(toolbarRect, treeView.searchString);
				treeView.OnGUI(treeViewRect);

				// Draw our footer
				if (attributeData.showAddRemove)
					DrawFooter(footerRect);

			} else {
				string errorMessage = string.Format("Unable to display Spreadsheet for Type '{0}', We have no serializable variables to show...", type.Name);

				GUIStyle centeredErrorStyle = new GUIStyle(EditorStyles.boldLabel);
				centeredErrorStyle.alignment = TextAnchor.MiddleCenter;
				centeredErrorStyle.wordWrap = true;
				centeredErrorStyle.padding = new RectOffset(30, 30, 0, 0);

				EditorGUI.LabelField(position, new GUIContent(errorMessage), centeredErrorStyle);
			}
		}

		private void DrawFooter(Rect rect)
		{
			// Create our Rects
			float rightEdge = rect.xMax;
			float leftEdge = rightEdge - 58;
			rect = new Rect(leftEdge, rect.y, rightEdge - leftEdge, rect.height);
			Rect addRect = new Rect(leftEdge + 4, rect.y, 25, 16);
			Rect removeRect = new Rect(rightEdge - 29, rect.y, 25, 16);


			// Draw our Background
			GUIStyle footerBackground = "RL Footer";
			if (Event.current.type == EventType.Repaint) {
				footerBackground.Draw(rect, false, false, false, false);
			}

			GUIStyle preButton = "RL FooterButton";
			if (GUI.Button(addRect, EditorGUIUtility.TrIconContent("Toolbar Plus", "Create new"), preButton)) {

				string path = EditorUtility.SaveFilePanelInProject($"Save new {type.Name}", $"{type.Name}.asset", "asset", "Please select valid path to save new asset to");

				if (path.Length != 0) {
					ScriptableObject newAsset = ScriptableObject.CreateInstance(type);
					AssetDatabase.CreateAsset(newAsset, path);
					AssetDatabase.SaveAssets();

				}
			}
			if (GUI.Button(removeRect, EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection"), preButton)) {

				List<int> selectedIndicies = treeView.GetSelection().ToList();
				if (selectedIndicies.Count > 0) {

					if (EditorUtility.DisplayDialog("Confirm Delete", $"Are you sure you want to delete '{selectedIndicies.Count}' items?\nThis cannot be undone.", "Yes, I'm sure", "Actually nah")) {

						for (int i = 0; i < selectedIndicies.Count; i++) {
							ScriptableCSVCellItem cellItem = treeView.treeModel.Find(selectedIndicies[i]);
							if (cellItem != null && cellItem.scriptableObject != null) {
								string assetPath = AssetDatabase.GetAssetPath(cellItem.scriptableObject);
								if (assetPath.Length > 0)
									AssetDatabase.DeleteAsset(assetPath);
							}
						}

						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();

					}
				}
			}

		}

		#endregion

		#region Set

		public void RefreshTreeModelAssetData()
		{
			isPendingReload = true;
		}
		private void ForceRefreshTreeModelAssetData()
		{
			// If we are initialized, set our new data, getting all possible new asset instances. Then force a reload
			if (initialized) {
				treeView.treeModel.SetData(GetAllAssetInstances());
				treeView.Reload();
			}

			isPendingReload = false;
		}


		#endregion

		#region Utility

		public string GetDisplayName()
		{
			// If we have no display name, return our full type name, Otherwise display our display name provided
			if (string.IsNullOrWhiteSpace(attributeData.displayName))
				return type.Name;
			else
				return attributeData.displayName;
		}

		#endregion

	}

}