using SweatyChair;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class EditorUtils
{

	#region Consts

	public const float INDENT_SIZE = 15f;
	public const float LINE_SPACE_SIZE = 3f;
	public static float FIELD_PADDING => EditorGUIUtility.standardVerticalSpacing;

	#endregion

	/// <summary>
	/// Writes code into the script at Asset path. Warning: everything will be overwritten.
	/// </summary>
	public static void WriteScript(string filePath, string code, UnityAction onComplete)
	{
		bool success = false;

		FileUtils.CreateDirectory(filePath);
		FileUtils.UnsetReadOnly(filePath);
		using (StreamWriter writer = new StreamWriter(filePath, false)) {
			try {
				writer.WriteLine("{0}", code);
				success = true;
			}
			catch (System.Exception ex) {
				string msg = " \n" + ex.ToString();
				Debug.LogError(msg);
				EditorUtility.DisplayDialog("EditorUtils:WriteScript - Error when trying to regenerate file at path=" + filePath, msg, "OK");
			}
		}

		if (success) {
			AssetDatabase.Refresh();
			onComplete?.Invoke();
		}
	}

	#region Get Base Object

	/// <summary>
	/// Gets the object the property represents.
	/// </summary>
	/// <param name="prop"></param>
	/// <returns></returns>
	public static object GetTargetObjectOfProperty(SerializedProperty prop)
	{
		if (prop == null)
			return null;

		var path = prop.propertyPath.Replace(".Array.data[", "[");
		object obj = prop.serializedObject.targetObject;
		var elements = path.Split('.');
		foreach (var element in elements) {
			if (element.Contains("[")) {
				var elementName = element.Substring(0, element.IndexOf("["));
				var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
				obj = GetValue_Imp(obj, elementName, index);
			} else {
				obj = GetValue_Imp(obj, element);
			}
		}
		return obj;
	}

	private static object GetValue_Imp(object source, string name, int index)
	{
		var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
		if (enumerable == null)
			return null;
		var enm = enumerable.GetEnumerator();
		//while (index-- >= 0)
		//    enm.MoveNext();
		//return enm.Current;

		for (int i = 0; i <= index; i++) {
			if (!enm.MoveNext())
				return null;
		}
		return enm.Current;
	}

	private static object GetValue_Imp(object source, string name)
	{
		if (source == null)
			return null;
		var type = source.GetType();

		while (type != null) {
			var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (f != null)
				return f.GetValue(source);

			var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (p != null)
				return p.GetValue(source, null);

			type = type.BaseType;
		}
		return null;
	}

	#endregion

	#region OnGUI Utility

	#region Horizontal Line

	private static GUIStyle _horizontalLine;

	// utility method
	public static void HorizontalLine(Color color, int thickness = 2, int padding = 10, GUILayoutOption layoutOption = null)
	{
		if (_horizontalLine == null) {
			_horizontalLine = new GUIStyle();
			_horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
		}
		//Set our height and padding
		_horizontalLine.fixedHeight = thickness;
		_horizontalLine.margin = new RectOffset(0, 0, padding / 2, padding / 2);

		//Then colour and draw our UI
		var c = GUI.color;
		GUI.color = color;

		if (layoutOption != null) {
			GUILayout.Box(GUIContent.none, _horizontalLine, layoutOption);
		} else {
			GUILayout.Box(GUIContent.none, _horizontalLine);
		}
		GUI.color = c;
	}

	#endregion

	#region Script View

	public static void DrawScriptField<T>(Object target) where T : MonoBehaviour
	{
		GUI.enabled = false;
		EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((T)target), typeof(T), false);
		GUI.enabled = true;
	}

	public static void DrawScriptFieldSO<T>(Object target) where T : ScriptableObject
	{
		GUI.enabled = false;
		EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((T)target), typeof(T), false);
		GUI.enabled = true;
	}

	#endregion

	#region Draw Image Preview

	/// <summary>
	/// Begins a simple camera render. Make sure to end this call with <see cref="EndSimplePreviewRender(Rect)"/>.
	/// </summary>
	/// <param name="imageRect">The size of the Texture going to be output</param>
	/// <param name="cameraPos">The camera position in world space</param>
	/// <param name="rotation">The camera rotation in world space</param>
	/// <param name="nearClip">The near clipping plane of the camera</param>
	/// <param name="farClip">The far clipping plane of the camera</param>
	/// <returns></returns>
	public static PreviewRenderUtility BeginSimplePreviewRender(Rect imageRect, Vector3 cameraPos, Quaternion rotation, float nearClip = 0.5f, float farClip = 1000f)
	{
		PreviewRenderUtility previewRenderer = new PreviewRenderUtility();

		previewRenderer.camera.backgroundColor = Color.red;

		// Set our Camera GameData
		previewRenderer.camera.transform.position = cameraPos;
		previewRenderer.camera.transform.rotation = rotation;

		previewRenderer.camera.nearClipPlane = nearClip;
		previewRenderer.camera.farClipPlane = farClip;

		// Set our Lighting data
		previewRenderer.lights[0].type = LightType.Directional;
		previewRenderer.lights[0].intensity = 1f;
		previewRenderer.lights[0].transform.forward = -Vector3.forward;
		previewRenderer.lights[1].intensity = 0.5f;

		// Begin our Preview
		previewRenderer.BeginStaticPreview(imageRect);

		return previewRenderer;
	}

	/// <summary>
	/// Clears the specified Render to a specified colour. This overwrites all data added to scene previously.
	/// </summary>
	/// <param name="renderUtility"></param>
	/// <param name="color"></param>
	public static void ClearPreviewRenderToColour(PreviewRenderUtility renderUtility, Color color)
	{
		RenderTexture prevTex = renderUtility.camera.targetTexture;
		// Create a new texture with our colour, and blit it to our screen
		Texture2D backgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, true);
		backgroundTexture.SetPixel(0, 0, color);
		backgroundTexture.Apply();
		Graphics.DrawTexture(new Rect(0, 0, prevTex.width, prevTex.height), backgroundTexture);
		Object.DestroyImmediate(backgroundTexture);
	}

	/// <summary>
	/// Clears the specified Render to nothing. This overwrites all data added to scene previously.
	/// </summary>
	/// <param name="renderUtility"></param>
	/// <param name="color"></param>
	public static void ClearPreviewRenderGL(PreviewRenderUtility renderUtility, bool clearDepth, bool clearColour, Color color)
	{
		RenderTexture prevTex = renderUtility.camera.targetTexture;

		RenderTexture cachedActiveTexture = RenderTexture.active;
		RenderTexture.active = prevTex;

		GL.Clear(clearDepth, clearColour, color);

		RenderTexture.active = cachedActiveTexture;
	}

	/// <summary>
	/// Ends a simple camera render. Make sure to call this after <see cref="BeginSimplePreviewRender(Rect, Vector3, Quaternion, float, float)"/>.
	/// </summary>
	/// <param name="previewRenderer"></param>
	/// <returns></returns>
	public static Texture2D EndSimplePreviewRender(PreviewRenderUtility previewRenderer)
	{
		// Turn off scene fog
		bool fog = RenderSettings.fog;
		Unsupported.SetRenderSettingsUseFogNoDirty(false);

		// Force render our camera
		previewRenderer.Render();

		//Reset our fog settingsS
		Unsupported.SetRenderSettingsUseFogNoDirty(fog);

		// Get our Texture and Clean up our references. EndStaticPreview already does all the work of copying the texture into a texture that wont be cleared
		Texture2D outputTexture = previewRenderer.EndStaticPreview();
		outputTexture.name = "EndSimplePreviewRenderer_Texture";

		previewRenderer.Cleanup();

		return outputTexture;
	}

	/// <summary>
	/// Ends a simple camera render. Make sure to call this after <see cref="BeginSimplePreviewRender(Rect, Vector3, Quaternion, float, float)"/>.
	/// </summary>
	/// <param name="previewRenderer"></param>
	/// <returns></returns>
	public static Texture2D EndSimplePreviewRenderWithAlpha(PreviewRenderUtility previewRenderer)
	{
		// Turn off scene fog
		bool fog = RenderSettings.fog;
		Unsupported.SetRenderSettingsUseFogNoDirty(false);

		// Force render our camera. 
		previewRenderer.Render();

		//Reset our fog settingsS
		Unsupported.SetRenderSettingsUseFogNoDirty(fog);

		// Get our Texture and Clean up our references. EndStaticPreview already does all the work of copying the texture into a texture that wont be cleared
		Texture2D outputTexture = EndStaticPreview(previewRenderer);
		outputTexture.name = "EndSimplePreviewRenderer_Texture";

		// Then just to make 100% sure we not breaking anything. Just immediately delete our static preview
		Object.DestroyImmediate(previewRenderer.EndStaticPreview());

		previewRenderer.Cleanup();

		return outputTexture;
	}

	/// <summary>
	/// Since by default Static Render does not support alpha transparency, we do a custom end Static here
	/// Code mostly Copied from https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/PreviewRenderUtility.cs
	/// </summary>
	/// <param name="previewRenderer"></param>
	/// <returns></returns>
	private static Texture2D EndStaticPreview(PreviewRenderUtility previewRenderer)
	{
		if (!EditorApplication.isUpdating)
			Unsupported.RestoreOverrideLightingSettings();

		// Create a new texture with the correct data
		var tmp = RenderTexture.GetTemporary(previewRenderer.camera.targetTexture.descriptor);

		Vector2 rtSize = new Vector2(tmp.width, tmp.height);

		// Generate our Blit Material
		Shader shader = EditorGUIUtility.LoadRequired("SceneView/GUITextureBlit2SRGB.shader") as Shader;
		Material s_GUITextureBlit2SRGBMaterial = new Material(shader);
		s_GUITextureBlit2SRGBMaterial.hideFlags |= HideFlags.DontSaveInEditor;
		s_GUITextureBlit2SRGBMaterial.SetFloat("_ManualTex2SRGB", QualitySettings.activeColorSpace == ColorSpace.Linear ? 1.0f : 0.0f);

		// Blit our current texture to our new texture
		Graphics.Blit(previewRenderer.camera.targetTexture, tmp, s_GUITextureBlit2SRGBMaterial);

		RenderTexture.active = tmp;
		var copy = new Texture2D((int)rtSize.x, (int)rtSize.y, TextureFormat.RGBA32, false, false);
		copy.ReadPixels(new Rect(0, 0, rtSize.x, rtSize.y), 0, 0);
		copy.Apply();
		RenderTexture.ReleaseTemporary(tmp);

		return copy;
	}

	#endregion

	#region Fitted Property Fields

	public static void DrawFittedPropertyGUI(Rect rectArea, SerializedProperty objectProperty)
	{
		DrawFittedPropertyGUI(rectArea, objectProperty, new GUIContent(objectProperty.displayName));
	}
	public static void DrawFittedPropertyGUI(Rect rectArea, SerializedProperty objectProperty, GUIContent content)
	{
		float halfWidth = rectArea.width / 2;
		Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(objectProperty.displayName));
		textSize.x = (textSize.x > halfWidth) ? halfWidth : textSize.x;

		// Draw our label 
		Rect labelRect = new Rect(rectArea.x, rectArea.y, textSize.x, rectArea.height);
		Rect valueRect = new Rect(labelRect.xMax + FIELD_PADDING, rectArea.y, rectArea.xMax - labelRect.xMax - FIELD_PADDING, rectArea.height);

		EditorGUI.LabelField(labelRect, content);
		EditorGUI.PropertyField(valueRect, objectProperty, GUIContent.none, false);
	}

	#endregion

	#region Property Drawer

	public static void DrawDefaultPropertyDrawer(Rect rect, SerializedProperty property, GUIContent content)
	{
		typeof(EditorGUI)
			.GetMethod("DefaultPropertyField", BindingFlags.NonPublic | BindingFlags.Static)
			.Invoke(null, new object[] { rect, property, content });
	}

	#endregion

	#endregion

}