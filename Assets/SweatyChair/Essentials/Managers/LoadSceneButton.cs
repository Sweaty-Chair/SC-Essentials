using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SweatyChair.UI
{

	/// <summary>
	/// Button to load a scene.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class LoadSceneButton : UIBehaviour
	{

		[SerializeField] private string _sceneName;

		protected override void Awake()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		public void OnClick()
		{
			GameSceneManager.LoadScene(_sceneName);
		}

	}

}