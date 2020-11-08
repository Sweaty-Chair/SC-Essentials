using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair
{

	/// <summary>
	/// A quit game button simply quit game on click.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class QuitGameButton : MonoBehaviour
	{

		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(QuitGame);
		}

		private void QuitGame()
		{
			Application.Quit();
		}

	}

}