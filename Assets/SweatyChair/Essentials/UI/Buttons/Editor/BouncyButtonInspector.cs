using UnityEditor;

namespace UnityEngine.UI
{

	[CanEditMultipleObjects()]
	[CustomEditor(typeof(BouncyButton))]
	public class BouncyButtonInspector : Editor
	{

		#region Variables

		private InlineSOProperty _presetProperty;

		#endregion

		#region OnEnable / OnDisable

		private void OnEnable()
		{
			_presetProperty = new InlineSOProperty(serializedObject.FindProperty("_preset"));
		}

		private void OnDisable()
		{
			_presetProperty.Reset();
		}

		#endregion

		#region On Inspector GUI

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			_presetProperty.DrawGUI();

			serializedObject.ApplyModifiedProperties();
		}

		#endregion

	}

}
