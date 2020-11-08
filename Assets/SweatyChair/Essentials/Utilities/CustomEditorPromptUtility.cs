using SweatyChair.Prompts;
using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

	public static class CustomEditorPromptUtility
	{

		#region Static

		#region Text Prompt

		public static void DisplayTextPrompt(string title, string message, string ok, UnityAction<string> onSubmit, string cancel = "", UnityAction onCancel = null)
		{
#if UNITY_EDITOR
			EditorTextPrompt prompt = new EditorTextPrompt(title, message, string.Empty, ok, onSubmit, cancel, onCancel);
#if UNITY_2019_1_OR_NEWER
			prompt.ShowModalUtility();
#else
			prompt.ShowUtility();
#endif
#endif
		}

		public static void DisplayTextPromptWithDefault(string title, string message, string defaultInput, string ok, UnityAction<string> onSubmit, string cancel = "", UnityAction onCancel = null)
		{
#if UNITY_EDITOR
			EditorTextPrompt prompt = new EditorTextPrompt(title, message, defaultInput, ok, onSubmit, cancel, onCancel);
#if UNITY_2019_1_OR_NEWER
			prompt.ShowModalUtility();
#else
			prompt.ShowUtility();
#endif
#endif
		}

		#endregion

		#endregion

	}
}

#region Custom EditorWindow Prompts

namespace SweatyChair.Prompts
{

#if UNITY_EDITOR

	public class EditorTextPrompt : UnityEditor.EditorWindow
	{

		#region Variables

		// Window data
		private string message;
		private string okButtonText;
		private string cancelButtonText;

		private UnityAction<string> submitCallback;
		private UnityAction cancelCallback;

		// Cached data
		private string cachedInputString;

		#endregion

		#region Constructor

		public EditorTextPrompt(string title, string message, string defaultInput, string ok, UnityAction<string> onSubmit, string cancel, UnityAction onCancel) : base()
		{
			this.message = message;
			this.okButtonText = ok;
			this.cancelButtonText = cancel;

			this.submitCallback = onSubmit;
			this.cancelCallback = onCancel;

			this.cachedInputString = defaultInput;

			// Set our Title
			titleContent = new GUIContent(title);

			// Hard code our size cause we know how big it will always be
			int padding = 15 + 18 + 36 + 18;
			minSize = new Vector2(300, padding);
			maxSize = new Vector2(300, padding);
		}

		#endregion

		#region OnGUI

		private void OnGUI()
		{
			GUILayout.Space(5);

			AddLeftPaddedSection(5, DrawMessage);
			AddLeftPaddedSection(15, DrawTextField);
			AddLeftPaddedSection(position.width * 0.333f, DrawConfirmButtons);

			GUILayout.Space(5);
		}

		private void DrawMessage()
		{
			UnityEditor.EditorGUILayout.LabelField(new GUIContent(message), UnityEditor.EditorStyles.wordWrappedLabel, GUILayout.Height(UnityEditor.EditorGUIUtility.singleLineHeight * 2));
		}

		private void DrawTextField()
		{
			cachedInputString = UnityEditor.EditorGUILayout.TextField(cachedInputString);
		}

		private void DrawConfirmButtons()
		{
			// Always draw our confirm button
			if (GUILayout.Button(new GUIContent(okButtonText))) {
				submitCallback?.Invoke(cachedInputString);
				Close();
			}

			// Only draw our cancel button if we have text to display
			if (!string.IsNullOrWhiteSpace(cancelButtonText)) {
				if (GUILayout.Button(new GUIContent(cancelButtonText))) {
					cancelCallback?.Invoke();
					Close();
				}
			}
		}

		#endregion

		#region Utility

		private void AddLeftPaddedSection(float paddingSize, UnityAction contentDrawing)
		{
			// Start a horizontal section
			UnityEditor.EditorGUILayout.BeginHorizontal();

			// Add our padding
			GUILayout.Space(paddingSize);

			// invoke our drawing function
			contentDrawing?.Invoke();

			// End our horizontal
			UnityEditor.EditorGUILayout.EndHorizontal();
		}

		#endregion

	}

#endif

}

#endregion