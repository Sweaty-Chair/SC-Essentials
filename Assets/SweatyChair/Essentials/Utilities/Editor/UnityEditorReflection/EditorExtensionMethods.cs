using System;
using System.Collections.Generic;
using System.Reflection;

namespace SweatyChair.EditorReflection
{


	public static class EditorExtensionMethods
	{
		// Public reimplmentation of extention methods and helpers that are marked internal in UnityEditor.dll

		public static bool IsArrayOrList(this Type listType)
		{
			return listType.IsArray || (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>));
		}

		public static Type GetArrayOrListElementType(this Type listType)
		{
			if (listType.IsArray) {
				return listType.GetElementType();
			}
			if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>)) {
				return listType.GetGenericArguments()[0];
			}

			return null;
		}
	}


	public class AssemblyHelper
	{
		internal static Type[] GetTypesFromAssembly(Assembly assembly)
		{
			if (assembly == null) {
				return new Type[0];
			}
			try {
				return assembly.GetTypes();
			} catch (ReflectionTypeLoadException) {
				return new Type[0];
			}
		}
	}

}