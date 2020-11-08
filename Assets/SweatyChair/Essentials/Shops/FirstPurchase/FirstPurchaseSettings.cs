using UnityEngine;

namespace SweatyChair
{

	[CreateAssetMenu(fileName = "FirstPurchaseSettings", menuName = "Sweaty Chair/Settings/First Purchase")]
	public class FirstPurchaseSettings : ScriptableObjectSingleton<FirstPurchaseSettings>
	{

		public SerializableItem[] rewards = new SerializableItem[0];
		public bool hidePanelAfterCollect;

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Sweaty Chair/Settings/First Purchase")]
		private static void OpenSettings()
		{
			current.OpenAssetsFile();
		}
#endif

	}

}