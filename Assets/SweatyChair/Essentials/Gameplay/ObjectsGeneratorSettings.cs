using UnityEngine;

namespace SweatyChair
{

	[CreateAssetMenu(fileName = "ObjectsGeneratorSettings", menuName = "Sweaty Chair/Settings/Object Generator")]
	public class ObjectsGeneratorSettings : ScriptableObjectSingleton<ObjectsGeneratorSettings>
	{

		public Transform defaultBulletTF;
		public float bulletSpeed = 3;

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Sweaty Chair/Settings/Object Generator")]
		private static void OpenSettings()
		{
			current.OpenAssetsFile();
		}

#endif

	}

}