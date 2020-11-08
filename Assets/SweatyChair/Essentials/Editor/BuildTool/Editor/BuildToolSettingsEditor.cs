using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SweatyChair
{

	[CustomEditor(typeof(BuildToolSettings))]
	public class BuildToolSettingsEditor : Editor
	{

		private bool _confirmDelete = false;

		private BuildToolSettings _bts => target as BuildToolSettings;

		private int _cachedCurIndex = -1;

		private int _curIndex {
			get {
				if (_cachedCurIndex == -1)
					_cachedCurIndex = EditorPrefs.GetInt("BuildToolSettingsEditorBuildInfoIndex");
				_cachedCurIndex = Mathf.Clamp(0, _cachedCurIndex, _bts.buildInfos.Length - 1); // Make sure it's positive
				return _cachedCurIndex;
			}
			set {
				_cachedCurIndex = value;
				EditorPrefs.SetInt("BuildToolSettingsEditorBuildInfoIndex", value);
			}
		}

		private BuildToolBuildInfo _curBuildInfo => _bts.buildInfos[_curIndex];

		public override void OnInspectorGUI()
		{
			if (_confirmDelete) {

				// Show the confirmation dialog
				GUILayout.Label(string.Format("Sure to delete build info '{0}'?", _curBuildInfo.name));
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				GUILayout.BeginHorizontal();
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button("Cancel"))
						_confirmDelete = false;

					GUI.backgroundColor = Color.red;
					if (GUILayout.Button("Delete")) {
						DeleteBuildInfo();
						_confirmDelete = false;
					}
					GUI.backgroundColor = Color.white;
				}
				GUILayout.EndHorizontal();

			} else {

				OnBuildInfosGUI();

				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				OnManagerGUI();
			}
		}

		private void OnBuildInfosGUI()
		{
			// New
			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("New Build Info")) {
				AddBuildInfo();
			}
			GUI.backgroundColor = Color.white;

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			// Validation
			if (_bts.buildInfos == null || _bts.buildInfos.Length == 0)
				_bts.buildInfos = new BuildToolBuildInfo[1];
			if (_curIndex >= _bts.buildInfos.Length)
				_curIndex = _bts.buildInfos.Length - 1;

			// Navigation
			GUILayout.BeginHorizontal();
			{
				bool canGoPrev = true;
				if (_curIndex <= 0) {
					GUI.color = Color.grey;
					canGoPrev = false;
				}
				if (GUILayout.Button("<<")) {
					if (canGoPrev) {
						_confirmDelete = false;
						_curIndex--;
					}
				}
				GUI.color = Color.white;

				_curIndex = EditorGUILayout.IntField(_curIndex + 1, GUILayout.Width(40)) - 1;
				GUILayout.Label("/ " + _bts.buildInfos.Length, GUILayout.Width(40));

				bool canGoNext = true;
				if (_curIndex >= _bts.buildInfos.Length - 1) {
					GUI.color = Color.grey;
					canGoNext = false;
				}
				if (GUILayout.Button(">>")) {
					if (canGoNext) {
						_confirmDelete = false;
						_curIndex++;
					}
				}
				GUI.color = Color.white;
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			// Build info name
			string name = EditorGUILayout.TextField(new GUIContent("Name", "Name of the build info, used for reference only."), _curBuildInfo.name);
			if (name != _curBuildInfo.name) {
				Undo.RegisterCompleteObjectUndo(_bts, "Build Info Name Change");
				_bts.buildInfos[_curIndex].name = name;
				EditorUtility.SetDirty(_bts);
			}

			// Build info product name
			if (_bts.setProductName) {
				string productName = EditorGUILayout.TextField(new GUIContent("Product Name", "Product Name of the build info"), _curBuildInfo.productName);
				if (productName != _curBuildInfo.productName) {
					Undo.RegisterCompleteObjectUndo(_bts, "Build Info Product Name Change");
					_bts.buildInfos[_curIndex].productName = productName;
					EditorUtility.SetDirty(_bts);
				}
			}

			// Build info bundle ID
			if (_bts.setBundleId) {
				string bundleId = EditorGUILayout.TextField(new GUIContent("Bundle ID", "Bundle ID of the build info"), _curBuildInfo.bundleId);
				if (bundleId != _curBuildInfo.bundleId) {
					Undo.RegisterCompleteObjectUndo(_bts, "Build Info Bundle ID Change");
					_bts.buildInfos[_curIndex].bundleId = bundleId;
					EditorUtility.SetDirty(_bts);
				}
			}

			// Build info define symbols
			if (_bts.setDefineSymbols) {
				string defineSymbols = EditorGUILayout.TextField(new GUIContent("Define Symbols", "Define symbols of the build info"), _curBuildInfo.defineSymbols);
				if (defineSymbols != _curBuildInfo.defineSymbols) {
					Undo.RegisterCompleteObjectUndo(_bts, "Build Info Define Symbols Change");
					_bts.buildInfos[_curIndex].defineSymbols = defineSymbols;
					EditorUtility.SetDirty(_bts);
				}
			}

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			if (GUILayout.Button("Set To This"))
				BuildTool.ChangeToBuild(_curBuildInfo);

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			// Delete Button
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("Delete"))
				_confirmDelete = true;
			GUI.backgroundColor = Color.white;
		}

		private void AddBuildInfo()
		{
			List<BuildToolBuildInfo> buildInfoList = new List<BuildToolBuildInfo>(_bts.buildInfos);
			BuildToolBuildInfo newBuildInfo = new BuildToolBuildInfo {
				name = "New Build"
			};
			buildInfoList.Add(newBuildInfo);
			_bts.buildInfos = buildInfoList.ToArray();
			_curIndex = buildInfoList.Count - 1;
		}

		private void DeleteBuildInfo()
		{
			List<BuildToolBuildInfo> buildInfoList = new List<BuildToolBuildInfo>(_bts.buildInfos);
			buildInfoList.RemoveAt(_curIndex);
			_bts.buildInfos = buildInfoList.ToArray();
			if (_curIndex >= _bts.buildInfos.Length)
				_curIndex = _bts.buildInfos.Length - 1;
		}

		private void OnManagerGUI()
		{
			bool setProductName = EditorGUILayout.Toggle(new GUIContent("Set Product Name", "Set all product names in bundle infos"), _bts.setProductName);
			if (setProductName != _bts.setProductName) {
				Undo.RegisterCompleteObjectUndo(_bts, "Set Product Name Change");
				_bts.setProductName = setProductName;
				EditorUtility.SetDirty(_bts);
			}

			bool setBundleId = EditorGUILayout.Toggle(new GUIContent("Set Bundle ID", "Set all bundle IDs in bundle infos"), _bts.setBundleId);
			if (setBundleId != _bts.setBundleId) {
				Undo.RegisterCompleteObjectUndo(_bts, "Set Bundle ID Change");
				_bts.setBundleId = setBundleId;
				EditorUtility.SetDirty(_bts);
			}

			bool setDefineSymbols = EditorGUILayout.Toggle(new GUIContent("Set Define Symbols", "Set all define symbols in bundle infos"), _bts.setDefineSymbols);
			if (setDefineSymbols != _bts.setDefineSymbols) {
				Undo.RegisterCompleteObjectUndo(_bts, "Set Define Symbols Change");
				_bts.setDefineSymbols = setDefineSymbols;
				EditorUtility.SetDirty(_bts);
			}

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			string keystorePassword = EditorGUILayout.TextField(new GUIContent("Keystore Password", "The password of the keystore"), _bts.keystorePassword);
			if (keystorePassword != _bts.keystorePassword) {
				Undo.RegisterCompleteObjectUndo(_bts, "Keystore Password Change");
				_bts.keystorePassword = keystorePassword;
				EditorUtility.SetDirty(_bts);
			}

			string keyAliasPassword = EditorGUILayout.TextField(new GUIContent("Key Alias Password", "The password of the key alias"), _bts.keyAliasPassword);
			if (keyAliasPassword != _bts.keyAliasPassword) {
				Undo.RegisterCompleteObjectUndo(_bts, "Key Alias Change");
				_bts.keyAliasPassword = keyAliasPassword;
				EditorUtility.SetDirty(_bts);
			}

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Build Now"))
				BuildTool.Build();
			GUI.backgroundColor = Color.white;
		}

	}

}