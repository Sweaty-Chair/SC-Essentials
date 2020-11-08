using UnityEngine;

namespace SweatyChair.UI
{

	/// <summary>
	/// A panel show info page on hint button clickk. Simply contains a hint button and info panel object.
	/// </summary>
	public class InfoHintPanel : Panel
	{

		private const string PREFS_INFO_HINT_SHOWN = "InfoHintShown"; // +path

		[Header("Info Hint")]
		[SerializeField] private GameObject _infoHintButtonGO;
		[SerializeField] private GameObject _infoHintPanelGO;
		[Tooltip("Force to show the hint panel on first time activated.")]
		[SerializeField] private bool _showOnFirstTime = true;

		public bool isInfoShown => _infoHintPanelGO.activeSelf;

		private void Awake()
		{
			if (_showOnFirstTime && !PlayerPrefs.HasKey(PREFS_INFO_HINT_SHOWN + transform.ToPath())) {
				ToggleInfo(true);
				PlayerPrefs.SetInt(PREFS_INFO_HINT_SHOWN + transform.ToPath(), 1);
			}
			ToggleInfo(false);
		}

		#region On Click

		public void OnShowInfoClick()
		{
			ToggleInfo(true);
		}

		public void OnHideInfoClick()
		{
			ToggleInfo(false);
		}

		#endregion

		public void ToggleInfo(bool show = true)
		{
			_infoHintButtonGO.SetActive(!show);
			_infoHintPanelGO.SetActive(show);
		}

	}


}