using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace SweatyChair.EditorReflection
{
	// provides easy access to a bunch of APIs that are marked as internal in the UnityEditor. we wimply copy the method signiture
	// and use reflection to get access to stuff were not supposed to 
	public static class EditorGUIUtility
	{
		internal static GUIContent TempContent(string t)
		{
			var bindflags = BindingFlags.NonPublic | BindingFlags.Static;
			var method = typeof(UnityEditor.EditorGUIUtility).GetMethod("TempContent", bindflags, null, new[] { typeof(string) }, null);

			return (GUIContent)method.Invoke(null, new[] { t });
		}
	}

	public static class EditorGUILayout
	{
		public static Rect s_LastRect
		{
			get {
				return ReflectionHelper.GetField<Rect>("s_LastRect", typeof(UnityEditor.EditorGUILayout));
			}
			set {
				ReflectionHelper.SetField<Rect>("s_LastRect", typeof(UnityEditor.EditorGUILayout), value);
			}
		}

		internal static Rect GetToggleRect(bool hasLabel, params GUILayoutOption[] options)
		{
			return (Rect)ReflectionHelper.GetReflectedMethod("GetToggleRect", typeof(UnityEditor.EditorGUILayout)).Invoke(null, new object[] { hasLabel, options });
		}
	}

	public static class EditorGUI
	{
		public static bool LabelHasContent(GUIContent label)
		{
			if (label == null) {
				return true;
			}
			return label.text != string.Empty || label.image != null;
		}

		public static float GetSinglePropertyHeight(SerializedProperty property, GUIContent label)
		{
			return (float)ReflectionHelper.GetReflectedMethod("GetSinglePropertyHeight", typeof(UnityEditor.EditorGUI)).Invoke(null, new object[] { property, label });
		}

		internal static bool HasVisibleChildFields(SerializedProperty property)
		{
			return (bool)ReflectionHelper.GetReflectedMethod("HasVisibleChildFields", typeof(UnityEditor.EditorGUI)).Invoke(null, new object[] { property });
		}

		internal static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label)
		{
			return (bool)ReflectionHelper.GetReflectedMethod("DefaultPropertyField", typeof(UnityEditor.EditorGUI)).Invoke(null, new object[] { position, property, label });
		}

		public static float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
		{
			return GetPropertyHeightInternal(property, label, includeChildren);
		}

		// Get the height needed for a ::ref::PropertyField control.
		internal static float GetPropertyHeightInternal(SerializedProperty property, GUIContent label, bool includeChildren)
		{
			return ScriptAttributeUtility.GetHandler(property).GetHeight(property, label, includeChildren);
		}

	}

	public class EditorAssemblies
	{
		internal static IEnumerable<Type> SubclassesOf(Type parent)
		{
			Type hiddenType = ReflectionHelper.GetPrivateType("UnityEditor.EditorAssemblies", typeof(CustomEditor));
			return (IEnumerable<Type>)ReflectionHelper.GetReflectedMethod("SubclassesOf", hiddenType).Invoke(null, new object[] { parent });
		}
	}

	public static class ReflectionHelper
	{
		public static Type GetPrivateType(string name, Type source)
		{
			var assembly = source.Assembly;

			return assembly.GetType(name);
		}

		public static Type GetPrivateType(string fqName)
		{
			return Type.GetType(fqName);
		}

		public static T GetField<T>(string name, Type type, bool isStatic = true, object instance = null)
		{
			var bindflags = isStatic ? (BindingFlags.NonPublic | BindingFlags.Static) : (BindingFlags.NonPublic | BindingFlags.Instance);
			var field = type.GetField(name, bindflags);

			return (T)field.GetValue(instance);
		}

		public static void SetField<T>(string name, Type type, T value, bool isStatic = true, object instantce = null)
		{
			var bindflags = isStatic ? (BindingFlags.NonPublic | BindingFlags.Static) : (BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			var field = type.GetField(name, bindflags);

			if (instantce != null) {
				field = instantce.GetType().GetField(name, bindflags);
			}

			field.SetValue(instantce, value);
		}

		public static T GetProperty<T>(string name, Type type, object instance)
		{
			var bindflags = BindingFlags.NonPublic | BindingFlags.Instance;
			var propInfo = type.GetProperty(name, bindflags);

			MethodInfo getAccessor = propInfo.GetGetMethod(true);

			return (T)getAccessor.Invoke(instance, null);
		}

		public static MethodInfo GetReflectedMethod(string name, Type type, bool isStatic = true, object instantce = null)
		{
			var bindflags = isStatic ? (BindingFlags.NonPublic | BindingFlags.Static) : (BindingFlags.NonPublic | BindingFlags.Instance);
			var method = type.GetMethod(name, bindflags);

			return method;
		}
	}

	public static class PropertyDrawerExtention
	{
		public static float GetPropertyHeightSafe(this PropertyDrawer drawer, SerializedProperty property, GUIContent label)
		{
			return (float)ReflectionHelper.GetReflectedMethod("GetPropertyHeightSafe", typeof(UnityEditor.PropertyDrawer), false, drawer).Invoke(drawer, new object[] { property, label });
		}

		public static void OnGUISafe(this PropertyDrawer drawer, Rect position, SerializedProperty property, GUIContent label)
		{
			ReflectionHelper.GetReflectedMethod("OnGUISafe", typeof(UnityEditor.PropertyDrawer), false, drawer).Invoke(drawer, new object[] { position, property, label });
		}

		public static void SetFieldInfo(this PropertyDrawer drawer, FieldInfo info)
		{
			ReflectionHelper.SetField("m_FieldInfo", typeof(PropertyDrawer), info, false, drawer);
		}

		public static void SetAttribute(this PropertyDrawer drawer, PropertyAttribute attrib)
		{
			ReflectionHelper.SetField("m_Attribute", typeof(PropertyDrawer), attrib, false, drawer);
		}
	}

	public static class DecoratorDrawerExtention
	{
		public static void SetAttribute(this DecoratorDrawer drawer, PropertyAttribute attrib)
		{
			ReflectionHelper.SetField("m_Attribute", typeof(DecoratorDrawer), attrib, false, drawer);
		}
	}

	public static class CustomPropertyDrawerExtentions
	{
		public static Type GetHiddenType(this CustomPropertyDrawer prop)
		{
			return ReflectionHelper.GetField<Type>("m_Type", typeof(CustomPropertyDrawer), false, prop);
		}

		public static bool GetUseForChildren(this CustomPropertyDrawer prop)
		{
			return ReflectionHelper.GetField<bool>("m_UseForChildren", typeof(CustomPropertyDrawer), false, prop);
		}
	}

	public static class SerializedPropertyExtentions
	{
		public static int GetHashCodeForPropertyPathWithoutArrayIndex(this SerializedProperty prop)
		{
			return ReflectionHelper.GetProperty<int>("hashCodeForPropertyPathWithoutArrayIndex", typeof(SerializedProperty), prop);
		}
	}

	public static class SerializedObjectExtentions
	{
		public static int GetInspectorMode(this SerializedObject prop)
		{
			//return ReflectionHelper.GetField<int>("inspectorMode", typeof(SerializedObject), false, prop);
			return ReflectionHelper.GetProperty<int>("inspectorMode", typeof(SerializedObject), prop);
		}
	}
}