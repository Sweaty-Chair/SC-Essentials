namespace SweatyChair.InputSystem
{

	#region Axes

	public enum InputAxes
	{

		#region General Axes (Used For Event System and UI) - Phase these out over time if we do not need or use them

		Horizontal,
		Vertical,
		Tab,
		Submit,
		Cancel,

		#endregion

		#region Block42 General Axes

		Undo,
		Redo,

		Copy,
		Paste,

		#endregion

		#region Mouse Axes (Used for most camera controls)

		Mouse_X,
		Mouse_Y,
		Mouse_ScrollWheel,
		Fire1,
		Fire2,
		Fire3,
		Quick_Save,
		Reload,

		#endregion

		#region Gameplay Axes

		//Player Controls
		Jump,
		Crouch,
		Sprint,
		Fly_Up,
		Fly_Down,
		Ride_Entity,

		Toggle_Inventory,
		Toggle_Camera_View,
		Toggle_Part_Menu,
		Toggle_Console,
		Toggle_Mining,
		Toggle_Options_Menu,
		Toggle_Build_Mode,
		Toggle_Editor_Mode,
		Toggle_Plot_Editor,
		Toggle_Fly,

		//HotBar
		Select_Hotbar_1,
		Select_Hotbar_2,
		Select_Hotbar_3,
		Select_Hotbar_4,
		Select_Hotbar_5,
		Select_Hotbar_6,
		Select_Hotbar_7,
		Select_Hotbar_8,
		Select_Hotbar_9,
		Select_Hotbar_0,

		//Guns
		GunButton_1,
		GunButton_2,

		#endregion

		#region Model Editor Axes

		//Anim controller
		Anim_Rotate,
		Anim_Translate,
		Anim_Scale,

		Delete,

		Toggle_UI,
		Focus_Object

		#endregion

	}

	#endregion

	#region States

	/// <summary>
	/// Enum for reference in Inputbutton to set the state of the button
	/// </summary>
	public enum ButtonState
	{
		OnHold,
		Holding,
		OnRelease,
		Released
	}

	public enum MouseButton
	{
		LeftMouse = 0,
		RightMouse = 1,
		MiddleMouse = 2
	}

	public enum DoubleClickState
	{
		None = 0,
		DoubleClick = 1,
		WaitingForSecondClick = 2,
		MouseCancel = 3,
		Timeout = 4
	}

	public enum ClickDragState
	{
		None = 0,
		MouseDown = 1,
		WaitingForDrag = 2,
		MouseDragging = 3,
		DragEnd = 4
	}

	#endregion

}