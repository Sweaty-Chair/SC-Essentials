using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(Image))]
	public class Tab : MonoBehaviour
	{

		[SerializeField] private int _tabIndex;
		[SerializeField] private TabGroup _tabGroup;
		[SerializeField] private GameObject _targetGO;
		[SerializeField] private Color _selectedColor;

		private Image _image;
		private Color _unselectedColor;

		private void Awake()
		{
			_image = GetComponent<Image>();
			_unselectedColor = _image.color;
			_tabGroup.Register(_tabIndex, this);
		}

		public void OnSelect()
		{
			_tabGroup.DeselectAllTabs();
			_targetGO.SetActive(true);
			_image.color = _selectedColor;
		}

		public void OnDeselect()
		{
			_targetGO.SetActive(false);
			_image.color = _unselectedColor;
		}

	}

}