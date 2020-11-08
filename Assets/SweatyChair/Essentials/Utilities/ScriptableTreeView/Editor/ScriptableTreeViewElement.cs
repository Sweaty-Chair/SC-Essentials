using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace SweatyChair.ScriptableTreeView
{

	public class ScriptableTreeViewElement : TreeViewItem
	{

		#region Variables

		public SerializedProperty objectProperty { get; protected set; }

		#endregion

		#region Constructor

		public ScriptableTreeViewElement(string name, int depth, int id) : base(id, depth, name) { }
		public ScriptableTreeViewElement(SerializedProperty property, int depth, int id) : base(id, depth, "Default")
		{
			objectProperty = property;

			// Set our object name if we are not null
			if (objectProperty != null)
				displayName = property.displayName;
		}

		#endregion

	}

}
