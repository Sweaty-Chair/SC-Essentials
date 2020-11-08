using UnityEngine;

namespace SweatyChair
{
	
	[CreateAssetMenu(fileName = "BuildToolSettings", menuName = "Sweaty Chair/Build Tool/Settings")]
    public class BuildToolSettings : ScriptableObjectSingleton<BuildToolSettings>
	{

		public BuildToolBuildInfo[] buildInfos = new BuildToolBuildInfo[1];

		public bool setProductName = false;
		public bool setBundleId = false;
		public bool setDefineSymbols = true;

		public string keystorePassword = "Apple123!";
		public string keyAliasPassword = "Orange123!";

		public bool FindBuildInfo(string value, out BuildToolBuildInfo result)
		{
			foreach (var buildInfo in buildInfos) {
				if (buildInfo.name.Contains(value)) {
					result = buildInfo;
					return true;
				}
			}
			Debug.LogErrorFormat("Cannot find build info with '{0}'", value);
			result = new BuildToolBuildInfo();
			return false;
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Sweaty Chair/Build Tool/Settings")]
		public static void OpenSettings()
		{
			current.OpenAssetsFile();
		}

#endif

	}

	[System.Serializable]
	public struct BuildToolBuildInfo
	{
		public string name;
		public string productName;
		public string bundleId;
		public string defineSymbols;
	}

}