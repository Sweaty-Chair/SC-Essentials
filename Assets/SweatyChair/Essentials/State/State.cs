namespace SweatyChair.StateManagement
{

	// TODO: to be moved to StateSettings so no hard code is needed across different games
	public enum State
	{
		None,			// Default state
		Logo,			// Developer logos at app launch
		SplashScreen,	// Showing game splash/loading screen at app launch
		Intro,			// A story intro, normally only show once at first launch
		Tutorial,		// Playing tutorial
		Menu,			// Showing main menu
		LogIn,			// Showing login panel, for some game only
		Shop,			// Showing shop panel
		PreGame,		// State or screen before entering game, e.g. picking starting cards
		Game,			// In a game
		PauseGame,		// Pausing a game
		EndGame,		// Game end, e.g. ending score board
		Continue,		// Revive to continue
		Quitting,		// WHen quiting the app, use this to make sure no core logic is running at this stage
		Message,		// A message box shown
		Editor,			// In a editor, only for app having an editor, e.g. Model Editor in Block42
		LevelSelect,	// Selecting level, e.g. selecting a world in Block42
		LevelSave,		// Saving a level, e.g. saving a world in Block42
		Inventory,		// Inventory e.g. card collection
		InventoryDraw,	// Inventory draw e.g. card machine
		Gadget,			// Gedget e.g. tech tree
		Setting,        // Setting panel
		Leaderboards,	// In-game leaderboard
		Achievements,   // In-game achievements
		DailyTask,      // Daily tasks
		Sharing,		// Sharing panel
		HouseAd,		// House ads
	}

}