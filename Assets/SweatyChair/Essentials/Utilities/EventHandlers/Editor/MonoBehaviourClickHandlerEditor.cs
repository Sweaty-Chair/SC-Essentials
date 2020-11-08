using UnityEditor;
using System;

namespace SweatyChair.Events
{
	[CustomEditor (typeof(MonoBehaviourClickHandler)), CanEditMultipleObjects]
	public class MonoBehaviourClickHandlerEditor : Editor
	{
		MonoBehaviourClickHandler _target;

		public override void OnInspectorGUI ()
		{
			_target = (MonoBehaviourClickHandler)target;

			serializedObject.Update ();

			EditorGUILayout.PropertyField (serializedObject.FindProperty ("selectedClickEvents"));

			for (int i = 0; i < Enum.GetValues(typeof(MonoClickType)).Length; i++) {
				if ((int)_target.selectedClickEvents == ((int)_target.selectedClickEvents | (1 << i)))
					EditorGUILayout.PropertyField (serializedObject.FindProperty (((MonoClickType)(1 << i)).ToString()));
			}

			serializedObject.ApplyModifiedProperties ();
		}
	}
}