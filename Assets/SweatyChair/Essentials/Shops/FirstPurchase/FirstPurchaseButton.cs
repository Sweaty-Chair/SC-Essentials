using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(Button))]
	public class FirstPurchaseButton : MonoBehaviour
	{

		[SerializeField] private GameObject _notificationIconGO;
		[SerializeField] private GameObject _hintGO;
		[SerializeField] private float _hintChance = 0.1f;

		private void Awake()
		{
			gameObject.SetActive(!FirstPurchaseManager.isRewardObtained);
			GetComponent<Button>().onClick.AddListener(OnClick);
			ShopManager.totalRealMoneySpentChanged += OnEnable;
			FirstPurchaseManager.rewardObtained += Awake;
		}

		private void OnDestroy()
		{
			ShopManager.totalRealMoneySpentChanged -= OnEnable;
			FirstPurchaseManager.rewardObtained -= Awake;
		}

		private void OnEnable()
		{
			if (_hintGO != null && !ShopManager.hasPaidRealMoney)
				_hintGO.SetActive(Random.value < _hintChance);
			_notificationIconGO.SetActive(ShopManager.hasPaidRealMoney);
		}

		private void OnClick()
		{
			FirstPurchaseManager.ToggleUI(true);
		}

	}

}