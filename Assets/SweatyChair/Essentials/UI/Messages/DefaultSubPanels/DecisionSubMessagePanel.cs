using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class DecisionSubMessagePanel : MessageSubPanel
	{

		#region Variables

		[Header("Core")]
		[SerializeField] private Text _titleText = null;
		[SerializeField] private Text _contentText = null;
		[SerializeField] private Text _extraContentText = null;

		[Header("Image")]
		[SerializeField] private TemporaryImageLoader _iconImageLoader = null;

		[Header("Buttons")]
		[SerializeField] private Image _firstChoiceButtonImage = null;
		[SerializeField] private TemporaryImageLoader _firstChoiceButtonIcon = null;
		[SerializeField] private Text _firstChoiceButtonText = null;
		[SerializeField] private Text _secondChoiceButtonText = null;
		[SerializeField] private Text _cancelButtonText = null;

		private Color _prevFirstChoiceButtonColor;

		#endregion

		#region Set

		protected override void OnSet()
		{
			SetText(_titleText, _currentMessage.title);
			SetText(_contentText, _currentMessage.content);
			SetText(_extraContentText, _currentMessage.extraContent);
			SetText(_firstChoiceButtonText, LocalizeUtils.Get(TermCategory.Button, _currentMessage.confirmButtonText), LocalizeUtils.Get(TermCategory.Button, "OK"));
			SetText(_secondChoiceButtonText, LocalizeUtils.Get(TermCategory.Button, _currentMessage.extraButtonText), LocalizeUtils.Get(TermCategory.Button, "Other"));
			SetText(_cancelButtonText, LocalizeUtils.Get(TermCategory.Button, _currentMessage.cancelButtonText), LocalizeUtils.Get(TermCategory.Button, "Cancel"));

			SetTemporaryImage(_iconImageLoader, _currentMessage.iconSpriteName);
			SetTemporaryImage(_firstChoiceButtonIcon, _currentMessage.confirmIconSpriteName);

			// Toggle confirm button
			if (_prevFirstChoiceButtonColor == default) _prevFirstChoiceButtonColor = _firstChoiceButtonImage.color;
			_firstChoiceButtonImage.color = _currentMessage.confirmButtonColor == default ? _prevFirstChoiceButtonColor : _currentMessage.confirmButtonColor;
		}

		#endregion

	}

}
