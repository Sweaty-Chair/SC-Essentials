using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class NoticeSubMessagePanel : MessageSubPanel
	{

		#region Variables

		[Header("Core")]
		[SerializeField] private Text _titleText = null;
		[SerializeField] private Text _contentText = null;
		[SerializeField] private Text _extraContentText = null;

		[Header("Images")]
		[SerializeField] private TemporaryImageLoader _iconImageLoader = null;
		[SerializeField] private Image _spriteImage = null;

		[Header("Buttons")]
		[SerializeField] private Text _confirmButtonText = null;

		#endregion

		#region Notice Message

		protected override void OnSet()
		{
			// Initialize our Text
			_titleText.text = _currentMessage.title;
			_contentText.text = _currentMessage.content;

			// Set all of our values
			SetText(_extraContentText, _currentMessage.extraContent);
			SetTemporaryImage(_iconImageLoader, _currentMessage.iconSpriteName);
			SetImage(_spriteImage, _currentMessage.iconSprite);

			// Then toggle our data
			SetText(_confirmButtonText, LocalizeUtils.Get(TermCategory.Button, _currentMessage.confirmButtonText), LocalizeUtils.Get(TermCategory.Button, "OK"));
		}

		#endregion

	}

}
