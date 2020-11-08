using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class ConfirmSubMessagePanel : MessageSubPanel
	{


		#region Variables

		[Header("Core")]
		[SerializeField] private Text _titleText = null;
		[SerializeField] private Text _contentText = null;
		[SerializeField] private Text _extraContentText = null;

		[Header("Images")]
		[SerializeField] private TemporaryImageLoader _iconImage = null;

		[Header("Buttons")]
		[SerializeField] private Image _confirmButtonImage = null;
		[SerializeField] private Text _confirmButtonLabel = null;
		[SerializeField] private Text _cancelButtonLabel = null;

		private Color _confirmPrevConfirmButtonColor;

		#endregion

		#region Set

		protected override void OnSet()
		{
			// Set our Texts
			SetText(_titleText, _currentMessage.title);
			SetText(_contentText, _currentMessage.content);
			SetText(_extraContentText, _currentMessage.extraContent);

			// Button Labels
			SetText(_confirmButtonLabel, LocalizeUtils.Get(TermCategory.Button, _currentMessage.confirmButtonText), LocalizeUtils.Get(TermCategory.Button, "OK"));
			SetText(_cancelButtonLabel, LocalizeUtils.Get(TermCategory.Button, _currentMessage.cancelButtonText), LocalizeUtils.Get(TermCategory.Button, "Cancel"));

			// Icon
			SetTemporaryImage(_iconImage, _currentMessage.iconSprite, _currentMessage.iconSpriteName);

			// Confirm Button Colour stuff
			if (_confirmPrevConfirmButtonColor == default) _confirmPrevConfirmButtonColor = _confirmButtonImage.color;
			_confirmButtonImage.color = _currentMessage.confirmButtonColor == default ? _confirmPrevConfirmButtonColor : _currentMessage.confirmButtonColor;
		}

		#endregion

	}

}
