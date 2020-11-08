namespace SweatyChair.StateManagement
{

	// TODO: to be moved to StateSettings so no hard code is needed across different games
	public enum GameState
	{
		None, // Default state (in-game)
		Dialogue,
		Inventory,
		Cutscene,
		Scripting,
		PlacementSelection,
		Console,
        LoadSelection,
		PlotManagement,
        InputField,
		PhotoMode,
		Message,
		Pause,
		End
	}

}