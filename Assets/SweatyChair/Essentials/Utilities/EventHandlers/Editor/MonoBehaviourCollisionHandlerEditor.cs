using UnityEditor;
using System;

namespace SweatyChair.Events
{
	[CustomEditor (typeof(MonoBehaviourCollisionHandler)), CanEditMultipleObjects]
	public class MonoBehaviourCollisionHandlerEditor : Editor
	{
		MonoBehaviourCollisionHandler _target;

		public override void OnInspectorGUI ()
		{
			_target = (MonoBehaviourCollisionHandler)target;

			serializedObject.Update ();

			EditorGUILayout.PropertyField (serializedObject.FindProperty ("collisionMask"));
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("selectedCollisionEvents"));

			for (int i = 0; i < Enum.GetValues(typeof(MonoCollisionType)).Length; i++) {
				if ((int)_target.selectedCollisionEvents == ((int)_target.selectedCollisionEvents | (1 << i)))
					EditorGUILayout.PropertyField (serializedObject.FindProperty (((MonoCollisionType)(1 << i)).ToString()));
			}

			serializedObject.ApplyModifiedProperties ();
		}
	}
}