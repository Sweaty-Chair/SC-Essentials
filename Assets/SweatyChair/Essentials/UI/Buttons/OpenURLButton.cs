using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair
{

	/// <summary>
	/// A quit game button simply quit game on click.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class OpenURLButton : MonoBehaviour
	{

		[SerializeField] private string _url;

		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		public void OnClick()
		{
			Application.OpenURL(_url);
		}

	}

}