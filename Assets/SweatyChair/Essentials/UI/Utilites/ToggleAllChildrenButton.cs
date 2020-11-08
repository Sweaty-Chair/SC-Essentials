using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Toogle all children on click.
namespace SweatyChair.UI
{

	[RequireComponent(typeof(Button))]
	public class ToggleAllChildrenButton : MonoBehaviour
	{

		private const string PREF_IS_TOGGLED = "ToggleAllChildrenIsToggled"; // + Name

		[SerializeField] private List<Transform> _excludedTransformList;
		[SerializeField] private RectTransform _arrowRectransform;

		[SerializeField] private int _toggledAngle = 0;
		[SerializeField] private int _untoggledAngle = 90;

		[SerializeField] private bool _defaultToggled = true;
		[SerializeField] private bool _savePlayerPrefs = true;

		private bool _isToggled;

		private void Awake()
		{
			// Add on click callback
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void Start()
		{
			if (_savePlayerPrefs)
				_isToggled = PlayerPrefs.GetInt(PREF_IS_TOGGLED + name, _defaultToggled ? 1 : 0) == 1;
			else
				_isToggled = _defaultToggled;
			Toggle();
		}

		private void OnClick()
		{
			_isToggled = !_isToggled;
			Toggle();

			// HACK: Unity doesn't update the LayoutGroup when any child changes, force change here
			HorizontalOrVerticalLayoutGroup hovlg = GetComponentInParent<HorizontalOrVerticalLayoutGroup>();
			if (hovlg != null) {
				ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
				hovlg.enabled = false;
				scrollRect.enabled = false;
				hovlg.enabled = true;
				scrollRect.enabled = true;
				Canvas.ForceUpdateCanvases();
			}
		}

		private void Toggle()
		{
			foreach (Transform child in transform) {
				if (_excludedTransformList.Contains(child))
					continue;
				child.gameObject.SetActive(_isToggled);
			}
			if (_arrowRectransform != null)
				_arrowRectransform.localEulerAngles = Vector3.forward * (_isToggled ? _toggledAngle : _untoggledAngle);
			if (_savePlayerPrefs)
				PlayerPrefs.SetInt(PREF_IS_TOGGLED + name, _isToggled ? 1 : 0);
		}

		public void Toogle(bool toggled)
		{
			_isToggled = toggled;
			Toggle();
		}

	}

}