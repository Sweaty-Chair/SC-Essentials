using UnityEngine;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(CanvasGroup))]
	public class UIFader : BaseFader
	{

		#region Variables

		[Header("Settings")] [SerializeField] private bool _toggleCanvasOnInvisible = false;
		[Space()]
		[SerializeField] private bool _isInteractableWhenInvisible = true;
		[SerializeField] private bool _blocksRaycastsWhenInvisible = true;

		// Base Overrides
		public override float currentValue { get { return canvasGroup.alpha; } }
		public override bool isFadedOut { get { return canvasGroup.alpha == 0; } }

		private Canvas _selfCanvas;
		public Canvas selfCanvas { get { if (_selfCanvas == null) { _selfCanvas = GetComponent<Canvas>(); } return _selfCanvas; } }

		private CanvasGroup _canvasGroup;
		public CanvasGroup canvasGroup { get { if (_canvasGroup == null) { _canvasGroup = GetComponent<CanvasGroup>(); } return _canvasGroup; } }

		#endregion

		#region Apply Preset

		protected override void ApplyPreset()
		{
			// Call our base preset apply
			base.ApplyPreset();

			if (_faderPreset != null) {
				// If our fader is type of UIFader Preset, apply our fader settings
				UIFaderPreset uiFaderPreset = _faderPreset as UIFaderPreset;
				if (uiFaderPreset != null) {
					_isInteractableWhenInvisible = uiFaderPreset.isInteractableWhenInvisible;
					_blocksRaycastsWhenInvisible = uiFaderPreset.blocksRaycastsWhenInvisible;
				} else {
					Debug.LogErrorFormat("Attempting to use an non-matching preset on gameobject '{0}'. Using '{1}' instead of '{2}'", gameObject.name, _faderPreset.GetType().Name, typeof(UIFaderPreset).Name);
				}
			}
		}

		#endregion

		#region On Tween Update

		protected override void OnTweenUpdate(float alphaAmnt)
		{
			canvasGroup.alpha = alphaAmnt;

			// Toggle our canvas on alpha 0
			if (selfCanvas && _toggleCanvasOnInvisible)
				selfCanvas.enabled = (alphaAmnt != 0);

			canvasGroup.interactable = (_isInteractableWhenInvisible || (!_isInteractableWhenInvisible && canvasGroup.alpha != 0));
			canvasGroup.blocksRaycasts = (_blocksRaycastsWhenInvisible || (!_blocksRaycastsWhenInvisible && canvasGroup.alpha != 0));
		}

		#endregion

	}

}
