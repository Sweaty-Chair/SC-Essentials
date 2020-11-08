using UnityEditor;
using System;

namespace SweatyChair.Events 
{
    [CustomEditor(typeof(MonoBehaviourCollision2DHandler)), CanEditMultipleObjects]
    public class MonoBehaviourCollision2DHandlerEditor : Editor {
        MonoBehaviourCollision2DHandler _target;

        public override void OnInspectorGUI() {
            _target = (MonoBehaviourCollision2DHandler)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionMask"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedCollisionEvents"));

            for (int i = 0; i < Enum.GetValues(typeof(MonoCollisionType)).Length; i++) {
                if ((int)_target.selectedCollisionEvents == ((int)_target.selectedCollisionEvents | (1 << i)))
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(((MonoCollisionType)(1 << i)).ToString() + "2D"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}