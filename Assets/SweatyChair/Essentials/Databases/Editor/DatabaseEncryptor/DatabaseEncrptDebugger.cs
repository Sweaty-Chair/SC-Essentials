//
// DatabaseEncryptor.cs
//
// Encrypt all raw database to .bytes in Resouces to be used in game
//
// by Richard Fu

using UnityEngine;
using UnityEditor;
using SweatyChair;

public class DatabaseEncrptDebugger : EditorWindow
{

	TextAsset txtAsset;
	string text = "Nothing Opened...";
	Vector2 scrollPos;
	
	void OnGUI ()
	{
		GUILayout.BeginHorizontal ();
		TextAsset newTxtAsset = EditorGUILayout.ObjectField ("Encrypted CSV", txtAsset, typeof (TextAsset), false) as TextAsset;
		if (GUILayout.Button ("Refresh"))
			txtAsset = null;
		GUILayout.EndHorizontal ();

		if (txtAsset != newTxtAsset)
			ReadTextAsset (newTxtAsset);

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);		
		text = EditorGUILayout.TextArea (text, GUILayout.Height (position.height - 30));		
		EditorGUILayout.EndScrollView();
	}

	void ReadTextAsset (TextAsset txt)
	{
		text = SimpleEncryptor.Decrypt (txt.bytes);
		txtAsset = txt;
	}

	[MenuItem("Tools/Databases Encryptor/Debugger", false, 10000)]
	public static void Init ()
	{
		DatabaseEncrptDebugger thisWindow = GetWindow (typeof(DatabaseEncrptDebugger)) as DatabaseEncrptDebugger;
		thisWindow.titleContent = new GUIContent ("Databases Encrypt Debugger");
		thisWindow.Show ();
	}

}