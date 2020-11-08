using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SweatyChair.StateManagement;

namespace SweatyChair.UI
{

	public enum LastBackClickAction
	{
		QuitGame,
		OpenSettings,
		Callback,
		None
	}

	/// <summary>
	/// A manager that attachs to main Canvas and mamanges all 1st-level panels.
	/// Panels beside 1st-level should be mannually managed by the 1st-level panels.
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public class PanelManager : MonoBehaviour
	{

		public static event UnityAction gameQuitted;

		// Dictionary of all Panels as level 1 child in the UI Canvas
		private static Dictionary<Type, Panel> _panelDict = new Dictionary<Type, Panel>();

		[SerializeField] private bool _allowEscapeKey;
		[SerializeField] private bool _useInputManagerKey = true;
		[SerializeField] private LastBackClickAction _lastBackClickAction;

		private static bool _isEscapeKeyAllowed = true; // A tempoily toogle, e.g. used to disable escape key in tutorial

		private void Awake()
		{
			foreach (Transform t in transform) {
				Panel panel = t.GetComponent<Panel>();
				if (panel != null) {
					panel.Init();
					AddPanel(panel);
				}
			}
		}

		public static void AddPanel(Panel panel)
		{
			_panelDict[panel.GetType()] = panel;
		}

#if UNITY_ANDROID || UNITY_STANDALONE || UNITY_EDITOR

		private bool _isQuitGameConfirmShown; // To avoid showing multiple message in Update

		// Back button detect
		private void Update()
		{
			if (!_allowEscapeKey)
				return;

			bool escapeKeyClicked = Input.GetKeyUp(KeyCode.Escape);
#if CROSS_PLATFORM_INPUT
			if (_useInputManagerKey)
				escapeKeyClicked |= SweatyChair.InputSystem.InputManager.GetButtonUp(SweatyChair.InputSystem.InputAxes.Toggle_Options_Menu);
#endif

			if (escapeKeyClicked) {
				
				if (Panel.shownPanels.Count > 0) { // Hide last shown panel if there's any

					if (!_isEscapeKeyAllowed) // Ignore back button click, e.g. when running a tutorial
						return;
					Panel.shownPanels.Peek().TriggerBackClick();

				} else { // Do the last back click action if no panel shown
					
					switch (_lastBackClickAction) {
						case LastBackClickAction.QuitGame:
							if (!_isQuitGameConfirmShown) {
								_isQuitGameConfirmShown = true;
								new Message {
									title = LocalizeUtils.GetFormat(TermCategory.Message, CommonTexts.MSG_QUIT_GAME_TITLE, LocalizeUtils.Get(Application.productName)),
									content = LocalizeUtils.Get(TermCategory.Message, CommonTexts.MSG_QUIT_GAME_CONTENT),
									confirmCallback = Application.Quit,
									cancelCallback = ResetQuitGameConfirmShown,
								}.Show();
							}
							break;
						case LastBackClickAction.OpenSettings:
							if (StateManager.Compare(State.Game, State.PauseGame)) // Only in game
								SettingManager.ToggleUI();
							break;
						case LastBackClickAction.Callback:
							gameQuitted?.Invoke();
							break;
					}

				}

            }
		}

		private void ResetQuitGameConfirmShown()
		{
			_isQuitGameConfirmShown = false;
		}

#endif

		private void OnDestroy()
		{
			// Reset all Panels
			foreach (Transform t in transform) {
				Panel panel = t.GetComponent<Panel>();
				if (panel != null)
					panel.Reset();
			}
			_panelDict.Clear();
		}

		/// <summary>
		/// Turns on/off the escape key function tempoily, e.g. in a tutorial.
		/// </summary>
		public static void ToggleEscapeKeyAllowed(bool isAllowed)
		{
			_isEscapeKeyAllowed = isAllowed;
		}

		/// <summary>
		/// Get a Panel instance, this only work for Panels as level 1 child in the UI Canvas
		/// </summary>
		public static T Get<T>() where T : Panel
		{
			Type t = typeof(T);
			// Get the exact class if any
			if (_panelDict.ContainsKey(t))
				return (T)_panelDict[t];
			// Try get the parent Panel class if any
			foreach (var kvp in _panelDict) {
				if (kvp.Key.IsSubclassOf(typeof(T)))
					return (T)kvp.Value;
			}
			return null;
		}

		/// <summary>
		/// Determines if a Panel instane is shown, this only work for Panels as level 1 child in the UI Canvas
		/// </summary>
		public static bool IsShown<T>() where T : Panel
		{
			Panel p = Get<T>();
			if (p == null)
				return false;
			return p.isShown;
		}

		/// <summary>
		/// Toggle a Panel instance, this only work for Panels as level 1 child in the UI Canvas
		/// </summary>
		public static void Toggle<T>(bool doShow) where T : Panel
		{
			Panel p = Get<T>();
			if (p != null)
				p.Toggle(doShow);
			else
				Debug.LogWarningFormat("PanelManager:Toggle - Panel type={0} not found.", typeof(T));
		}

		public static void Show<T>() where T : Panel
		{
			Toggle<T>(true);
		}

		public static void Hide<T>() where T : Panel
		{
			Toggle<T>(false);
		}

		public static void Destroy<T>() where T : Panel
		{
			Panel p = Get<T>();
			if (p == null) {
				_panelDict.Remove(typeof(T));
				Destroy(p);
			}
		}

		private static PanelManager _instance;
		public static PanelManager instance {
			get {
				if (_instance == null)
					_instance = FindObjectOfType<PanelManager>();
				return _instance;
			}
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Panels/Print Panel Dict")]
		public static void PrintPanelDict()
		{
			DebugUtils.LogCollection(_panelDict);
		}

#endif

	}

}