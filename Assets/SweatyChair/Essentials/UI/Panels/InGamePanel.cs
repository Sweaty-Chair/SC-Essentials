using System.Collections;
using UnityEngine;

namespace SweatyChair.UI
{

	/// <summary>
	/// A generic in-game panel that take care of pause and in-game functions.
	/// </summary>
	public class InGamePanel : Panel
	{

		[SerializeField] private GameObject _pausePanelGO;
		[SerializeField] private GameObject _countdownPanelGO;
#if TextMeshPro
		[SerializeField] private TMPro.TextMeshProUGUI _countdownText;
#endif

		private bool _paused;

		public override void Init()
		{
			GameManager.gameStarted += Show;
		}

		public override void Reset()
		{
			GameManager.gameStarted -= Show;
		}

		public override void OnBackClick()
		{
			_paused = !_paused;
			Pause(!_paused);
		}

		public void OnPauseClick()
		{
			Pause();
		}

		public void OnUnPauseClick()
		{
			Pause(false);
		}

		public void OnBackToMenuClick()
		{
			// Stop and delete megacool recording
			MegacoolController.StopRecording();
			MegacoolController.DeleteRecording();
			Time.timeScale = 1; // Unpause the game, so animation can be played
			GameManager.RestartGame();
		}

		public void Pause(bool pause = true)
		{
			if (pause) {
				Time.timeScale = 0;
				if (_pausePanelGO != null)
					_pausePanelGO?.SetActive(true);
				else
					PanelManager.Show<PausePanel>();
				GameManager.PauseGame();
			} else {
				if (_pausePanelGO != null)
					_pausePanelGO.SetActive(false);
				else
					PanelManager.Hide<PausePanel>();
				if (_countdownPanelGO != null)
					StartCoroutine(ShowCountdownCoroutine());
				else
					Unpause();
			}
		}

		private IEnumerator ShowCountdownCoroutine()
		{
			Time.timeScale = 0;
			_countdownPanelGO.SetActive(true);
			int i = 3;
			do {
#if TextMeshPro
				_countdownText.gameObject.SetActive(false); // Trigger the TweenOnEnable
				_countdownText.gameObject.SetActive(true);
				_countdownText.text = i.ToString();
#endif
				yield return new WaitForSecondsRealtime(0.5f);
				i--;
			} while (i > 0);
			_countdownPanelGO.SetActive(false);
			Unpause();
		}

		private void Unpause()
		{
			Time.timeScale = 1;
			GameManager.PauseGame(false);
		}

	}

}