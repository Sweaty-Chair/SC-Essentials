using UnityEditor;
using UnityEngine;

public static class CustomEditorScopes
{

	public class HierarchyModeScope : GUI.Scope
	{
		#region Variables

		private bool _cachedHierarchyScope;

		#endregion

		#region Open / Close

		public HierarchyModeScope(bool disabled)
		{
			_cachedHierarchyScope = EditorGUIUtility.hierarchyMode;
			EditorGUIUtility.hierarchyMode = disabled;
		}

		protected override void CloseScope()
		{
			EditorGUIUtility.hierarchyMode = _cachedHierarchyScope;
		}

		#endregion

	}

	public class IndentLevelScope : GUI.Scope
	{
		#region Variables

		private int _cachedIndentLevel;

		#endregion

		#region Open / Close

		public IndentLevelScope(int indentLevel)
		{
			_cachedIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = indentLevel;
		}

		protected override void CloseScope()
		{
			EditorGUI.indentLevel = _cachedIndentLevel;
		}

		#endregion

	}


}