using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SweatyChair.UI
{

	/// <summary>
	/// Save and load persisent value from PlayerPrefs into a Slider.
	/// </summary>
	[RequireComponent(typeof(Slider))]
	public class SliderValueSetter : UIBehaviour
	{

		[Tooltip("PlayerPrefs key saving the toggle value, please make sure the key is not used in anywhere else.")]
		[SerializeField] private string _playerPrefsKey = "SliderValue";
		[Tooltip("The default value of the toggle.")]
		[SerializeField] private float _defaultValue;
		[Tooltip("Load and set the value in every OnEnable, otherwise once in Awake.")]
		[SerializeField] private bool _setOnEnable;

		protected override void Awake()
		{
			if (!_setOnEnable)
				SetToggleValue();
		}

		protected override void OnEnable()
		{
			if (_setOnEnable)
				SetToggleValue();
		}

		private void SetToggleValue()
		{
			Slider slider = GetComponent<Slider>();
			if (slider != null) {
				slider.value = PlayerPrefs.GetFloat(_playerPrefsKey, _defaultValue);
				slider.onValueChanged.AddListener(OnSliderValueChanged);
			}
		}

		private void OnSliderValueChanged(float value)
		{
			PlayerPrefs.SetFloat(_playerPrefsKey, value);
		}

	}

}