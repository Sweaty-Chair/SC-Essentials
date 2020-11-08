using UnityEngine;

namespace SweatyChair
{

	public class PrefabToImageSettings : ScriptableObjectSingleton<PrefabToImageSettings>
	{

		#region Variables

		[Header("References")]
		public GameObject prefabCameraRig = null;
		public GameObject prefabLightingRig = null;

		[Header("Settings")]
		public Layer cameraLayerMask = 1;

		#endregion

		#region Editor

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Sweaty Chair/Settings/Prefab to Image")]
		private static void OpenSettings()
		{
			current.OpenAssetsFile();
		}

#endif

		#endregion

	}

}