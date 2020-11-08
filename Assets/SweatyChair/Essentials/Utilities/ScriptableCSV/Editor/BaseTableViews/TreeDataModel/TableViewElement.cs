using System;
using System.Collections.Generic;
using UnityEngine;


namespace SweatyChair.TreeView
{

	[Serializable]
	public class TableViewElement
	{
		#region Variables

		[SerializeField] private int _id;
		[SerializeField] private string _name;
		[SerializeField] private int _depth;

		[NonSerialized] private TableViewElement _parent;
		[NonSerialized] private List<TableViewElement> _children;

		// Properties
		public int id
		{
			get { return _id; }
			set { _id = value; }
		}
		public string name
		{
			get { return _name; }
			set { _name = value; }
		}
		public int depth
		{
			get { return _depth; }
			set { _depth = value; }
		}

		public TableViewElement parent
		{
			get { return _parent; }
			set { _parent = value; }
		}
		public List<TableViewElement> children
		{
			get { return _children; }
			set { _children = value; }
		}

		public bool hasChildren => children != null && children.Count > 0;

		#endregion

		#region Constructor

		public TableViewElement()
		{
		}

		public TableViewElement(string name, int depth, int id)
		{
			_name = name;
			_id = id;
			_depth = depth;
		}

		#endregion

	}

}