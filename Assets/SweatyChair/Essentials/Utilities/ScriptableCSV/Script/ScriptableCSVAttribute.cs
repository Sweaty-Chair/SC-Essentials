using System;

namespace SweatyChair.ScriptableCSV
{

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ScriptableCSVAttribute : Attribute
	{

		#region Variables

		public int order { get; set; }

		public readonly string displayName;
		public readonly bool showAddRemove = false;

		#endregion

		#region Constructor

		public ScriptableCSVAttribute() { }
		public ScriptableCSVAttribute(string displayName)
		{
			this.displayName = displayName;
		}
		public ScriptableCSVAttribute(string displayName, bool showAddRemove) : this(displayName)
		{
			this.showAddRemove = showAddRemove;
		}

		#endregion

	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class HideInScriptableCSVAttribute : Attribute
	{

		#region Constructor

		public HideInScriptableCSVAttribute() { }

		#endregion

	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ScriptableCSVColumnAttribute : Attribute
	{

		#region Variables

		public readonly bool allowToggleVisibility = true;
		public readonly bool allowSorting = true;
		public readonly float minWidth = 20f;
		public readonly float maxWidth = 1000000f;

		#endregion

		#region Constructor

		public ScriptableCSVColumnAttribute() { }

		public ScriptableCSVColumnAttribute(float minWidth, float maxWidth)
		{
			this.minWidth = minWidth;
			this.maxWidth = maxWidth;
		}

		public ScriptableCSVColumnAttribute(bool allowToggleVisibility, bool allowSorting)
		{
			this.allowToggleVisibility = allowToggleVisibility;
			this.allowSorting = allowSorting;
		}

		public ScriptableCSVColumnAttribute(bool allowToggleVisibility, bool allowSorting, float minWidth, float maxWidth)
		{
			this.allowToggleVisibility = allowToggleVisibility;
			this.allowSorting = allowSorting;
			this.minWidth = minWidth;
			this.maxWidth = maxWidth;
		}

		#endregion

	}

}
