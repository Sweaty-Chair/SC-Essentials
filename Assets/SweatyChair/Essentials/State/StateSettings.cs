using UnityEngine;

namespace SweatyChair
{

	[CreateAssetMenu(fileName = "StateSettings", menuName = "Sweaty Chair/Settings/State")]
	public class StateSettings : ScriptableObjectSingleton<StateSettings>
	{
		public bool debugMode;

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Sweaty Chair/Settings/State")]
		private static void OpenSettings()
		{
			current.OpenAssetsFile();
		}
#endif

	}

}