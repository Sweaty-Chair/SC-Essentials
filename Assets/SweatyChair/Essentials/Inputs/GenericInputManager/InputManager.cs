using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair.InputSystem
{

	/// <summary>
	/// A Custom input manager, which acts as a wrapper for UnityEngine.Input. Allows for getting input changes as events.
	/// </summary>
	public class InputManager : PresetSingleton<InputManager>
	{

#if CROSS_PLATFORM_INPUT

		#region Variables

		// Buttons
		private Dictionary<string, InputButton> _buttonDictionary = new Dictionary<string, InputButton>(); // Dictionary to hold all our button input data
		private List<string> _buttonKeyIndex = new List<string>(); // List to store all the keys in our dictionary to use for indexing

		// Axises
		private Dictionary<string, InputAxis> _axisDictionary = new Dictionary<string, InputAxis>(); // Dictionary to hold all our Axis input data
		private List<string> _axisKeyIndex = new List<string>(); // List to store all the keys in our dictionary to use for indexing

		// Click and Drag
		private Dictionary<MouseButton, InputDrag> _dragDictionary = new Dictionary<MouseButton, InputDrag>();
		private List<MouseButton> _dragKeyIndex = new List<MouseButton>();

		// DoubleClick
		private DoubleClickState _dblClickState;
		private float _dblClickTimeThreshold = 0.5f;
		private float _lastClickTime = -1;
		private bool _inClickCheck => _lastClickTime != -1;

		#endregion

		#region Init

		protected override void Init()
		{
			// Initialize lists and data
			_buttonDictionary = new Dictionary<string, InputButton>();
			_buttonKeyIndex = new List<string>();

			_axisDictionary = new Dictionary<string, InputAxis>();
			_axisKeyIndex = new List<string>();

			_dragDictionary = new Dictionary<MouseButton, InputDrag>();
			_dragKeyIndex = new List<MouseButton>();
		}

		#endregion

		#region Update

		private void Update()
		{
			RefreshButtonStates();
			RefreshAxisStates();
			RefreshDragStates();
			UpdateDoubleClickState();
		}

		/// <summary>
		/// Refreshes all the button states to read the correct input when needing to be polled
		/// </summary>
		private void RefreshButtonStates()
		{
			for (int i = 0; i < _buttonKeyIndex.Count; i++) { // Increment through all our dictionary keys
				string buttonKey = _buttonKeyIndex[i];
				if (_buttonDictionary.ContainsKey(buttonKey))
					_buttonDictionary[buttonKey].RefreshState(); // And refresh thier input states
			}
		}

		/// <summary>
		/// Refreshes all the axis states to read the correct input when needing to be polled
		/// </summary>
		private void RefreshAxisStates()
		{
			for (int i = 0; i < _axisKeyIndex.Count; i++) { // Increment through all our dictionary keys
				string axisKey = _axisKeyIndex[i];
				if (_axisDictionary.ContainsKey(axisKey))
					_axisDictionary[axisKey].RefreshState(); // And refresh thier input states
			}
		}

		private void RefreshDragStates()
		{
			for (int i = 0; i < _dragKeyIndex.Count; i++) {
				MouseButton dragKey = _dragKeyIndex[i];
				if (_dragDictionary.ContainsKey(dragKey))
					_dragDictionary[dragKey].RefreshState();
			}
		}

		#endregion

		#region Button Methods

		#region Add Button

		private bool AddButton(string buttonName)
		{
			if (InputUtils.IsButtonAvailable(buttonName)) {
				_buttonKeyIndex.Add(buttonName); // Add the key to our indexing list
				_buttonDictionary.Add(buttonName, new InputButton(buttonName)); // Then add a new input button to our dictionary
				return true;

			} else {
				Debug.LogFormat("[InputManager] : Unable to add button '{0}' to input manager. As it does not seem to exist in ProjectSettings>Input.", buttonName);
				return false;
			}
		}

		#endregion

		#region Disable Button

		public static void SetButtonDisabled(InputAxes axes, bool isDisabled)
		{
			SetButtonDisabled(InputManagerUtility.AxesNameFromEnum(axes), isDisabled);
		}

		public static void SetButtonDisabled(string buttonName, bool isDisabled)
		{
			instance?.SetButtonDisabledInternal(buttonName, isDisabled);
		}

		private void SetButtonDisabledInternal(string buttonName, bool isDisabled)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return;
			}
			_buttonDictionary[buttonName].DisableButton(isDisabled);
		}

		#endregion

		#region Get Button State

		public static ButtonState GetButtonState(InputAxes axes)
		{
			return GetButtonState(InputManagerUtility.AxesNameFromEnum(axes));
		}

		public static ButtonState GetButtonState(string buttonName)
		{
			return instanceExists ? instance.GetButtonStateInternal(buttonName) : ButtonState.Released;
		}

		private ButtonState GetButtonStateInternal(string buttonName)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) { return ButtonState.Released; }
			}
			return _buttonDictionary[buttonName].GetButtonState();
		}

		#endregion

		#region Get Button GameData

		public static bool GetButtonData(InputAxes button, out InputButton outButton)
		{
			return GetButtonData(InputManagerUtility.AxesNameFromEnum(button), out outButton);
		}

		public static bool GetButtonData(string buttonName, out InputButton outButton)
		{
			outButton = null;
			return instanceExists && instance.GetButtonDataInternal(buttonName, out outButton);
		}

		private bool GetButtonDataInternal(string buttonName, out InputButton outButton)
		{
			outButton = null;
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) { return false; }
			}
			outButton = _buttonDictionary[buttonName];
			return true;
		}

		#endregion

		#region Get Button

		public static bool GetButton(InputAxes button)
		{
			return GetButton(InputManagerUtility.AxesNameFromEnum(button));
		}
		public static bool GetButton(string buttonName)
		{
			return instanceExists && instance.GetButtonInternal(buttonName);
		}

		private bool GetButtonInternal(string buttonName)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) { return false; }
			}
			return _buttonDictionary[buttonName].GetButton();
		}

		#endregion

		#region Get Button Down

		public static bool GetButtonDown(InputAxes button)
		{
			return GetButtonDown(InputManagerUtility.AxesNameFromEnum(button));
		}

		public static bool GetButtonDown(string buttonName)
		{
			return instanceExists && instance.GetButtonDownInternal(buttonName);
		}

		private bool GetButtonDownInternal(string buttonName)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return false;
			}
			return _buttonDictionary[buttonName].GetButtonDown();
		}

		#endregion

		#region Get Button Up

		public static bool GetButtonUp(InputAxes button)
		{
			return GetButtonUp(InputManagerUtility.AxesNameFromEnum(button));
		}

		public static bool GetButtonUp(string buttonName)
		{
			return instanceExists && instance.GetButtonUpInternal(buttonName);
		}

		private bool GetButtonUpInternal(string buttonName)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return false;
			}
			return _buttonDictionary[buttonName].GetButtonUp();
		}

		#endregion

		#region Get Hold Time

		public static float GetButtonHoldTime(InputAxes axes)
		{
			return GetButtonHoldTime(InputManagerUtility.AxesNameFromEnum(axes));
		}

		public static float GetButtonHoldTime(string buttonName)
		{
			return instanceExists ? instance.GetButtonHoldTimeInternal(buttonName) : 0;
		}

		private float GetButtonHoldTimeInternal(string buttonName)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return 0;
			}
			return _buttonDictionary[buttonName].GetHoldTime();
		}

		#endregion

		#region Get Unscaled Hold Time

		public static float GetButtonUnscaledHoldTime(InputAxes axes)
		{
			return GetButtonUnscaledHoldTime(InputManagerUtility.AxesNameFromEnum(axes));
		}

		public static float GetButtonUnscaledHoldTime(string buttonName)
		{
			return instanceExists ? instance.GetButtonUnscaledHoldTimeInternal(buttonName) : 0;
		}

		private float GetButtonUnscaledHoldTimeInternal(string buttonName)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return 0;
			}
			return _buttonDictionary[buttonName].GetUnscaledHoldTime();
		}

		#endregion

		#region Add Listeners

		#region Button State Change

		public static void AddListenerToButtonState(InputAxes button, UnityAction<ButtonState> action)
		{
			AddListenerToButtonState(InputManagerUtility.AxesNameFromEnum(button), action);
		}

		public static void AddListenerToButtonState(string buttonName, UnityAction<ButtonState> action)
		{
			instance?.AddListenerToButtonStateInternal(buttonName, action);
		}

		private void AddListenerToButtonStateInternal(string buttonName, UnityAction<ButtonState> action)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return;
			}
			_buttonDictionary[buttonName].AddStateChangeListener(action);
		}

		public static void RemoveListenerFromButtonState(InputAxes button, UnityAction<ButtonState> action)
		{
			RemoveListenerFromButtonState(InputManagerUtility.AxesNameFromEnum(button), action);
		}

		public static void RemoveListenerFromButtonState(string buttonName, UnityAction<ButtonState> action)
		{
			instance?.RemoveListenerFromButtonStateInternal(buttonName, action);
		}

		private void RemoveListenerFromButtonStateInternal(string buttonName, UnityAction<ButtonState> action)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return;
			}
			_buttonDictionary[buttonName].RemoveStateChangeListener(action);
		}

		#endregion

		#region Button Value Change

		public static void AddListenerToButtonValue(InputAxes button, UnityAction<bool> action)
		{
			AddListenerToButtonValue(InputManagerUtility.AxesNameFromEnum(button), action);
		}

		public static void AddListenerToButtonValue(string buttonName, UnityAction<bool> action)
		{
			instance?.AddListenerToButtonValueInternal(buttonName, action);
		}

		private void AddListenerToButtonValueInternal(string buttonName, UnityAction<bool> action)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return;
			}

			_buttonDictionary[buttonName].AddValueChangeListener(action);
		}

		public static void RemoveListenerFromButtonValue(InputAxes button, UnityAction<bool> action)
		{
			RemoveListenerFromButtonValue(InputManagerUtility.AxesNameFromEnum(button), action);
		}

		public static void RemoveListenerFromButtonValue(string buttonName, UnityAction<bool> action)
		{
			instance?.RemoveListenerFromButtonValueInternal(buttonName, action);
		}

		private void RemoveListenerFromButtonValueInternal(string buttonName, UnityAction<bool> action)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return;
			}
			_buttonDictionary[buttonName].RemoveValueChangeListener(action);
		}

		#endregion

		#region Button Down

		public static void AddListenerToButtonDown(InputAxes button, UnityAction action)
		{
			AddListenerToButtonDown(InputManagerUtility.AxesNameFromEnum(button), action);
		}

		public static void AddListenerToButtonDown(string buttonName, UnityAction action)
		{
			instance?.AddListenerToButtonDownInternal(buttonName, action);
		}

		private void AddListenerToButtonDownInternal(string buttonName, UnityAction action)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return;
			}
			_buttonDictionary[buttonName].AddButtonDownListener(action);
		}


		public static void RemoveListenerFromButtonDown(InputAxes button, UnityAction action)
		{
			RemoveListenerFromButtonDown(InputManagerUtility.AxesNameFromEnum(button), action);
		}

		public static void RemoveListenerFromButtonDown(string buttonName, UnityAction action)
		{
			instance?.RemoveListenerFromButtonDownInternal(buttonName, action);
		}

		private void RemoveListenerFromButtonDownInternal(string buttonName, UnityAction action)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return;
			}
			_buttonDictionary[buttonName].RemoveButtonDownListener(action);
		}

		#endregion

		#region Button Up

		public static void AddListenerToButtonUp(InputAxes button, UnityAction action)
		{
			AddListenerToButtonUp(InputManagerUtility.AxesNameFromEnum(button), action);
		}

		public static void AddListenerToButtonUp(string buttonName, UnityAction action)
		{
			instance?.AddListenerToButtonUpInternal(buttonName, action);
		}

		private void AddListenerToButtonUpInternal(string buttonName, UnityAction action)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return;
			}
			_buttonDictionary[buttonName].AddButtonUpListener(action);
		}

		public static void RemoveListenerFromButtonUp(InputAxes button, UnityAction action)
		{
			RemoveListenerFromButtonUp(InputManagerUtility.AxesNameFromEnum(button), action);
		}

		public static void RemoveListenerFromButtonUp(string buttonName, UnityAction action)
		{
			instance?.RemoveListenerFromButtonUpInternal(buttonName, action);
		}

		private void RemoveListenerFromButtonUpInternal(string buttonName, UnityAction action)
		{
			if (!_buttonDictionary.ContainsKey(buttonName)) { // If we dont have a reference to the button already, create one
				bool succeed = AddButton(buttonName);
				if (!succeed) return;
			}
			_buttonDictionary[buttonName].RemoveButtonUpListener(action);
		}

		#endregion

		#endregion

		#endregion

		#region Axis Methods

		#region Add Axis

		private bool CreateAxisIfNonExistent(string axisName)
		{
			bool succeed = true;
			if (!_axisDictionary.ContainsKey(axisName)) // If we dont have a reference to the axis already, create one
				succeed = AddAxis(axisName);
			return succeed;
		}

		private bool AddAxis(string axisName)
		{
			if (InputUtils.IsAxisAvailable(axisName)) {
				_axisKeyIndex.Add(axisName); // Add the key to our indexing list
				_axisDictionary.Add(axisName, new InputAxis(axisName)); // Then add a new input axis to our dictionary
				return true;
			}
			Debug.LogFormat("[InputManager] : Unable to add axis '{0}' to input manager. As it does not seem to exist in ProjectSettings>Input.", axisName);
			return false;
		}

		#endregion

		#region Disable Axis

		public static void SetAxisDisabled(InputAxes axis, bool isDisabled)
		{
			SetAxisDisabled(InputManagerUtility.AxesNameFromEnum(axis), isDisabled);
		}

		public static void SetAxisDisabled(string axisName, bool isDisabled)
		{
			instance?.SetAxisDisabledInternal(axisName, isDisabled);
		}

		private void SetAxisDisabledInternal(string axisName, bool isDisabled)
		{
			if (!_axisDictionary.ContainsKey(axisName)) { // If we dont have a reference to the axis already, create one
				bool succeed = AddAxis(axisName);
				if (!succeed) return;
			}
			_axisDictionary[axisName].DisableAxis(isDisabled);
		}

		#endregion

		#region Get Axis / Get Axis Delta

		/// <summary>
		/// Returns the deslta value from last frame for a given axis.
		/// </summary>
		public static float GetAxis(InputAxes axis)
		{
			return GetAxis(InputManagerUtility.AxesNameFromEnum(axis));
		}

		public static float GetAxis(string axisName)
		{
			return instanceExists ? instance.GetAxisInternal(axisName) : 0;
		}

		private float GetAxisInternal(string axisName)
		{
			bool succeed = CreateAxisIfNonExistent(axisName);
			if (!succeed) return 0;
			return _axisDictionary[axisName].GetAxis();
		}

		// Delta
		public static float GetAxisDelta(InputAxes axis)
		{
			return GetAxisDelta(InputManagerUtility.AxesNameFromEnum(axis));
		}

		public static float GetAxisDelta(string axisName)
		{
			return instanceExists ? instance.GetAxisDeltaInternal(axisName) : 0;
		}

		private float GetAxisDeltaInternal(string axisName)
		{
			bool succeed = CreateAxisIfNonExistent(axisName);
			if (!succeed) { return 0; }
			return _axisDictionary[axisName].GetAxisDelta();
		}

		#endregion

		#region Get Axis Raw / Get Axis Raw delta

		public static float GetAxisRaw(InputAxes axis)
		{
			return GetAxisRaw(InputManagerUtility.AxesNameFromEnum(axis));
		}
		public static float GetAxisRaw(string axisName)
		{
			return instanceExists ? instance.GetAxisRawInternal(axisName) : 0;
		}

		private float GetAxisRawInternal(string axisName)
		{
			bool succeed = CreateAxisIfNonExistent(axisName);
			if (!succeed) { return 0; }
			return _axisDictionary[axisName].GetAxisRaw();
		}

		// Delta
		public static float GetAxisRawDelta(InputAxes axis)
		{
			return GetAxisRawDelta(InputManagerUtility.AxesNameFromEnum(axis));
		}

		public static float GetAxisRawDelta(string axisName)
		{
			return instanceExists ? instance.GetAxisRawDeltaInternal(axisName) : 0;
		}

		private float GetAxisRawDeltaInternal(string axisName)
		{
			bool succeed = CreateAxisIfNonExistent(axisName);
			if (!succeed) { return 0; }
			return _axisDictionary[axisName].GetAxisRawDelta();
		}

		#endregion

		#region Listen To Axis Value

		#region Axis Change

		public static void AddListenerToAxisValueChange(InputAxes axis, UnityAction<float> action)
		{
			AddListenerToAxisValueChange(InputManagerUtility.AxesNameFromEnum(axis), action);
		}

		public static void AddListenerToAxisValueChange(string axisName, UnityAction<float> action)
		{
			instance?.AddListenerToAxisValueChangeInternal(axisName, action);
		}

		private void AddListenerToAxisValueChangeInternal(string axisName, UnityAction<float> action)
		{
			if (!_axisDictionary.ContainsKey(axisName)) { // f we dont have a reference to the axis already, create one
				bool succeed = AddAxis(axisName);
				if (!succeed) return;
			}
			_axisDictionary[axisName].AddAxisChangeListener(action);
		}

		public static void RemoveListenerFromAxisValueChange(InputAxes axis, UnityAction<float> action)
		{
			RemoveListenerFromAxisValueChange(InputManagerUtility.AxesNameFromEnum(axis), action);
		}

		public static void RemoveListenerFromAxisValueChange(string axisName, UnityAction<float> action)
		{
			instance?.RemoveListenerFromAxisValueChangeInternal(axisName, action);
		}

		private void RemoveListenerFromAxisValueChangeInternal(string axisName, UnityAction<float> action)
		{
			if (!_axisDictionary.ContainsKey(axisName)) { // If we dont have a reference to the axis already, create one
				bool succeed = AddAxis(axisName);
				if (!succeed) return;
			}
			_axisDictionary[axisName].RemoveAxisChangeListener(action);
		}

		#endregion

		#region Axis

		public static void AddListenerToAxisValue(InputAxes axis, UnityAction<float> action)
		{
			AddListenerToAxisValue(InputManagerUtility.AxesNameFromEnum(axis), action);
		}

		public static void AddListenerToAxisValue(string axisName, UnityAction<float> action)
		{
			instance?.AddListenerToAxisValueInternal(axisName, action);
		}

		private void AddListenerToAxisValueInternal(string axisName, UnityAction<float> action)
		{
			if (!_axisDictionary.ContainsKey(axisName)) { // If we dont have a reference to the axis already, create one
				bool succeed = AddAxis(axisName);
				if (!succeed) return;
			}
			_axisDictionary[axisName].AddAxisListener(action);
		}

		public static void RemoveListenerFromAxisValue(InputAxes axis, UnityAction<float> action)
		{
			RemoveListenerFromAxisValue(InputManagerUtility.AxesNameFromEnum(axis), action);
		}

		public static void RemoveListenerFromAxisValue(string axisName, UnityAction<float> action)
		{
			instance?.RemoveListenerFromAxisValueInternal(axisName, action);
		}

		private void RemoveListenerFromAxisValueInternal(string axisName, UnityAction<float> action)
		{
			if (!_axisDictionary.ContainsKey(axisName)) { // If we dont have a reference to the axis already, create one
				bool succeed = AddAxis(axisName);
				if (!succeed) return;
			}
			_axisDictionary[axisName].RemoveAxisListener(action);
		}

		#endregion

		#region Axis Raw Change

		public static void AddListenerToRawAxisValueChange(InputAxes axis, UnityAction<float> action)
		{
			AddListenerToRawAxisValueChange(InputManagerUtility.AxesNameFromEnum(axis), action);
		}

		public static void AddListenerToRawAxisValueChange(string axisName, UnityAction<float> action)
		{
			instance?.AddListenerToRawAxisValueChangeInternal(axisName, action);
		}

		private void AddListenerToRawAxisValueChangeInternal(string axisName, UnityAction<float> action)
		{
			if (!_axisDictionary.ContainsKey(axisName)) { // If we dont have a reference to the axis already, create one
				bool succeed = AddAxis(axisName);
				if (!succeed) return;
			}
			_axisDictionary[axisName].AddRawAxisChangeListener(action);
		}

		public static void RemoveListenerFromRawAxisValueChange(InputAxes axis, UnityAction<float> action)
		{
			RemoveListenerFromRawAxisValueChange(InputManagerUtility.AxesNameFromEnum(axis), action);
		}

		public static void RemoveListenerFromRawAxisValueChange(string axisName, UnityAction<float> action)
		{
			instance?.RemoveListenerFromRawAxisValueChangeInternal(axisName, action);
		}

		private void RemoveListenerFromRawAxisValueChangeInternal(string axisName, UnityAction<float> action)
		{
			if (!_axisDictionary.ContainsKey(axisName)) { // If we dont have a reference to the axis already, create one
				bool succeed = AddAxis(axisName);
				if (!succeed) return;
			}
			_axisDictionary[axisName].RemoveRawAxisChangeListener(action);
		}

		#endregion

		#region Axis Raw

		public static void AddListenerToRawAxisValue(InputAxes axis, UnityAction<float> action)
		{
			AddListenerToRawAxisValue(InputManagerUtility.AxesNameFromEnum(axis), action);
		}

		public static void AddListenerToRawAxisValue(string axisName, UnityAction<float> action)
		{
			instance?.AddListenerToRawAxisValueInternal(axisName, action);
		}

		private void AddListenerToRawAxisValueInternal(string axisName, UnityAction<float> action)
		{
			if (!_axisDictionary.ContainsKey(axisName)) { // If we dont have a reference to the axis already, create one
				bool succeed = AddAxis(axisName);
				if (!succeed) return;
			}
			_axisDictionary[axisName].AddRawAxisListener(action);
		}

		public static void RemoveListenerFromRawAxisValue(InputAxes axis, UnityAction<float> action)
		{
			RemoveListenerFromRawAxisValue(InputManagerUtility.AxesNameFromEnum(axis), action);
		}

		public static void RemoveListenerFromRawAxisValue(string axisName, UnityAction<float> action)
		{
			instance?.RemoveListenerFromRawAxisValueInternal(axisName, action);
		}

		private void RemoveListenerFromRawAxisValueInternal(string axisName, UnityAction<float> action)
		{
			if (!_axisDictionary.ContainsKey(axisName)) { // If we dont have a reference to the axis already, create one
				bool succeed = AddAxis(axisName);
				if (!succeed) return;
			}
			_axisDictionary[axisName].RemoveRawAxisListener(action);
		}

		#endregion

		#endregion

		#endregion

		#region Mouse Input (Dragging, Clicks and Doubleclicks)

		#region Drag Methods

		#region Add Drag

		private bool AddDrag(MouseButton mouseButton)
		{
			_dragKeyIndex.Add(mouseButton); // Add the key to our indexing list
			_dragDictionary.Add(mouseButton, new InputDrag(mouseButton)); // Then add a new input button to our dictionary
			return true;
		}

		#endregion

		#region Disable Drag

		public static void SetDragDisabled(MouseButton mouseButton, bool isDisabled)
		{
			instance?.SetDragDisabledInternal(mouseButton, isDisabled);
		}

		private void SetDragDisabledInternal(MouseButton mouseButton, bool isDisabled)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the axis already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) return;
			}
			_dragDictionary[mouseButton].DisableDrag(isDisabled);
		}

		#endregion

		#region Get Drag State

		public static ClickDragState GetDragState(MouseButton mouseButton)
		{
			return instanceExists ? instance.GetDragStateInternal(mouseButton) : ClickDragState.None;
		}

		private ClickDragState GetDragStateInternal(MouseButton mouseButton)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the button already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) { return ClickDragState.None; }
			}
			return _dragDictionary[mouseButton].GetDragState();
		}

		#endregion

		#region Get Drag GameData

		public static InputDragPayload GetDragData(MouseButton mouseButton)
		{
			return instanceExists ? instance.GetDragDataInternal(mouseButton) : new InputDragPayload();
		}

		private InputDragPayload GetDragDataInternal(MouseButton mouseButton)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the button already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) { return new InputDragPayload(); }
			}
			return _dragDictionary[mouseButton].GetDragData();
		}

		#endregion

		#region Get isDragging

		public static bool GetIsDragging(MouseButton mouseButton)
		{
			return instanceExists && instance.GetIsDraggingInternal(mouseButton);
		}

		private bool GetIsDraggingInternal(MouseButton mouseButton)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the button already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) return false;
			}
			return _dragDictionary[mouseButton].IsDragging();
		}

		#endregion

		#region Add Listeners

		#region Potential Drag

		public static void AddListenerToPotentialDrag(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			instance?.AddListenerToPotentialDragInternal(mouseButton, action);
		}

		private void AddListenerToPotentialDragInternal(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the button already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) return;
			}
			_dragDictionary[mouseButton].AddPotentialDragListener(action);
		}

		public static void RemoveListenerFromPotentialDrag(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			instance?.RemoveListenerFromPotentialDragInternal(mouseButton, action);
		}

		private void RemoveListenerFromPotentialDragInternal(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the button already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) return;
			}
			_dragDictionary[mouseButton].RemovePotentialDragListener(action);
		}

		#endregion

		#region Dragging Update

		public static void AddListenerToDrag(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			instance?.AddListenerToDragInternal(mouseButton, action);
		}

		private void AddListenerToDragInternal(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the button already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) return;
			}
			_dragDictionary[mouseButton].AddDraggingListener(action);
		}

		public static void RemoveListenerFromDrag(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			instance?.RemoveListenerFromDragInternal(mouseButton, action);
		}

		private void RemoveListenerFromDragInternal(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the button already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) return;
			}
			_dragDictionary[mouseButton].RemoveDraggingListener(action);
		}

		#endregion

		#region Dragging End

		public static void AddListenerToDragEnd(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			instance?.AddListenerToDragEndInternal(mouseButton, action);
		}

		private void AddListenerToDragEndInternal(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the button already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) return;
			}
			_dragDictionary[mouseButton].AddDragEndListener(action);
		}

		public static void RemoveListenerFromDragEnd(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			instance?.RemoveListenerFromDragEndInternal(mouseButton, action);
		}

		private void RemoveListenerFromDragEndInternal(MouseButton mouseButton, UnityAction<InputDragPayload> action)
		{
			if (!_dragDictionary.ContainsKey(mouseButton)) { // If we dont have a reference to the button already, create one
				bool succeed = AddDrag(mouseButton);
				if (!succeed) return;
			}
			_dragDictionary[mouseButton].RemoveDragEndListener(action);
		}

		#endregion

		#endregion

		#endregion

		#region Click and DoubleClick

		private void UpdateDoubleClickState()
		{
			if (_dblClickState != DoubleClickState.None && _dblClickState != DoubleClickState.WaitingForSecondClick) { _dblClickState = DoubleClickState.None; }    //Reset our clickstate back to none each frame so our data refreshes

			// Check if our mouse has moved
			if (_inClickCheck && (GetAxisDeltaInternal(InputManagerUtility.AxesNameFromEnum(InputAxes.Mouse_X)) != 0 || GetAxisDeltaInternal(InputManagerUtility.AxesNameFromEnum(InputAxes.Mouse_Y)) != 0)) {  //If our mouse changes position at all
				_lastClickTime = -1; //Reset our click check
				_dblClickState = DoubleClickState.MouseCancel;
			}

			if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse)) {

				// If we arent currently within our double click check, and we get left mouse button down we start to check for more inputs
				if (!_inClickCheck) {
					_lastClickTime = Time.time;
					_dblClickState = DoubleClickState.WaitingForSecondClick;
					return;
				}

				// If we have clicked within our threshold, then we have successfully double clicked
				if (Time.time <= _lastClickTime + _dblClickTimeThreshold) {
					_lastClickTime = -1;
					_dblClickState = DoubleClickState.DoubleClick;
				}
			}

			// If our first click has expired past our threshold, reset our click state
			if (_inClickCheck && Time.time > _lastClickTime + _dblClickTimeThreshold) {
				_lastClickTime = -1;
				_dblClickState = DoubleClickState.Timeout;
			}
		}

		public static DoubleClickState GetDoubleClickState()
		{
			return instanceExists ? instance._dblClickState : DoubleClickState.None; //Returns our clickstate
		}

		public static bool GetDoubleClick()
		{
			return instanceExists && instance._dblClickState == DoubleClickState.DoubleClick;
		}

		public static bool GetUnsuccessfulDoubleClick()
		{
			return instanceExists && (instance._dblClickState == DoubleClickState.MouseCancel || instance._dblClickState == DoubleClickState.Timeout);
		}

		#endregion

		#endregion

		//Setup system to make custom datatypes.
		//E.g D-pad (Two axis converted into one data returning a vector2)
		//E.g HotkeyBar (Map a number of button inputs to a single float or int range)

#endif

	}

}