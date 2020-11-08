using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class RewardSubMessagePanel : MessageSubPanel
	{

		#region Variables

		[Header("Core")]
		[SerializeField] private Text _titleLabel = null;
		[SerializeField] private Text _contentLabel = null;

		[Header("Rewards")]
		[SerializeField] private GameObject _coinImageGO = null;
		[SerializeField] private Text _coinText = null;
		[Space]
		[SerializeField] private GameObject _gemImageGO = null;
		[SerializeField] private Text _gemText = null;
		[Space]
		[SerializeField] private GameObject _itemImageGO = null;
		[SerializeField] private Image _itemImage = null;

		[Header("Buttons")]
		[SerializeField] private GameObject _extraButtonGO = null;
		[SerializeField] private Text _extraButtonText = null;

		#endregion

		#region Set

		protected override void OnSet()
		{
			SetText(_titleLabel, _currentMessage.title, LocalizeUtils.Get(TermCategory.Ending, "Reward"));
			SetText(_contentLabel, _currentMessage.content, LocalizeUtils.Get(TermCategory.Shop, "Received:"));

			// TODO: Make this properly dynamically load a list of reward items which get displayed in the UI

			// Go through and just simply clear all items in our reward slot, then add all of our new data
			if (_coinImageGO != null) _coinImageGO.SetActive(false);
			if (_gemImageGO != null) _gemImageGO.SetActive(false);
			if (_itemImageGO != null) _itemImageGO.SetActive(false);

			if (_currentMessage.rewardItemList != null && _currentMessage.rewardItemList.Count > 0) {
				foreach (Item rewardItem in _currentMessage.rewardItemList)
					SetRewardItemInfo(rewardItem);
			}
			if (_currentMessage.rewardItem != null)
				SetRewardItemInfo(_currentMessage.rewardItem);

			// If we have an extra callback, show our extra button data
			if (_extraButtonGO != null)
				_extraButtonGO.SetActive(_currentMessage.extraCallback != null);
			SetText(_extraButtonText, _currentMessage.extraButtonText);

			// TODO: particle effect?
		}

		private void SetRewardItemInfo(Item rewardItem)
		{
			switch (rewardItem.type) {
				case ItemType.Coin:
					_coinText.transform.parent.gameObject.SetActive(true);
					_coinText.text = rewardItem.amount.ToString();
					break;
				case ItemType.Gem:
					_gemText.transform.parent.gameObject.SetActive(true);
					_gemText.text = rewardItem.amount.ToString();
					break;
				default:
					_itemImageGO.SetActive(rewardItem.iconSprite != null);
					_itemImage.sprite = rewardItem.iconSprite;
					break;
			}
		}

		#endregion

	}

}
