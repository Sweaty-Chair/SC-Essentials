using UnityEditor.IMGUI.Controls;

namespace SweatyChair.TreeView
{

	public class TableViewItem<T> : TreeViewItem where T : TableViewElement
	{
		#region Variables

		public T data { get; set; }

		#endregion

		#region Constructor

		public TableViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
		{
			this.data = data;
		}

		#endregion

	}

}