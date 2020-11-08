using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(Text))]
	public class VersionText : MonoBehaviour
	{

		private void Awake()
		{
			GetComponent<Text>().text = Application.version;
		}

	}

}