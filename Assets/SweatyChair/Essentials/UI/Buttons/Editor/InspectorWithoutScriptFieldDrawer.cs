using UnityEditor;

namespace SweatyChair
{
	public class InspectorWithoutScriptFieldDrawer : Editor
	{

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			SerializedProperty Iterator = serializedObject.GetIterator();

			Iterator.NextVisible(true);

			while (Iterator.NextVisible(false)) {
				EditorGUILayout.PropertyField(Iterator, true);
			}

			serializedObject.ApplyModifiedProperties();
		}

	}
}
