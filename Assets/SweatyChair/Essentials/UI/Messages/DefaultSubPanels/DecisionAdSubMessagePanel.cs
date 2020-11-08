using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class DecisionAdSubMessagePanel : MessageSubPanel
	{

		#region Variables

		[Header("Core")]
		[SerializeField] private Text _titleText = null;
		[SerializeField] private Text _contentText = null;
		[SerializeField] private Text _extraContentText = null;


		[Header("Images")]
		[SerializeField] private Image _backgroundImage = null;
		[SerializeField] private TemporaryImageLoader _iconImageLoader = null;
		[SerializeField] private Image _iconImage = null;

		[Header("Buttons")]
		[SerializeField] private TemporaryImageLoader _confirmButtonSprite = null;
		[SerializeField] private Text _confirmButtonText = null;
		[SerializeField] private GameObject _cancelButtonGO = null;
		[SerializeField] private Text _cancelButtonText = null;

		[Header("Assorted")]
		[SerializeField] private GameObject _rewardObject = null;

		#endregion

		#region Set

		protected override void OnSet()
		{
			// Set Text
			SetText(_titleText, _currentMessage.title);
			SetText(_contentText, _currentMessage.content);
			SetText(_extraContentText, _currentMessage.extraContent);

			// Preview Icons
			SetTemporaryImage(_iconImageLoader, _currentMessage.iconSpriteName);
			if (_backgroundImage != null)
				_backgroundImage.enabled = _currentMessage.iconSprite != null;
			SetImage(_iconImage, _currentMessage.iconSprite);

			// Reward object
			if (_rewardObject != null)
				_rewardObject.SetActive(_currentMessage.rewardItem != null);

			// Confirm Button Stuff
			SetText(_confirmButtonText, _currentMessage.confirmButtonText, LocalizeUtils.Get("Collect"));
			if (_confirmButtonSprite != null)
				_confirmButtonSprite.enabled = _currentMessage.confirmButtonText != LocalizeUtils.Get("FREE");

			// Set our Cancel Button
			SetText(_cancelButtonText, _currentMessage.cancelButtonText, LocalizeUtils.Get("Nope"));
		}

		#endregion

	}

}
