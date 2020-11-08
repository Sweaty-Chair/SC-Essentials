using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

    public static class FirstPurchaseManager
    {

		private const string GS_FIRST_PURCHASE_OBTAINED = "FirstPurchasedObtained";

        public static event UnityAction<bool> uiToggled;
		public static event UnityAction rewardObtained;

		private static FirstPurchaseSettings _settings => FirstPurchaseSettings.current;

		public static bool isRewardObtained => GameSave.GetBool(GS_FIRST_PURCHASE_OBTAINED);

		public static void ToggleUI(bool doShow = true)
		{
			uiToggled?.Invoke(doShow);
		}

		public static void Reward()
		{
			if (ShopManager.hasPaidRealMoney) {
				if (isRewardObtained) return; // Just in case
				foreach (var reward in _settings.rewards)
					reward.Obtain("first_purchase");
				GameSave.SetBool(GS_FIRST_PURCHASE_OBTAINED, true);
				rewardObtained?.Invoke();
				GameAnalytics.ClaimFirstPurchase();
			}
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Shop/Reset First Purchase")]
		private static void ResetFirstPurchase()
		{
			ShopManager.ResetRealMoneySpent();
			GameSave.DeleteKey(GS_FIRST_PURCHASE_OBTAINED);
			if (Application.isPlaying)
				Debug.Log("Restart to take effect");
		}

#endif

	}

}