using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SweatyChair.EditorReflection
{

	public class PropertyHandler
	{
		// handles drawing a single property to the inspector - keeps track of property and decorator drawers

		private PropertyDrawer m_PropertyDrawer = null;

		private List<DecoratorDrawer> m_DecoratorDrawers = null;

		public string tooltip = null;

		public List<ContextMenuItemAttribute> contextMenuItems = null;

		public bool hasPropertyDrawer
		{
			get {
				return this.propertyDrawer != null;
			}
		}

		private PropertyDrawer propertyDrawer
		{
			get {
				return (!this.isCurrentlyNested) ? this.m_PropertyDrawer : null;
			}
		}

		private bool isCurrentlyNested
		{
			get {
				return this.m_PropertyDrawer != null && ScriptAttributeUtility.s_DrawerStack.Any() && this.m_PropertyDrawer == ScriptAttributeUtility.s_DrawerStack.Peek();
			}
		}


		public bool empty
		{
			get {
				return this.m_DecoratorDrawers == null && this.tooltip == null && this.propertyDrawer == null && this.contextMenuItems == null;
			}
		}

		public void HandleAttribute(PropertyAttribute attribute, FieldInfo field, Type propertyType)
		{
			if (attribute is TooltipAttribute) {
				this.tooltip = (attribute as TooltipAttribute).tooltip;
			} else if (attribute is ContextMenuItemAttribute) {
				if (!propertyType.IsArrayOrList()) {
					if (this.contextMenuItems == null) {
						this.contextMenuItems = new List<ContextMenuItemAttribute>();
					}
					this.contextMenuItems.Add(attribute as ContextMenuItemAttribute);
				}
			} else {
				this.HandleDrawnType(attribute.GetType(), propertyType, field, attribute);
			}
		}

		public void HandleDrawnType(Type drawnType, Type propertyType, FieldInfo field, PropertyAttribute attribute)
		{
			Type drawerTypeForType = ScriptAttributeUtility.GetDrawerTypeForType(drawnType);
			if (drawerTypeForType != null) {
				if (typeof(PropertyDrawer).IsAssignableFrom(drawerTypeForType)) {
					if (propertyType != null && propertyType.IsArrayOrList()) {
						return;
					}
					this.m_PropertyDrawer = (PropertyDrawer)Activator.CreateInstance(drawerTypeForType);

					this.m_PropertyDrawer.SetFieldInfo(field);
					this.m_PropertyDrawer.SetAttribute(attribute);
				} else if (typeof(DecoratorDrawer).IsAssignableFrom(drawerTypeForType)) {
					if (field != null && field.FieldType.IsArrayOrList() && !propertyType.IsArrayOrList()) {
						return;
					}
					DecoratorDrawer decoratorDrawer = (DecoratorDrawer)Activator.CreateInstance(drawerTypeForType);

					decoratorDrawer.SetAttribute(attribute);
					if (this.m_DecoratorDrawers == null) {
						this.m_DecoratorDrawers = new List<DecoratorDrawer>();
					}
					this.m_DecoratorDrawers.Add(decoratorDrawer);
				}
			}
		}

		public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren, bool includeDecoratorDrawers = true)
		{
			// save off the ammount of height were supposed to use
			float heighRemaining = position.height;
			position.height = 0f;

			// Do we have any DecoratorDrawers? if so do em
			if (includeDecoratorDrawers) {
				if (this.m_DecoratorDrawers != null && !this.isCurrentlyNested) {
					foreach (DecoratorDrawer decoratorDrawer in this.m_DecoratorDrawers) {
						position.height = decoratorDrawer.GetHeight();
						// cache widths incase they change
						float labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
						float fieldWidth = UnityEditor.EditorGUIUtility.fieldWidth;

						// draw the decorator
						decoratorDrawer.OnGUI(position);

						// reset widths
						UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
						UnityEditor.EditorGUIUtility.fieldWidth = fieldWidth;

						// add update position with the space we just took up
						position.y += position.height;
						heighRemaining -= position.height;
					}
				}
			}

			// plug in the ammount of height left post drawing Decorators
			position.height = heighRemaining;

			// Do we have a property drawer? if so, use it, otherwise falldown to default drawers
			if (this.propertyDrawer != null) {
				float labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
				float fieldWidth = UnityEditor.EditorGUIUtility.fieldWidth;
				this.propertyDrawer.OnGUISafe(position, property.Copy(),
					label ?? EditorGUIUtility.TempContent(property.displayName));
				UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
				UnityEditor.EditorGUIUtility.fieldWidth = fieldWidth;
				return false;
			}
			if (!includeChildren) {
				// if were not drawing nested children, fall down into the default drawer and return
				return EditorGUI.DefaultPropertyField(position, property, label);
			}

			// save off things that might change
			Vector2 iconSize = UnityEditor.EditorGUIUtility.GetIconSize();
			bool enabled = GUI.enabled;
			int indentLevel = UnityEditor.EditorGUI.indentLevel;
			indentLevel = indentLevel - property.depth;

			SerializedProperty serializedProperty = property.Copy(); // copy the itterator so we can advance it without screwing up logic elsewhere
			SerializedProperty endProperty = serializedProperty.GetEndProperty();

			// draw the root property and remove its height from position, so we can draw the children in the right position
			position.height = EditorGUI.GetSinglePropertyHeight(serializedProperty, label);
			UnityEditor.EditorGUI.indentLevel = serializedProperty.depth + indentLevel;
			bool enterChildren = EditorGUI.DefaultPropertyField(position, serializedProperty, label) && EditorGUI.HasVisibleChildFields(serializedProperty);
			position.y += position.height + 2f;

			// while we have children to jump down in to, get them to draw
			while (serializedProperty.NextVisible(enterChildren) && !SerializedProperty.EqualContents(serializedProperty, endProperty)) {
				UnityEditor.EditorGUI.indentLevel = serializedProperty.depth + indentLevel;
				position.height = EditorGUI.GetPropertyHeight(serializedProperty, null, false);
				UnityEditor.EditorGUI.BeginChangeCheck();
				enterChildren = (ScriptAttributeUtility.GetHandler(serializedProperty).OnGUI(position, serializedProperty, null, false) && EditorGUI.HasVisibleChildFields(serializedProperty));
				if (UnityEditor.EditorGUI.EndChangeCheck()) {
					break;
				}
				position.y += position.height + 2f;
			}

			// reset things that might have changed whilst drawing nested children
			GUI.enabled = enabled;
			UnityEditor.EditorGUIUtility.SetIconSize(iconSize);
			UnityEditor.EditorGUI.indentLevel = indentLevel;

			return false;
		}

		public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, bool includeDecoratorDrawers = true, params GUILayoutOption[] options)
		{
			return this.OnGUI(EditorGUILayout.s_LastRect = ((property.propertyType != SerializedPropertyType.Boolean || this.propertyDrawer != null || (includeDecoratorDrawers && (this.m_DecoratorDrawers != null && this.m_DecoratorDrawers.Count != 0)))
				? UnityEditor.EditorGUILayout.GetControlRect(EditorGUI.LabelHasContent(label), this.GetHeight(property, label, includeChildren, includeDecoratorDrawers), options)
				: EditorGUILayout.GetToggleRect(true, options)), property, label, includeChildren, includeDecoratorDrawers); // reflected options param pass might not work?
		}

		// Measure this properties height
		public float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren, bool includeDecoratorDrawers = true)
		{
			float height = 0f;
			if (includeDecoratorDrawers) {
				if (this.m_DecoratorDrawers != null && !this.isCurrentlyNested) {
					// add up the heigh of all attached decorators
					foreach (DecoratorDrawer decoratorDrawer in this.m_DecoratorDrawers) {
						height += decoratorDrawer.GetHeight();
					}
				}
			}

			if (this.propertyDrawer != null) {
				// if this is drawn by a Property drawer, ask that for its height
				height += this.propertyDrawer.GetPropertyHeightSafe(property.Copy(), label ?? EditorGUIUtility.TempContent(property.displayName));
			} else if (!includeChildren) {
				// otherwise, it must use the default drawing logic. If it dosnt have children, lets ask it for its single height
				height += EditorGUI.GetSinglePropertyHeight(property, label);
			} else {
				// otherwise we need to drop down and ask each child how big it is and add that to height
				property = property.Copy(); // copy the itterator so we can advance it without screwing up logic elsewhere
				SerializedProperty endProperty = property.GetEndProperty();
				height += EditorGUI.GetSinglePropertyHeight(property, label);
				bool enterChildren = property.isExpanded && EditorGUI.HasVisibleChildFields(property);
				while (property.NextVisible(enterChildren) && !SerializedProperty.EqualContents(property, endProperty)) {
					height += ScriptAttributeUtility.GetHandler(property).GetHeight(property, EditorGUIUtility.TempContent(property.displayName), true);
					enterChildren = false;
					height += 2f;
				}
			}
			return height;
		}
	}
}