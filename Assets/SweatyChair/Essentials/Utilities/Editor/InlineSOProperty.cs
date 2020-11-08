using SweatyChair;
using System;
using UnityEditor;
using UnityEngine;

public class InlineSOProperty
{

	#region Const

	private static readonly Type INSPECTOR_GUI_TYPE = typeof(InspectorWithoutScriptFieldDrawer);

	#endregion

	#region Variables

	// Settings
	public bool showCreateButton { get; set; }
	public GUIContent createButtonContent { get; set; } = new GUIContent("Create");

	// Cached data
	private Editor _scriptableObjectEditor;
	public SerializedProperty scriptableObjectProp { get; private set; }

	// Properties
	private bool isValidPropertyType => scriptableObjectProp != null && scriptableObjectProp.propertyType == SerializedPropertyType.ObjectReference || scriptableObjectProp.propertyType == SerializedPropertyType.ExposedReference;


	#endregion

	#region Constructor / Reset

	public InlineSOProperty(SerializedProperty customProperty, bool showCreateButton = true)
	{
		// Set our core data
		scriptableObjectProp = customProperty;

		// Set our settings
		this.showCreateButton = showCreateButton;
	}

	public void Reset()
	{
		// Then null our editor, so we force get our brand new data
		if (_scriptableObjectEditor != null)
			UnityEngine.Object.DestroyImmediate(_scriptableObjectEditor);

		_scriptableObjectEditor = null;
	}

	#endregion

	#region OnGUI

	public void DrawGUI()
	{
		if (isValidPropertyType)
			DrawValidPropertyDrawer();
		else
			DrawInvalidPropertyDrawer();
	}

	private void DrawInvalidPropertyDrawer()
	{
		EditorGUILayout.HelpBox("Unable to display a custom Drawer for our current Type. This drawer only works for Scriptable Object Types", MessageType.Error, true);
	}

	private void DrawValidPropertyDrawer()
	{
		// This will always be a frame late, since we want a specific layout. Which shouldn't be a problem
		// Then attempt to re-generate our editor if any data has changed
		Editor.CreateCachedEditor(scriptableObjectProp.objectReferenceValue, typeof(InspectorWithoutScriptFieldDrawer), ref _scriptableObjectEditor);

		// Create a horizontal scope for our scriptable object field
		using (new EditorGUILayout.HorizontalScope()) {

			Rect foldoutRect = GUILayoutUtility.GetRect(GetPrefixWidth(), EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(false));
			GUIContent propertyLabel = EditorGUIUtility.TrTempContent(scriptableObjectProp.displayName);
			using (new CustomEditorScopes.HierarchyModeScope(false)) {
				if (_scriptableObjectEditor != null)
					scriptableObjectProp.isExpanded = EditorGUI.Foldout(foldoutRect, scriptableObjectProp.isExpanded, propertyLabel, true);
				else
					EditorGUI.LabelField(foldoutRect, propertyLabel);
			}

			// Draw our field
			using (new EditorGUILayout.HorizontalScope()) {

				EditorGUILayout.PropertyField(scriptableObjectProp, GUIContent.none, false);
				if (showCreateButton && GUILayout.Button(createButtonContent, GUILayout.Width(EditorStyles.label.CalcSize(createButtonContent).x + 10)))
					OnCreateButtonClicked();
			}
		}

		// Then Display our Editor GUI
		using (new EditorGUI.IndentLevelScope()) {
			if (_scriptableObjectEditor != null && scriptableObjectProp.isExpanded) {

				// Draw a box to define what is scriptable object or not
				using (new EditorGUILayout.VerticalScope(new GUIStyle("HelpBox")))
					_scriptableObjectEditor.OnInspectorGUI();
			}
		}
	}

	#endregion

	#region Callbacks

	private void OnCreateButtonClicked()
	{
		SweatyChair.EditorReflection.ScriptAttributeUtility.GetFieldInfoFromProperty(scriptableObjectProp, out Type scriptableType);

		// Bring up a save prompt for a new object
		string path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", scriptableType.Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", "Assets");
		if (path == "") return;

		ScriptableObject asset = ScriptableObject.CreateInstance(scriptableType);
		AssetDatabase.CreateAsset(asset, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		EditorGUIUtility.PingObject(asset);

		scriptableObjectProp.objectReferenceValue = asset;
	}

	#endregion

	#region Utility

	private float GetPrefixWidth()
	{
		return EditorGUIUtility.labelWidth - (EditorGUI.indentLevel * 15) - 4;
	}

	#endregion

}

