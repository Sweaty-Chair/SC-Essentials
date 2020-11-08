using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using SweatyChair;

// Encrypt all raw database to .bytes in Resouces to be used in game

public class DatabaseEncryptor : EditorWindow
{

	public const string DIR_DATABASES_RAW = "DatabasesRaw";
	public const string DIR_ASSETS = "Assets";
	public const string DIR_RESOURCES = "Resources";

	private const string PREF_DATABASE_SELECTED = "DatabaseEncryptor.DatabaseSelected"; // +index

	// Databases
	private bool dbSelectAll = false;
	private List<string> dbFiles = new List<string>();
	private Dictionary<string, bool> dbFilesSelected = new Dictionary<string, bool>();
	private Vector2 dbScrollPosition = Vector2.zero;

	private static string resourceProjectPath {
		get { return Path.Combine(DIR_ASSETS, DIR_RESOURCES); }
	}

	private static string resourceFullPath {
		get { return Path.Combine(Application.dataPath, DIR_RESOURCES); }
	}

	public static string dbRawProjectPath {
		get { return Path.Combine(resourceProjectPath, DIR_DATABASES_RAW); }
	}

	private static string dbRawFullPath {
		get { return Path.Combine(resourceFullPath, DIR_DATABASES_RAW); }
	}

	public static string dbEncryptedProjectPath {
		get { return resourceProjectPath; }
	}

	private static string dbEncryptedFullPath {
		get { return resourceFullPath; }
	}

	void OnEnable()
	{
		Reset();
		string[] assetsInFolder = Directory.GetFiles(dbRawFullPath);
		int i = 0;
		foreach (string asset in assetsInFolder) {
			string filename = Path.GetFileName(asset);
			if (asset.EndsWith(".csv") || asset.EndsWith(".txt")) { // Only process .csv and .txt dbFiles
				dbFiles.Add(filename);
				dbFilesSelected.Add(filename, EditorPrefs.GetBool(PREF_DATABASE_SELECTED + (i++), true));
			}
		}
	}

	void OnGUI()
	{
		GUILayout.Label("Raw Databases Folder", EditorStyles.label);
		GUILayout.BeginHorizontal();
		GUILayout.Label(dbRawFullPath, EditorStyles.label);
		if (GUILayout.Button("Open"))
			EditorUtility.RevealInFinder(resourceFullPath);
		GUILayout.EndHorizontal();

		GUILayout.Label("Encrypted Databases Folder", EditorStyles.label);
		GUILayout.BeginHorizontal();
		GUILayout.Label(dbEncryptedFullPath, EditorStyles.label);
		if (GUILayout.Button("Open"))
			EditorUtility.RevealInFinder(dbEncryptedFullPath);
		GUILayout.EndHorizontal();

		GUILayout.Space(20);

		GUILayout.BeginHorizontal();
		int dbTotalSelected = 0;
		foreach (KeyValuePair<string, bool> kvp in dbFilesSelected) {
			if (kvp.Value)
				dbTotalSelected++;
		}
		GUILayout.Label(string.Format("{0} selected", dbTotalSelected), EditorStyles.label);
		if (GUILayout.Button("Select " + (dbSelectAll ? "all" : "none"))) {
			List<string> keys = new List<string>(dbFilesSelected.Keys);
			for (int i = 0, imax = keys.Count; i < imax; i++) {
				string key = keys[i];
				dbFilesSelected[key] = dbSelectAll;
				EditorPrefs.SetBool(PREF_DATABASE_SELECTED + i, dbSelectAll);
			}
			dbSelectAll = !dbSelectAll;
		}
		GUILayout.EndHorizontal();

		dbScrollPosition = EditorGUILayout.BeginScrollView(dbScrollPosition);
		for (int i = 0, imax = dbFiles.Count; i < imax; i++) {
			string filename = dbFiles[i];
			GUILayout.BeginHorizontal();
			bool selected = EditorGUILayout.Toggle(dbFilesSelected[filename], GUILayout.Width(30));
			if (selected != dbFilesSelected[filename]) {
				dbFilesSelected[filename] = selected;
				EditorPrefs.SetBool(PREF_DATABASE_SELECTED + i, dbFilesSelected[filename]);
				Debug.Log(i + "|" + EditorPrefs.GetBool(PREF_DATABASE_SELECTED + i));
			}
			if (GUILayout.Button(filename)) {
				dbFilesSelected[filename] = !dbFilesSelected[filename];
				EditorPrefs.SetBool(PREF_DATABASE_SELECTED + i, dbFilesSelected[filename]);
				Debug.Log(i + "|" + EditorPrefs.GetBool(PREF_DATABASE_SELECTED + i));
			}
			GUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();

		if (GUILayout.Button("Encrypt selected raw databases"))
			EncryptSelectedDatabases();
	}

	private void EncryptSelectedDatabases()
	{
		int numEncrpted = 0;
		foreach (KeyValuePair<string, bool> kvp in dbFilesSelected) {
			if (!kvp.Value)
				continue;

			string fromProjPath = Path.Combine(dbRawProjectPath, kvp.Key);
			string toProjPath = Path.Combine(dbEncryptedProjectPath, kvp.Key.Replace(".csv", ".bytes").Replace(".txt", ".bytes"));
			if (EncryptFile(fromProjPath, toProjPath))
				numEncrpted++;
		}
		Debug.LogFormat("{0} databases has successfully encrypted.", numEncrpted);
	}

	// Full path in project includes the extension, e.g. "Assets/DatabasesRaw/Patterns/city.csv"
	public static bool EncryptFile(string fromProjPath, string toProjPath)
	{
		TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(fromProjPath);
		if (ta == null) {
			Debug.LogErrorFormat("There's no csv file at: {0}. Make sure it's in project path.", fromProjPath);
			return false;
		}
		string toFullPath = Path.Combine(Application.dataPath, toProjPath.Replace(DIR_ASSETS + "/", ""));
		return EncryptFile(ta.bytes, toFullPath);
	}

	public static bool EncryptFileFromString(string str, string toFullPath)
	{
		return EncryptFile(System.Text.Encoding.UTF8.GetBytes(str), toFullPath);
	}

	public static bool EncryptFile(byte[] bytes, string toFullPath)
	{
//		Debug.LogFormat("DatabaseEncryptor:EncryptFile({0},{1})", bytes.Length, toFullPath);
		if (!toFullPath.Contains(".bytes")) {
			Debug.LogError("Please put .bytes as extension in output path.");
			return false;
		}
		FileInfo fi = new FileInfo(toFullPath);
		if (File.Exists(toFullPath) && fi.IsReadOnly) {
			Debug.LogError("File \"" + toFullPath + "\" not writable! Make sure you checkout in Perforce.");
			return false;
		}
		fi.Directory.Create(); // Create .bytes file if not exists

		File.WriteAllBytes(toFullPath, SimpleEncryptor.Encrypt(bytes));
		return true;
	}

	private void Reset()
	{
		dbSelectAll = false;
		dbFiles.Clear();
		dbFilesSelected.Clear();
		dbScrollPosition = Vector2.zero;
	}

	[MenuItem("Tools/Databases Encryptor/Open", false, 1)]
	public static void ShowWindow()
	{
		DatabaseEncryptor thisWindow = (DatabaseEncryptor)EditorWindow.GetWindow(typeof(DatabaseEncryptor));
		thisWindow.titleContent = new GUIContent("Databases Encryptor");
	}

}