using SweatyChair;
using SweatyChair.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopoutSlider : MonoBehaviour
{

	#region Struct

	[System.Serializable]
	public class SliderIconRange
	{

		#region Variables

		public GameObject iconGO;
		public Vector2 minMaxRange;

		[HideInInspector] public UIFader iconFader;

		#endregion

	}

	#endregion

	#region Variable

	[Header("Required")]
	[SerializeField] private Button _showButton;
	[SerializeField] private UIFader _sliderHolderFader;
	[SerializeField] private Slider _slider;

	[Header("Visuals")]
	[SerializeField] private List<SliderIconRange> _iconRanges;
	[SerializeField] private UIFaderPreset _iconFaderPreset;

	[Header("Events")]
	public FloatUnityEvent onSliderValueChange;

	// Public getters
	public float value => _slider.value;

	#endregion

	#region OnEnable / OnDisable

	private void OnEnable()
	{
		_slider.onValueChanged.AddListener(OnSliderChanged);
		_showButton.onClick.AddListener(OnToggleSliderPopout);

		SetIconFaderPreset();

		OnToggleSliderPopout(false, true);
	}

	private void SetIconFaderPreset()
	{
		bool hasPreset = _iconFaderPreset != null;

		// Go through all of our icon ranges, and if we have a UI fader set our preset
		for (int i = 0; i < _iconRanges.Count; i++) {

			// Get our fader
			UIFader fader = _iconRanges[i].iconGO.GetComponent<UIFader>();

			// If we have a preset but no fader, add a fader to this object
			if (hasPreset && fader == null)
				fader = _iconRanges[i].iconGO.AddComponent<UIFader>();

			// If our fader is not null we should set our preset and then we assign our fader to our reference
			if (fader != null)
				fader.SetPreset(_iconFaderPreset);

			_iconRanges[i].iconFader = fader;

		}

	}

	private void OnDisable()
	{
		// Sometimes Using '?.' with Unity values will return not null, even if the object is null. So we use vanilla checks
		if (_slider != null)
			_slider.onValueChanged.RemoveListener(OnSliderChanged);

		if (_showButton != null)
			_showButton.onClick.AddListener(OnToggleSliderPopout);
	}

	#endregion

	#region Set Values

	public void SetSliderValue(float value, bool sendCallback)
	{
		// When we set slider value we do not want to call our callbacks
		UpdateSliderVisuals(value);

		if (sendCallback)
			onSliderValueChange?.Invoke(value);

	}

	#endregion

	#region UICallbacks

	public void OnSliderChanged(float value)
	{

		SetSliderValue(value, true);
	}

	private void UpdateSliderVisuals(float value)
	{
		// Reset our slider value with no callback just to update our visuals
		_slider.SetValueNoCallback(value);

		// Update our Slider Icon if required
		UpdateSliderIcon(value);
	}
	private void UpdateSliderIcon(float value)
	{
		// If our icon ranges is not null, and we do have some items in our range
		if (_iconRanges != null && _iconRanges.Count > 0) {

			for (int i = 0; i < _iconRanges.Count; i++) {

				bool fadeIconOut = !_iconRanges[i].minMaxRange.Contains(value);

				UIFader faderComponent = _iconRanges[i].iconFader;
				if (faderComponent != null) {
					// If we are already faded out we should not fade
					if (fadeIconOut != faderComponent.isFadingOut)
						faderComponent.Fade(fadeIconOut);

				} else {
					_iconRanges[i].iconGO.SetActive(fadeIconOut);
				}
			}

		}
	}

	private void OnToggleSliderPopout()
	{
		_sliderHolderFader.Fade(!_sliderHolderFader.isFadedOut);
	}
	private void OnToggleSliderPopout(bool show)
	{
		_sliderHolderFader.Fade(show, false);
	}
	private void OnToggleSliderPopout(bool show, bool isInstant)
	{
		_sliderHolderFader.Fade(!show, isInstant);
	}

	#endregion

}