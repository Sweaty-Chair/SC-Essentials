using UnityEngine;
using System.Collections.Generic;

namespace SweatyChair.UI
{
	
	public class TabGroup : MonoBehaviour
	{

		[SerializeField] private int _defaultIndex = 0;

		private Dictionary<int, Tab> _tabDict = new Dictionary<int, Tab>();

		private void Start()
		{
			if (_tabDict.Count > 0)
				_tabDict[0].OnSelect();
		}

		public void Register(int index, Tab tab)
		{
			_tabDict[index] = tab;
		}

		public void DeselectAllTabs()
		{
			foreach (var kvp in _tabDict)
				kvp.Value.OnDeselect();
		}

	}

}