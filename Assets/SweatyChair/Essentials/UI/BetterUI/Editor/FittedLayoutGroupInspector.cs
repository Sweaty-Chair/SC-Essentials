using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;

namespace UnityEditor.UI
{
	[CustomEditor(typeof(FittedGridLayoutGroup), true)]
	[CanEditMultipleObjects]
	public class FittedLayoutGroupInspector : Editor
	{
		SerializedProperty m_Padding;
		SerializedProperty m_CellSize;
		SerializedProperty m_Spacing;
		SerializedProperty m_StartCorner;
		SerializedProperty m_StartAxis;
		SerializedProperty m_ChildAlignment;
		SerializedProperty m_Constraint;
		SerializedProperty m_ConstraintCount;

		SerializedProperty m_PrefferedGridLayoutGroup;
		SerializedProperty m_FitToSize;
		SerializedProperty m_UseRatio;
		SerializedProperty m_PrefferedRatio;

		protected virtual void OnEnable() {
			m_Padding = serializedObject.FindProperty("m_Padding");
			m_CellSize = serializedObject.FindProperty("m_CellSize");
			m_Spacing = serializedObject.FindProperty("m_Spacing");
			m_StartCorner = serializedObject.FindProperty("m_StartCorner");
			m_StartAxis = serializedObject.FindProperty("m_StartAxis");
			m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
			m_Constraint = serializedObject.FindProperty("m_Constraint");
			m_ConstraintCount = serializedObject.FindProperty("m_ConstraintCount");

			m_PrefferedGridLayoutGroup = serializedObject.FindProperty("m_PrefferedCellSize");
			m_FitToSize = serializedObject.FindProperty("m_fitToSize");
			m_UseRatio = serializedObject.FindProperty("m_useRatio");
			m_PrefferedRatio = serializedObject.FindProperty("m_prefferedRatio");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.PropertyField(m_Padding, true);

			//Dont allow input on our cell size
			GUI.enabled = false;
			EditorGUILayout.PropertyField(m_CellSize, true);
			GUI.enabled = true;

			EditorGUILayout.PropertyField(m_Spacing, true);
			EditorGUILayout.PropertyField(m_StartCorner, true);
			EditorGUILayout.PropertyField(m_StartAxis, true);
			EditorGUILayout.PropertyField(m_ChildAlignment, true);
			EditorGUILayout.PropertyField(m_Constraint, true);
			if (m_Constraint.enumValueIndex > 0) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(m_ConstraintCount, true);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(m_FitToSize, true);
			if (!m_FitToSize.boolValue) {
				EditorGUILayout.PropertyField(m_PrefferedGridLayoutGroup, true);
			}

			EditorGUILayout.PropertyField(m_UseRatio, true);
			if (m_UseRatio.boolValue) {
				EditorGUILayout.PropertyField(m_PrefferedRatio, true);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}