using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(Button))]
	public class SettingButton : UIBehaviour
	{

		protected override void Awake()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		public void OnClick()
		{
			SettingManager.ToggleUI();
		}

	}

}