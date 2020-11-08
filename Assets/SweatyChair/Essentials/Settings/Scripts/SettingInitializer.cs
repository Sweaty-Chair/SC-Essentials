using UnityEngine;

namespace SweatyChair
{

	public class SettingInitializer : MonoBehaviour
	{

		private void Awake()
		{
			SettingManager.Init();
		}

	}

}