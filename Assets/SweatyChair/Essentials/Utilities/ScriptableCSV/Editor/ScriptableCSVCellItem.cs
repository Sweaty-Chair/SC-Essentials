using SweatyChair.TreeView;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SweatyChair.ScriptableCSV
{

	[Serializable]
	public class ScriptableCSVCellItem : TableViewElement
	{

		#region Variables

		public ScriptableObject scriptableObject;
		public SerializedObject serializedObject;

		private Dictionary<string, SerializedProperty> _propertyDict = new Dictionary<string, SerializedProperty>();

		#endregion

		#region Constructor

		public ScriptableCSVCellItem(string name, int depth, int id) : base(name, depth, id) { }
		public ScriptableCSVCellItem(ScriptableObject scriptableObject, int depth, int id) : base("DefaultName", depth, id)
		{
			// Initialize our data
			this.scriptableObject = scriptableObject;
			serializedObject = new SerializedObject(scriptableObject);

			// Set our name if we are not null
			if (scriptableObject != null)
				name = scriptableObject.name;

			// Go through all of our Properties and create dictionary of serialized properties and our object data
			_propertyDict = new Dictionary<string, SerializedProperty>();
		}

		#endregion

		#region Get Property

		public SerializedProperty GetSerializedProperty(string propertyName)
		{
			// If our Object is null, return a null property
			if (serializedObject == null)
				return null;

			if (_propertyDict.TryGetValue(propertyName, out SerializedProperty prop))
				return prop;
			else {
				// Get our property
				SerializedProperty newProperty = serializedObject.FindProperty(propertyName);

				// if we are not null, add it to our dictionary so we can get it later
				if (newProperty != null)
					_propertyDict.Add(propertyName, newProperty);

				// Return
				return newProperty;
			}
		}

		#endregion

	}
}