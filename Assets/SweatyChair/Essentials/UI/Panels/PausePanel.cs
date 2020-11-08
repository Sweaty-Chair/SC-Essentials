namespace SweatyChair.UI
{

	/// <summary>
	/// A generic pause panel that shows while game pause.
	/// </summary>
	public class PausePanel : Panel
	{

		public override void Init()
		{
			base.Init();
			GameManager.gamePaused += Toggle;
		}

		public override void Reset()
		{
			base.Reset();
			GameManager.gamePaused -= Toggle;
		}

		public override void OnBackClick()
		{
			GameManager.PauseGame(false);
		}

	}

}