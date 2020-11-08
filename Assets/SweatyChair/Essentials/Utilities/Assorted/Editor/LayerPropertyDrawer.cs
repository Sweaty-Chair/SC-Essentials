using UnityEditor;
using UnityEngine;

namespace SweatyChair
{

	[CustomPropertyDrawer(typeof(Layer))]
	public class LayerPropertyDrawer : PropertyDrawer
	{
		#region OnGUI

		public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
		{
			// Begin our Property
			EditorGUI.BeginProperty(_position, GUIContent.none, _property);

			// Init our Property
			SerializedProperty layerIndex = _property.FindPropertyRelative("m_LayerIndex");
			_position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);

			if (layerIndex != null)
				layerIndex.intValue = EditorGUI.LayerField(_position, layerIndex.intValue);

			EditorGUI.EndProperty();
		}

		#endregion
	}

}