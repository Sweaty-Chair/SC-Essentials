using UnityEditor;
using UnityEngine;

namespace SweatyChair
{

	// In Unity 2020.1 serialization of generic types are supported, so this would no longer need to be a base implementation for all other classes to be used with

	[CustomPropertyDrawer(typeof(ParameterOverride))]
	public class ParameterOverrideDrawer : PropertyDrawer
	{

		#region OnGUI

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Init our Label and our Property
			label = EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			bool cachedMode = EditorGUIUtility.hierarchyMode;
			EditorGUIUtility.hierarchyMode = false;

			SerializedProperty overrideState = property.FindPropertyRelative("overrideState");
			SerializedProperty valueProperty = property.FindPropertyRelative("value");

			// Override checkbox
			Rect overrideRect = new Rect(position.x, position.y, 17f, 17f);
			overrideState.boolValue = GUI.Toggle(overrideRect, overrideState.boolValue, new GUIContent("", "Toggles the override on or off"));
			position.xMin += 19f;

			// Property
			using (new EditorGUI.DisabledScope(!overrideState.boolValue)) {
				property.isExpanded = (valueProperty.isArray || valueProperty.propertyType == SerializedPropertyType.Generic) && (valueProperty.hasVisibleChildren && !valueProperty.isExpanded);

				GUIContent valueContent = property.isExpanded ? new GUIContent(string.Format("See more")) : GUIContent.none;

				float cachedLabelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = Mathf.Min(EditorStyles.label.CalcSize(new GUIContent("Element00000:")).x, position.x / 2);

				EditorGUI.PropertyField(position, valueProperty, valueContent, true);

				EditorGUIUtility.labelWidth = cachedLabelWidth;
			}

			EditorGUIUtility.hierarchyMode = cachedMode;

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}

		#endregion

		#region Property Height

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"));
		}

		#endregion

	}

}
