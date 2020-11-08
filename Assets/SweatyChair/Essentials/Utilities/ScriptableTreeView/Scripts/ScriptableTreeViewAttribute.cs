using System;

namespace SweatyChair.ScriptableTreeView
{

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ScriptableTreeViewAttribute : Attribute
	{

		#region Variables

		public int order { get; set; }

		public readonly string displayName;

		#endregion

		#region Constructor

		public ScriptableTreeViewAttribute() { }
		public ScriptableTreeViewAttribute(string displayName)
		{
			this.displayName = displayName;
		}

		#endregion

	}

}
