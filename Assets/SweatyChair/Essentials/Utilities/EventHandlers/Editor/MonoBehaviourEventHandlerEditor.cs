using UnityEditor;
using System;

namespace SweatyChair.Events
{

	[CustomEditor(typeof(MonoBehaviourEventHandler)), CanEditMultipleObjects]
	public class MonoBehaviourEventHandlerEditor : Editor
	{
		MonoBehaviourEventHandler _target;

		public override void OnInspectorGUI()
		{
			_target = (MonoBehaviourEventHandler)target;

			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedMonoEvents"));

			for (int i = 0; i < Enum.GetValues(typeof(MonoEventType)).Length; i++) {
				if ((int)_target.selectedMonoEvents == ((int)_target.selectedMonoEvents | (1 << i)))
					EditorGUILayout.PropertyField(serializedObject.FindProperty(((MonoEventType)(1 << i)).ToString().ToLower()));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}

}