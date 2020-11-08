namespace SweatyChair.UI
{

	/// <summary>
	/// A generic holder panel that hold a number of children panels.
	/// </summary>
	public abstract class HolderPanel : Panel
	{

		public override void Init()
		{
			base.Init();
			// Go through each of our children panels and init them
			Panel[] panels = GetComponentsInChildren<Panel>(true);
			foreach (Panel panel in panels) {
				if (panel == this) continue;
				panel.Init();
			}
		}

		public override void Reset()
		{
			base.Reset();
			// Go through each of our children panels and reset them
			Panel[] panels = GetComponentsInChildren<Panel>(true);
			foreach (Panel panel in panels) {
				if (panel == this) continue;
				panel.Reset();
			}
		}

	}

}