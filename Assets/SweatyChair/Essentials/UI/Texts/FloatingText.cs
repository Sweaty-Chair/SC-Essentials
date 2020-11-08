namespace SweatyChair.UI
{

	public enum FloatingTextPosition {
		Top,
		Middle,
		Bottom
	}

	public class FloatingText
	{

		public string text { get; set; }
		public FloatingTextPosition position { get; set; }
		public float duration { get; set; }

		public void Show()
		{
			FloatingTextManager.Show(this);
		}

		public void ShowOnTop()
		{
			position = FloatingTextPosition.Top;
			Show();
		}

	}

}