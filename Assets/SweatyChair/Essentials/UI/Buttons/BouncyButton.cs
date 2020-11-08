using SweatyChair.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI.CoroutineTween;
using UnityEngine.UI.Extensions.Tweens;

namespace UnityEngine.UI
{

	[RequireComponent(typeof(Selectable))]
	public class BouncyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{

		#region Const

		private const string DEFAULT_BOUNCY_BUTTON_PRESET = "BouncyButtonPresets/DefaultBouncyButtonPreset";

		#endregion

		#region Variables

		[FormerlySerializedAs("_bouncyButtonPreset")] [SerializeField] private BouncyButtonPreset _preset;

		// Target Data
		private Selectable _selectable;
		private RectTransform m_RectTransform;

		// Tweening data
		[NonSerialized] private TweenRunner<EaseTypeTween> m_AnimCurveTweenRunner;
		private TweenRunner<EaseTypeTween> animCurveTweenRunner
		{
			get {
				if (m_AnimCurveTweenRunner == null) {
					m_AnimCurveTweenRunner = new TweenRunner<EaseTypeTween>();
					m_AnimCurveTweenRunner.Init(this);
				}
				return m_AnimCurveTweenRunner;
			}
		}
		private Canvas m_CanvasComponent;

		// Cached Starting Values
		[NonSerialized] private Vector3 _initialScale;
		[NonSerialized] private float _currentSizeMult = 1;
		[NonSerialized] private bool _isHovering;
		[NonSerialized] private bool _isMouseDown;
		[NonSerialized] private bool _isPlayingAnim;

		#endregion

		#region Awake / Reset

		private void Awake()
		{
			if (m_RectTransform == null)
				m_RectTransform = GetComponent<RectTransform>();

			if (_selectable == null)
				_selectable = GetComponent<Selectable>();
		}

		private void Reset()
		{
			if (_preset == null)
				_preset = Resources.Load<BouncyButtonPreset>(DEFAULT_BOUNCY_BUTTON_PRESET);
		}

		#endregion

		#region OnEnable / OnDisable

		private void OnEnable()
		{
			_currentSizeMult = 1;
			_initialScale = m_RectTransform.localScale;
			_isHovering = false;
		}

		private void OnDisable()
		{
			_currentSizeMult = 1;
			m_RectTransform.localScale = _initialScale;
			_isHovering = false;
		}

		#endregion

		#region Pointer Methods

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (_preset == null)
				return;

			// Dont allow enter events when our hover has been previously used
			if (eventData.used || !_selectable.interactable)
				return;

			if (m_RectTransform != null || !isActiveAndEnabled)
				ScaleTween(_preset.curves.pressedEase, false);
			_isMouseDown = true;

			if (_preset.eatInteractionEvents)
				eventData.Use();
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (_preset == null)
				return;

			if (m_RectTransform != null || !isActiveAndEnabled) {
				if (_isHovering)
					ScaleTween(_preset.curves.highlightedEase, false);
				else
					ScaleTween(_preset.curves.normalEase, false);
			}
			_isMouseDown = false;

			if (_preset.eatInteractionEvents)
				eventData.Use();
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (_preset == null || !_preset.allowHover)
				return;

			// Dont allow enter events when our hover has been previously used
			if (eventData.used || !_selectable.interactable)
				return;

			if (m_RectTransform != null || !isActiveAndEnabled) {
				if (!_isMouseDown)
					ScaleTween(_preset.curves.highlightedEase, false);
			}

			// If we play sound on hover
			if (_preset.hoverAudioProvider != null)
				_preset.hoverAudioProvider.PlaySFX();

			// Set our sorting order
			SetSortingOrder(true);
			_isHovering = true;

			if (_preset.eatInteractionEvents)
				eventData.Use();

		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (_preset == null || !_preset.allowHover)
				return;

			if (m_RectTransform != null || !isActiveAndEnabled) {
				if (!_isMouseDown || eventData.used)
					ScaleTween(_preset.curves.normalEase, false);
			}
			// Set our sorting order
			SetSortingOrder(false);
			_isHovering = false;

			if (_preset.eatInteractionEvents)
				eventData.Use();
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (_preset == null)
				return;

			// Dont allow enter events when our hover has been previously used
			if (eventData.used || !_selectable.interactable)
				return;

			// If we have an audio provider, play our SFX
			if (_preset.clickAudioProvider != null)
				_preset.clickAudioProvider.PlaySFX();

			if (_preset.eatInteractionEvents)
				eventData.Use();
		}

		#endregion

		#region Scale Method

		private void ScaleTween(EaseData targetCurve, bool instant)
		{
			if (m_RectTransform == null || !isActiveAndEnabled) {
				return;
			}
			_isPlayingAnim = true;
			float animDuration = instant ? 0 : targetCurve.animationTime;
			var curveTween = new EaseTypeTween {
				duration = targetCurve.animationTime,
				startFloat = _currentSizeMult,
				endFloat = targetCurve.endScale,
				ignoreTimeScale = true
			};
			curveTween.SetEasingFunc(targetCurve.easeType);
			curveTween.AddOnChangedCallback(TransformScaleCallback);
			curveTween.AddOnCompleteCallback(() => {
				_isPlayingAnim = false;
			});
			animCurveTweenRunner.StartTween(curveTween);
		}

		void TransformScaleCallback(float alpha)
		{
			if (m_RectTransform == null || !isActiveAndEnabled)
				return;
			_currentSizeMult = alpha;
			m_RectTransform.localScale = _initialScale * _currentSizeMult;
		}

		#endregion

		#region Sorting Order Method

		private void SetSortingOrder(bool isHovered)
		{
			// Return if we do not override sorting order
			if (!_preset.overrideSortingOrder || !isActiveAndEnabled) { return; }

			// Then get component if our object is null
			if (m_CanvasComponent == null) {
				// Add Canvas so we can override sorting
				m_CanvasComponent = this.GetOrAddComponent<Canvas>();
				m_CanvasComponent.overrideSorting = true;
				m_CanvasComponent.sortingOrder = _preset.normalSortingOrder;
				// Add our graphic raycaster otherwise we cannot select our UI anymore
				this.GetOrAddComponent<GraphicRaycaster>();
			}

			// Then set our sorting order
			m_CanvasComponent.sortingOrder = (isHovered) ? _preset.hoveredSortingOrder : _preset.normalSortingOrder;
			m_CanvasComponent.overrideSorting = m_CanvasComponent.sortingOrder != _preset.normalSortingOrder;
		}

		#endregion

#if UNITY_EDITOR

		[ContextMenu("Add to all buttons in scene")]
		private void AddToAllButtons()
		{
			foreach (Button go in Resources.FindObjectsOfTypeAll(typeof(Button)) as Button[]) {
				if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
					continue;
				if (UnityEditor.EditorUtility.IsPersistent(go.transform.root.gameObject))
					continue;
				if (go.GetComponent<BouncyButton>() == null)
					UnityEditor.ObjectFactory.AddComponent<BouncyButton>(go.gameObject);
			}
		}

#endif

	}

}