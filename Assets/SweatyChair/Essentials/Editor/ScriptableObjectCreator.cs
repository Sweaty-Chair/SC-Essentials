using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectCreator
{

	[MenuItem("Assets/Create/Instance")]
	public static void CreateInstance()
	{
		foreach (Object o in Selection.objects) {
			if (o is MonoScript) {
				MonoScript script = (MonoScript)o;
				System.Type type = script.GetClass();
				if (type != null && type.IsSubclassOf(typeof(ScriptableObject)))
					CreateAsset(type);
			}
		}
	}

	[MenuItem("Assets/Create/Instance", true)]
	public static bool ValidateCreateInstance()
	{
		foreach (Object o in Selection.objects) {
			if (o is MonoScript) {
				MonoScript script = (MonoScript)o;
				System.Type type = script.GetClass();
				if (type != null && type.IsSubclassOf(typeof(ScriptableObject)))
					return true;
			}
		}
		return false;
	}

	private static void CreateAsset(System.Type type)
	{
		var asset = ScriptableObject.CreateInstance(type);
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (path == "")
			path = "Assets";
		else if (Path.GetExtension(path) != "")
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + type.ToString() + ".asset");
		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

}