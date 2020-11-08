using UnityEngine;
using System.Collections.Generic;
using SweatyChair.StateManagement;

namespace SweatyChair.UI
{

	/// <summary>
	/// A base class for all UI panels. Contains common functions such as back click, show, hide, etc.
	/// </summary>
	public abstract class Panel : MonoBehaviour
	{

		protected static Stack<Panel> _shownPanels = new Stack<Panel>(); // First come, last serve

		[Header("General")]
		[Tooltip("Listen to back button and hide when it's clicked.")]
		[SerializeField] protected bool _hideOnBackClick;

		private float _shownTime;

		public static Stack<Panel> shownPanels => _shownPanels;

		public bool isShown => gameObject.activeSelf;

		public float shownDuration => Time.unscaledTime - _shownTime;

		public static bool IsNoPanelShownExcept<T>()
		{
			foreach (Panel panel in _shownPanels) {
				if (panel is T)
					continue;
				return false;
			}
			return true;
		}

		public static bool IsPanelShown<T>()
		{
			foreach (Panel panel in _shownPanels) {
				if (panel is T)
					return true;
			}
			return false;
		}

		public static bool IsPanelOnTop<T>()
		{
			return _shownPanels.Peek() is T;
		}

		public static void Clear()
		{
			_shownPanels.Clear();
		}

		/// <summary>
		/// A initialization method run on Awake.
		/// Note: Remember to put base.Init() if it needs to check state
		/// </summary>
		public virtual void Init()
		{
			StateManager.stateChanged += OnStateChanged;
		}

		/// <summary>
		/// A reset method run on Destroy.
		/// Note: Remember to put base.Reset() if it needs to check state
		/// </summary>
		public virtual void Reset()
		{
			StateManager.stateChanged -= OnStateChanged;
		}

		protected virtual void OnStateChanged(State state) { }

		/// <summary>
		/// Mannually tiggers back button click with script.
		/// </summary>
		public virtual void TriggerBackClick()
		{
			if (_hideOnBackClick)
				OnBackClick();
		}

		/// <summary>
		/// Hides the panel on back click, override this for any custom action.
		/// </summary>
		public virtual void OnBackClick()
		{
			Hide();
		}

		/// <summary>
		/// Shows the panel, override this for any custom action.
		/// </summary>
		public virtual void Show()
		{
			Toggle(true);
		}

		/// <summary>
		/// Hides the panel, override this for any custom action.
		/// </summary>
		public virtual void Hide()
		{
			Toggle(false);
		}

		/// <summary>
		/// Shows/hides the panel, override this for any custom action.
		/// </summary>
		public virtual void Toggle(bool show)
		{
			if (gameObject != null) gameObject.SetActive(show); // Avoid scene change and game object is destroyed
			CheckShownPanels(show);
			if (show) _shownTime = Time.unscaledTime;
		}

		/// <summary>
		/// Checks adding/removing this panels in the cached shown panels list.
		/// </summary>
		protected void CheckShownPanels(bool show)
		{
			if (_hideOnBackClick) {
				if (show) {
					if (_shownPanels.Count == 0 || _shownPanels.Peek() != this)
						_shownPanels.Push(this);
				} else if (_shownPanels.Count > 0 && _shownPanels.Peek() == this) {
					_shownPanels.Pop();
				}
			}
		}

		/// <summary>
		/// Hide the toppest panel.
		/// </summary>
		public static void HideCurrent()
		{
			if (_shownPanels.Count > 0)
				_shownPanels.Pop()?.Hide();
		}

		/// <summary>
		/// Hide all shown panel.
		/// </summary>
		public static void HideAll()
		{
			while (_shownPanels.Count > 0)
				_shownPanels.Pop()?.Hide();
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Panels/Print Shown Panels", false, 100)]
		public static void PrintShownPanels()
		{
			DebugUtils.LogCollection(_shownPanels);
		}

		[ContextMenu("Print Path")]
		public void PrintPath()
		{
			Debug.Log(transform.ToPath());
		}

		[ContextMenu("Show")]
		public void DebugShow()
		{
			Show();
		}

		[ContextMenu("Hide")]
		public void DebugHide()
		{
			Hide();
		}

#endif

	}

}