using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SweatyChair.EditorReflection
{

	// clone of an internal Unity class that does stuff like keep track of PropertyHandlers and finding Attributes the inspector uses for drawing
	internal class ScriptAttributeUtility
	{
		private struct DrawerKeySet
		{
			public Type drawer;

			public Type type;
		}

		internal static Stack<PropertyDrawer> s_DrawerStack = new Stack<PropertyDrawer>();

		private static Dictionary<Type, DrawerKeySet> s_DrawerTypeForType = null;

		private static Dictionary<string, List<PropertyAttribute>> s_BuiltinAttributes = null;

		private static PropertyHandler s_SharedNullHandler = new PropertyHandler();

		private static PropertyHandler s_NextHandler = new PropertyHandler();

		private static PropertyHandlerCache s_GlobalCache = new PropertyHandlerCache();

		private static PropertyHandlerCache s_CurrentCache = null;

		internal static PropertyHandlerCache propertyHandlerCache
		{
			get {
				return s_CurrentCache ?? s_GlobalCache;
			}
			set {
				s_CurrentCache = value;
			}
		}

		internal static void ClearGlobalCache() // TODO in Unity this is called by InspectorWindow.OnSelectionChange(), need to trigger this outselves from some selection change event
		{
			s_GlobalCache.Clear();
		}

		private static void PopulateBuiltinAttributes()
		{
			s_BuiltinAttributes = new Dictionary<string, List<PropertyAttribute>>();
			AddBuiltinAttribute("GUIText", "m_Text", new MultilineAttribute());
			AddBuiltinAttribute("TextMesh", "m_Text", new MultilineAttribute());
		}

		private static void AddBuiltinAttribute(string componentTypeName, string propertyPath, PropertyAttribute attr)
		{
			string key = componentTypeName + "_" + propertyPath;
			if (!s_BuiltinAttributes.ContainsKey(key)) {
				s_BuiltinAttributes.Add(key, new List<PropertyAttribute>());
			}
			s_BuiltinAttributes[key].Add(attr);
		}

		private static List<PropertyAttribute> GetBuiltinAttributes(SerializedProperty property)
		{
			if (property.serializedObject.targetObject == null) {
				return null;
			}
			Type type = property.serializedObject.targetObject.GetType();
			if (type == null) {
				return null;
			}
			string key = type.Name + "_" + property.propertyPath;
			List<PropertyAttribute> result = null;
			s_BuiltinAttributes.TryGetValue(key, out result);
			return result;
		}

		private static void BuildDrawerTypeForTypeDictionary()
		{
			ScriptAttributeUtility.s_DrawerTypeForType = new Dictionary<Type, DrawerKeySet>();
			Type[] source = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => AssemblyHelper.GetTypesFromAssembly(x)).ToArray();
			foreach (Type item in EditorAssemblies.SubclassesOf(typeof(GUIDrawer))) {
				object[] customAttributes = item.GetCustomAttributes(typeof(CustomPropertyDrawer), true);
				object[] array = customAttributes;
				for (int i = 0; i < array.Length; i++) {
					CustomPropertyDrawer editor = (CustomPropertyDrawer)array[i];
					//ScriptAttributeUtility.s_DrawerTypeForType[editor.m_Type] = new DrawerKeySet
					s_DrawerTypeForType[editor.GetHiddenType()] = new DrawerKeySet {
						drawer = item,
						type = editor.GetHiddenType() /*editor.m_Type*/
					};
					if (editor.GetUseForChildren() /*editor.m_UseForChildren*/) {
						IEnumerable<Type> enumerable = from x in source
													   where x.IsSubclassOf(editor.GetHiddenType() /*editor.m_Type*/)
													   select x;
						foreach (Type item2 in enumerable) {
							if (ScriptAttributeUtility.s_DrawerTypeForType.ContainsKey(item2)) {
								Type type = editor.GetHiddenType(); //editor.m_Type;
								DrawerKeySet drawerKeySet = ScriptAttributeUtility.s_DrawerTypeForType[item2];
								if (!type.IsAssignableFrom(drawerKeySet.type)) {
									goto IL_0158;
								}
								continue;
							}
							goto IL_0158;
							IL_0158:
							ScriptAttributeUtility.s_DrawerTypeForType[item2] = new DrawerKeySet {
								drawer = item,
								type = editor.GetHiddenType() //editor.m_Type;
							};
						}
					}
				}
			}
		}

		internal static Type GetDrawerTypeForType(Type type)
		{
			if (ScriptAttributeUtility.s_DrawerTypeForType == null) {
				ScriptAttributeUtility.BuildDrawerTypeForTypeDictionary();
			}
			DrawerKeySet drawerKeySet = default(DrawerKeySet);
			ScriptAttributeUtility.s_DrawerTypeForType.TryGetValue(type, out drawerKeySet);
			if (drawerKeySet.drawer != null) {
				return drawerKeySet.drawer;
			}
			if (type.IsGenericType) {
				ScriptAttributeUtility.s_DrawerTypeForType.TryGetValue(type.GetGenericTypeDefinition(), out drawerKeySet);
			}
			return drawerKeySet.drawer;
		}

		private static List<PropertyAttribute> GetFieldAttributes(FieldInfo field)
		{
			if (field == null) {
				return null;
			}
			object[] customAttributes = field.GetCustomAttributes(typeof(PropertyAttribute), true);
			if (customAttributes != null && customAttributes.Length > 0) {
				return new List<PropertyAttribute>(from e in customAttributes
												   select e as PropertyAttribute into e
												   orderby -e.order
												   select e);
			}
			return null;
		}

		public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
		{
			Type scriptTypeFromProperty = ScriptAttributeUtility.GetScriptTypeFromProperty(property);
			if (scriptTypeFromProperty == null) {
				type = null;
				return null;
			}
			return ScriptAttributeUtility.GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath, out type);
		}

		private static Type GetScriptTypeFromProperty(SerializedProperty property)
		{
			SerializedProperty serializedProperty = property.serializedObject.FindProperty("m_Script");
			if (serializedProperty == null) {
				return null;
			}
			MonoScript monoScript = serializedProperty.objectReferenceValue as MonoScript;
			if (monoScript == null) {
				return null;
			}
			return monoScript.GetClass();
		}

		private static FieldInfo GetFieldInfoFromPropertyPath(Type host, string path, out Type type)
		{
			FieldInfo fieldInfo = null;
			type = host;
			string[] array = path.Split('.');
			for (int i = 0; i < array.Length; i++) {
				string text = array[i];
				if (i < array.Length - 1 && text == "Array" && array[i + 1].StartsWith("data[")) {
					if (type.IsArrayOrList()) {
						type = type.GetArrayOrListElementType();
					}
					i++;
				} else {
					FieldInfo fieldInfo2 = null;
					Type type2 = type;
					while (fieldInfo2 == null && type2 != null) {
						fieldInfo2 = type2.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						type2 = type2.BaseType;
					}
					if (fieldInfo2 == null) {
						type = null;
						return null;
					}
					fieldInfo = fieldInfo2;
					type = fieldInfo.FieldType;
				}
			}
			return fieldInfo;
		}

		internal static PropertyHandler GetHandler(SerializedProperty property)
		{
			if (property == null) {
				return ScriptAttributeUtility.s_SharedNullHandler;
			}
			if (property.serializedObject.GetInspectorMode() != 0) {
				return ScriptAttributeUtility.s_SharedNullHandler;
			}
			PropertyHandler handler = ScriptAttributeUtility.propertyHandlerCache.GetHandler(property);
			if (handler != null) {
				return handler;
			}
			Type type = null;
			List<PropertyAttribute> list = null;
			FieldInfo field = null;
			UnityEngine.Object targetObject = property.serializedObject.targetObject;
			if (targetObject is MonoBehaviour || targetObject is ScriptableObject) {
				field = ScriptAttributeUtility.GetFieldInfoFromProperty(property, out type);
				list = ScriptAttributeUtility.GetFieldAttributes(field);
			} else {
				if (ScriptAttributeUtility.s_BuiltinAttributes == null) {
					ScriptAttributeUtility.PopulateBuiltinAttributes();
				}
				if (list == null) {
					list = ScriptAttributeUtility.GetBuiltinAttributes(property);
				}
			}
			handler = ScriptAttributeUtility.s_NextHandler;
			if (list != null) {
				for (int num = list.Count - 1; num >= 0; num--) {
					handler.HandleAttribute(list[num], field, type);
				}
			}
			if (!handler.hasPropertyDrawer && type != null) {
				handler.HandleDrawnType(type, type, field, null);
			}
			if (handler.empty) {
				ScriptAttributeUtility.propertyHandlerCache.SetHandler(property, ScriptAttributeUtility.s_SharedNullHandler);
				handler = ScriptAttributeUtility.s_SharedNullHandler;
			} else {
				ScriptAttributeUtility.propertyHandlerCache.SetHandler(property, handler);
				ScriptAttributeUtility.s_NextHandler = new PropertyHandler();
			}
			return handler;
		}
	}

	// stores PropertyHandlers for drawing properties in a dictionary against property hashes 
	internal class PropertyHandlerCache
	{
		protected Dictionary<int, PropertyHandler> m_PropertyHandlers = new Dictionary<int, PropertyHandler>();

		internal PropertyHandler GetHandler(SerializedProperty property)
		{
			int propertyHash = GetPropertyHash(property);
			PropertyHandler result = null;
			if (this.m_PropertyHandlers.TryGetValue(propertyHash, out result)) {
				return result;
			}
			return null;
		}

		internal void SetHandler(SerializedProperty property, PropertyHandler handler)
		{
			int propertyHash = GetPropertyHash(property);
			m_PropertyHandlers[propertyHash] = handler;
		}

		private static int GetPropertyHash(SerializedProperty property)
		{
			if (property.serializedObject.targetObject == null) {
				return 0;
			}
			int num = property.serializedObject.targetObject.GetInstanceID() ^ property.GetHashCodeForPropertyPathWithoutArrayIndex(); /*property.hashCodeForPropertyPathWithoutArrayIndex;*/
			if (property.propertyType == SerializedPropertyType.ObjectReference) {
				num ^= property.objectReferenceInstanceIDValue;
			}
			return num;
		}

		public void Clear()
		{
			this.m_PropertyHandlers.Clear();
		}
	}

}
