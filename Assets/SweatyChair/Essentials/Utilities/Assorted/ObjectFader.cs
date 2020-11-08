using UnityEngine;

namespace SweatyChair
{

	public class ObjectFader : BaseFader
	{

		#region Const

		public const string FADE_PROPERTY_ID = "_Alpha";

		#endregion

		#region Variables

		[Header("Settings")]
		[SerializeField] private bool _getChildrenRenderers = true;
		[SerializeField] private bool _disableRenderersOnFullFade = true;
		[SerializeField] private bool _disableCollidersOnFullFade = false;

		private float _currentValue = 0;

		// Base Overrides
		public override float currentValue { get { return _currentValue; } }
		public override bool isFadedOut { get { return _currentValue == 0; } }

		private Renderer[] _allAttachedRenderers;
		private Collider[] _allAttachedColliders;
		private MaterialPropertyBlock _currentPropBlock;

		#endregion

		#region Apply Preset

		protected override void ApplyPreset()
		{
			// Call our base preset apply
			base.ApplyPreset();

			if (_faderPreset != null) {

				// If our fader is type of UIFader Preset, apply our fader settings
				ObjectFaderPreset objectFaderPreset = _faderPreset as ObjectFaderPreset;
				if (objectFaderPreset != null) {
					_getChildrenRenderers = objectFaderPreset.getChildrenRenderers;
					_disableRenderersOnFullFade = objectFaderPreset.disableRenderersOnFullFade;
					_disableCollidersOnFullFade = objectFaderPreset.disableCollidersOnFullFade;
				} else
					Debug.LogErrorFormat("Attempting to use an non-matching preset on gameobject '{0}'. Using '{1}' instead of '{2}'", gameObject.name, _faderPreset.GetType().Name, typeof(ObjectFaderPreset).Name);

			}
		}

		#endregion

		#region OnEnable / OnDisable

		private void OnEnable()
		{
			// Get all our object components
			_currentPropBlock = new MaterialPropertyBlock();
			_allAttachedRenderers = (_getChildrenRenderers) ? transform.GetComponentsInChildren<Renderer>() : transform.GetComponents<Renderer>();
			_allAttachedColliders = (_getChildrenRenderers) ? transform.GetComponentsInChildren<Collider>() : transform.GetComponents<Collider>();

			_currentValue = 0;
		}

		#endregion

		#region On Tween Update

		protected override void OnTweenUpdate(float alphaAmnt)
		{
			_currentValue = alphaAmnt;
			// Set the current value in our property block, then go through all of the other renderers and set these values
			_currentPropBlock.SetFloat(FADE_PROPERTY_ID, alphaAmnt);

			for (int i = 0; i < _allAttachedRenderers.Length; i++) {
				_allAttachedRenderers[i]?.SetPropertyBlock(_currentPropBlock);

				if (_allAttachedRenderers[i] != null && _disableRenderersOnFullFade) {
					_allAttachedRenderers[i].enabled = !Mathf.Approximately(alphaAmnt, 0);
				}
			}

			// Then if we need to do anything with colliders
			if (_disableCollidersOnFullFade) {
				for (int i = 0; i < _allAttachedColliders.Length; i++) {
					if (_allAttachedColliders[i] != null)
						_allAttachedColliders[i].enabled = !Mathf.Approximately(alphaAmnt, 0);
				}
			}
		}

		#endregion

	}

}
