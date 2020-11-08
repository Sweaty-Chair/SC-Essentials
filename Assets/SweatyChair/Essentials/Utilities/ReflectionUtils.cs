﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SweatyChair
{

	public static class ReflectionUtils
	{

		#region Attribute Utility

		public static List<Type> GetAllClassesWithAttribute(Type attributeType)
		{
			// Cache a list to return with all attributes of our type
			List<Type> typesWithAttribute = new List<Type>();

			// First lets get all currently loaded assemblies
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			// Go through all of our assemblies, and check if we have our required attribute
			for (int i = 0; i < assemblies.Length; i++) {

				// Go through each of our types defined in the current assembly, and check for our attribute
				foreach (Type type in assemblies[i].GetTypes()) {

					if (type.IsDefined(attributeType, true))
						typesWithAttribute.Add(type);
				}
			}

			return typesWithAttribute;
		}

		#endregion

		#region Subclass of Generic

		// Thanks Stack Overflow
		// https://stackoverflow.com/a/18828085

		public static bool IsSubClassOfGeneric(this Type child, Type parent)
		{
			if (child == parent)
				return false;

			if (child.IsSubclassOf(parent))
				return true;

			var parameters = parent.GetGenericArguments();
			var isParameterLessGeneric = !(parameters != null && parameters.Length > 0 &&
				((parameters[0].Attributes & TypeAttributes.BeforeFieldInit) == TypeAttributes.BeforeFieldInit));

			while (child != null && child != typeof(object)) {
				var cur = GetFullTypeDefinition(child);
				if (parent == cur || (isParameterLessGeneric && cur.GetInterfaces().Select(i => GetFullTypeDefinition(i)).Contains(GetFullTypeDefinition(parent))))
					return true;
				else if (!isParameterLessGeneric)
					if (GetFullTypeDefinition(parent) == cur && !cur.IsInterface) {
						if (VerifyGenericArguments(GetFullTypeDefinition(parent), cur))
							if (VerifyGenericArguments(parent, child))
								return true;
					} else
						foreach (var item in child.GetInterfaces().Where(i => GetFullTypeDefinition(parent) == GetFullTypeDefinition(i)))
							if (VerifyGenericArguments(parent, item))
								return true;

				child = child.BaseType;
			}

			return false;
		}

		private static Type GetFullTypeDefinition(Type type)
		{
			return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
		}

		private static bool VerifyGenericArguments(Type parent, Type child)
		{
			Type[] childArguments = child.GetGenericArguments();
			Type[] parentArguments = parent.GetGenericArguments();
			if (childArguments.Length == parentArguments.Length)
				for (int i = 0; i < childArguments.Length; i++)
					if (childArguments[i].Assembly != parentArguments[i].Assembly || childArguments[i].Name != parentArguments[i].Name || childArguments[i].Namespace != parentArguments[i].Namespace)
						if (!childArguments[i].IsSubclassOf(parentArguments[i]))
							return false;

			return true;
		}

		#endregion

	}

}
