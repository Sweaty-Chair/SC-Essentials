using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{

	/// <summary>
	/// Better toggle -- replaces the stock one single graphic, into using on or off graphics.
	/// </summary>
	[AddComponentMenu("UI/BetterUI/BetterToggle", 31)]
	[RequireComponent(typeof(RectTransform))]
	public class BetterToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
	{

		#region Class

		public enum ToggleTransition
		{
			None,
			Fade
		}

		[Serializable]
		public class ToggleEvent : UnityEvent<bool> { }

		#endregion

		#region Variables

		/// <summary>
		/// Whether the toggle is currently active.
		/// </summary>
		public bool isOn {
			get { return m_IsOn; }
			set { Set(value); }
		}

		// Whether the toggle is on
		[Tooltip("Is the toggle currently on or off?")]
		[SerializeField] private bool m_IsOn;


		/// <summary>
		/// Transition type.
		/// </summary>
		public ToggleTransition toggleTransition = ToggleTransition.Fade;

		/// <summary>
		/// Graphic the toggle should be working with.
		/// </summary>
		public Graphic onGraphic;
		/// <summary>
		/// Graphic the toggle should be working with.
		/// </summary>
		public Graphic offGraphic;

		/// <summary>
		/// Group that this toggle can belong to.
		/// </summary>
		[SerializeField]
		private BetterToggleGroup m_Group;
		public BetterToggleGroup group {
			get { return m_Group; }
			set {
				m_Group = value;
#if UNITY_EDITOR
				if (Application.isPlaying)
#endif
				{
					SetToggleGroup(m_Group, true);
					PlayEffect(true);
				}
			}
		}

		/// <summary>
		/// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
		/// </summary>
		public ToggleEvent onValueChanged = new ToggleEvent();

		#endregion

		#region Constructor

		protected BetterToggle() { }

		#endregion

		#region Enable / Disable

		/// <summary>
		/// Assume the correct visual state.
		/// </summary>
		protected override void Start()
		{
			PlayEffect(true);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetToggleGroup(m_Group, false);
			PlayEffect(true);
		}

		protected override void OnDisable()
		{
			SetToggleGroup(null, false);
			base.OnDisable();
		}

		#endregion

		#region Validation

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
			if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

#endif // if UNITY_EDITOR

		#endregion

		#region Graphic Rebuild

		public virtual void Rebuild(CanvasUpdate executing)
		{
#if UNITY_EDITOR
			if (executing == CanvasUpdate.Prelayout)
				onValueChanged.Invoke(m_IsOn);
#endif
		}

		public virtual void LayoutComplete() { }

		public virtual void GraphicUpdateComplete() { }

		protected override void OnDidApplyAnimationProperties()
		{
			// Check if isOn has been changed by the animation.
			// Unfortunately there is no way to check if we don�t have a graphic.
			if (onGraphic != null && offGraphic != null) {
				bool oldValue = !Mathf.Approximately(onGraphic.canvasRenderer.GetColor().a, 0);
				if (m_IsOn != oldValue) {
					m_IsOn = oldValue;
					Set(!oldValue);
				}
			}

			base.OnDidApplyAnimationProperties();
		}

		#endregion

		#region Set Values

		public void Set(bool value)
		{
			Set(value, true);
		}

		public void Set(bool value, bool sendCallback)
		{
			if (m_IsOn == value)
				return;

			m_IsOn = value;

			//Do Toggle Group
			if (m_Group != null && IsActive()) {
				if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff)) {
					m_IsOn = true;
					m_Group.NotifyToggleOn(this);
				}
			}

			PlayEffect(toggleTransition == ToggleTransition.None);
			if (sendCallback) {
				UISystemProfilerApi.AddMarker("Toggle.value", this);
				onValueChanged.Invoke(m_IsOn);
			}
		}

		/// <summary>
		/// Play the appropriate effect.
		/// </summary>
		public void PlayEffect(bool instant)
		{
			if (!Application.isPlaying) {
#if UNITY_EDITOR
				onGraphic?.canvasRenderer?.SetAlpha(m_IsOn ? 1f : 0f);
				offGraphic?.canvasRenderer?.SetAlpha(m_IsOn ? 0f : 1f);
#endif
			} else {
				onGraphic?.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
				offGraphic?.CrossFadeAlpha(m_IsOn ? 0f : 1f, instant ? 0f : 0.1f, true);
			}
		}

		#endregion

		#region Toggle Group

		private void SetToggleGroup(BetterToggleGroup newGroup, bool setMemberValue)
		{
			BetterToggleGroup oldGroup = m_Group;

			// Sometimes IsActive returns false in OnDisable so don't check for it.
			// Rather remove the toggle too often than too little.
			if (m_Group != null)
				m_Group.UnregisterToggle(this);

			// At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
			// That's why we use the setMemberValue parameter.
			if (setMemberValue)
				m_Group = newGroup;

			// Only register to the new group if this Toggle is active.
			if (newGroup != null && IsActive())
				newGroup.RegisterToggle(this);

			// If we are in a new group, and this toggle is on, notify group.
			// Note: Don't refer to m_Group here as it's not guaranteed to have been set.
			if (newGroup != null && newGroup != oldGroup && isOn && IsActive())
				newGroup.NotifyToggleOn(this);
		}

		#endregion

		#region Internal

		private void InternalToggle()
		{
			if (!IsActive() || !IsInteractable())
				return;

			isOn = !isOn;
		}

		#endregion

		#region Input Handler

		/// <summary>
		/// React to clicks.
		/// </summary>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			InternalToggle();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			InternalToggle();
		}

		#endregion

	}

}