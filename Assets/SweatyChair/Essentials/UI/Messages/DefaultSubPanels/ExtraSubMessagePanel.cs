using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class ExtraSubMessagePanel : MessageSubPanel
	{

		#region Variables

		[Header("Core")]
		[SerializeField] private Text _titleText = null;
		[SerializeField] private Text _contentText = null;
		[SerializeField] private Text _extraContentText = null;
		[SerializeField] private Text _extraContent2Text = null;

		[Header("Images")]
		[SerializeField] private TemporaryImageLoader _iconImageLoader = null;

		[Header("Assorted")]
		[SerializeField] private GameObject _rewardObject = null;

		#endregion

		#region Set

		protected override void OnSet()
		{
			SetText(_titleText, _currentMessage.title);
			SetText(_contentText, _currentMessage.content);
			SetText(_extraContentText, _currentMessage.extraContent);
			SetText(_extraContent2Text, _currentMessage.extraContent2);
			SetTemporaryImage(_iconImageLoader, _currentMessage.iconSprite, _currentMessage.iconSpriteName);


			_rewardObject.gameObject.SetActive(_currentMessage.rewardItem != null);
		}

		#endregion

	}

}
