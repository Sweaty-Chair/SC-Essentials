using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using SweatyChair;

public class PackageExporter : EditorWindow
{

	private bool _selectAll = true;
	private bool _toggleAll = true;
	private Dictionary<string, bool> _selectedDict = new Dictionary<string, bool>();
	private string _exportPath;
	private int _pluginCount;

	[MenuItem("Sweaty Chair/Package Exporter", false, 501)]
	private static void Init()
	{
		PackageExporter window = GetWindow<PackageExporter>();
		window.Show();
	}

	private void OnEnable()
	{
		_exportPath = EditorPrefs.GetString("PackageExporterPath", "/Users/fur/Google Drive/SweatyChair/Unity Plugins/@SweatyChair/");
		Refresh();
	}

	private void OnGUI()
	{
		string exportPath = EditorGUILayout.TextField("Export Path", _exportPath);
		if (exportPath != _exportPath) {
			_exportPath = exportPath;
			Refresh();
			EditorPrefs.SetString("PackageExporterPath", _exportPath);
		}

		_selectAll = EditorGUILayout.Toggle("Select All", _selectAll);

		if (!_selectAll) {

			EditorGUILayout.Space();
			
			string pathSweatyChair = Path.Combine(Application.dataPath, "SweatyChair");

			bool toggleAll = EditorGUILayout.Toggle("  ", _toggleAll);
			if (toggleAll != _toggleAll) {
				foreach (string subFolderPath in Directory.GetDirectories(pathSweatyChair)) {
					string subFolderName = Path.GetFileName(subFolderPath);
					_selectedDict[subFolderName] = toggleAll;
				}
				_toggleAll = toggleAll;
			}

			foreach (string subFolderPath in Directory.GetDirectories(pathSweatyChair)) {
				string subFolderName = Path.GetFileName(subFolderPath);
				bool selected = _selectedDict.ContainsKey(subFolderName) && _selectedDict[subFolderName];
				_selectedDict[subFolderName] = EditorGUILayout.Toggle(subFolderName, selected);
			}

			int selectedCount = 0;
			foreach (bool b in _selectedDict.Values) {
				if (b)
					selectedCount++;
			}
				
			EditorGUILayout.LabelField("Selected Plugins: " + selectedCount);

			EditorGUILayout.Space();

		} else {
			
			EditorGUILayout.LabelField("Total Plugins: " + _pluginCount);
		}

		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Export", GUILayout.Width(100))) {
				ExportPackages();
			}

			if (GUILayout.Button("Open Folder", GUILayout.Width(100))) {
				EditorUtility.RevealInFinder(_exportPath);
			}

			if (GUILayout.Button("Refresh", GUILayout.Width(100))) {
				Refresh();
			}
		}
	}

	private void ExportPackages()
	{
		string pathSweatyChair = Path.Combine(Application.dataPath, "SweatyChair");

		foreach (string subFolderPath in Directory.GetDirectories(pathSweatyChair)) {

			string subFolderName = Path.GetFileName(subFolderPath);

			if (!_selectAll && !_selectedDict[subFolderName])
				continue;
			
			Debug.LogFormat("Exporting {0}...", subFolderName);

			string exportFolderPath = Path.Combine(_exportPath, subFolderName);
			string projectNameSuffix = "_" + Application.productName.TrimSpaces().ToLower() + ".unitypackage";

			// Delete the old packages
			string[] oldFilePaths = Directory.GetFiles(exportFolderPath, "*" + projectNameSuffix, SearchOption.TopDirectoryOnly);
			foreach (var oldFilePath in oldFilePaths) {
				File.Delete(oldFilePath);
			}

			string exportPath = exportFolderPath + "/" + System.DateTime.Now.ToString("yyMMdd") + projectNameSuffix;

			// Delete the old package

			if (!Directory.Exists(exportFolderPath))
				Directory.CreateDirectory(exportFolderPath);
			if (File.Exists(exportPath))
				File.Delete(exportPath);

			AssetDatabase.ExportPackage("Assets/SweatyChair/" + subFolderName, exportPath, ExportPackageOptions.Recurse);

		}

		if (_selectAll) {
			Debug.LogFormat("All {0} plugins are exported successfully.", _pluginCount);
		} else {
			int selectedCount = 0;
			foreach (bool b in _selectedDict.Values) {
				if (b)
					selectedCount++;
			}
			Debug.LogFormat("{0} plugins are exported successfully.", selectedCount);
		}
	}

	private void Refresh()
	{
		_pluginCount = FileUtils.GetDirectoryCount(Path.Combine(Application.dataPath, "SweatyChair"));
		_selectedDict = new Dictionary<string, bool>();
		string pathSweatyChair = Path.Combine(Application.dataPath, "SweatyChair");
		foreach (string subFolderPath in Directory.GetDirectories(pathSweatyChair)) {
			string subFolderName = Path.GetFileName(subFolderPath);
			_selectedDict[subFolderName] = true;
		}
	}

}