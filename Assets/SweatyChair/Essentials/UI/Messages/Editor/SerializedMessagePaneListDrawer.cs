using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SweatyChair.UI
{

	[CustomPropertyDrawer(typeof(MessagePanel.SerializedMessagePanelList))]
	public class SerializedMessagePanelListDrawer : PropertyDrawer
	{

		#region Const

		private const float FIELD_PADDING = 2f;

		private const string LIST_PROPERTY_NAME = "subPanelPair";
		private const string NAME_PROPERTY = "format";
		private const string ID_PROPERTY = "panel";

		#endregion

		#region Variables

		private ReorderableList _stateList;

		#endregion

		#region OnGUI / Property Height

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			using (new EditorGUI.PropertyScope(position, label, property)) {
				var list = GetList(property);
				list.DoList(position);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var states = GetList(property);
			return states.GetHeight();
		}

		#endregion

		#region Reorderable list stuff

		private ReorderableList GetList(SerializedProperty property)
		{
			if (_stateList == null) {
				var listProperty = property.FindPropertyRelative(LIST_PROPERTY_NAME);
				_stateList = new ReorderableList(property.serializedObject, property.FindPropertyRelative(LIST_PROPERTY_NAME));

				_stateList.drawHeaderCallback = (Rect rect) => {
					EditorGUI.LabelField(rect, "States");
				};

				_stateList.drawElementCallback =
					(Rect rect, int index, bool isActive, bool isFocused) => {
						rect.yMin += 1;
						rect.yMax -= 1;

						// Get two rects for our meme
						Rect rectLeft = new Rect(rect);
						rectLeft.xMax -= (rect.width / 2) + FIELD_PADDING;
						Rect rectRight = new Rect(rect);
						rectRight.xMin += (rect.width / 2) + FIELD_PADDING;

						EditorUtils.DrawFittedPropertyGUI(rectLeft, listProperty.GetArrayElementAtIndex(index).FindPropertyRelative(NAME_PROPERTY), new GUIContent("Format"));
						EditorUtils.DrawFittedPropertyGUI(rectRight, listProperty.GetArrayElementAtIndex(index).FindPropertyRelative(ID_PROPERTY), new GUIContent("Panel"));
					};
			}

			return _stateList;
		}

		#endregion

	}

}