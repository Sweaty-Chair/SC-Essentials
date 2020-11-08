using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SweatyChair.UI
{

	/// <summary>
	/// Save and load persisent value from PlayerPrefs into a Toggle.
	/// </summary>
	[RequireComponent(typeof(Toggle))]
	public class ToggleValueSetter : UIBehaviour
	{

		[Tooltip("PlayerPrefs key saving the toggle value, please make sure the key is not used in anywhere else.")]
		[SerializeField] private string _playerPrefsKey = "ToggleEnabled";
		[Tooltip("The default value of the toggle.")]
		[SerializeField] private bool _defaultValue;
		[Tooltip("Load and set the value in every OnEnable, otherwise once in Awake.")]
		[SerializeField] private bool _setOnEnable;
		[Tooltip("Invoke the on value changed event of toggle on initialize.")]
		[SerializeField] private bool _invokeOnValueChangeCallback = true;

		protected override void Awake()
		{
			if (!_setOnEnable)
				SetToggleValue();
			GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
		}

		protected override void OnEnable()
		{
			if (_setOnEnable)
				SetToggleValue();
		}

		private void SetToggleValue()
		{
			Toggle toggle = GetComponent<Toggle>();
			bool isOn = PlayerPrefs.GetInt(_playerPrefsKey, _defaultValue ? 1 : 0) == 1;
			if (_invokeOnValueChangeCallback) {
				if (toggle.isOn == isOn)
					toggle?.onValueChanged?.Invoke(isOn);
				else
					toggle.isOn = isOn;
			} else {
				toggle.SetValueNoCallback(isOn);
			}
		}

		private void OnToggleValueChanged(bool toggled)
		{
			PlayerPrefs.SetInt(_playerPrefsKey, toggled ? 1 : 0);
		}

#if UNITY_EDITOR

		[ContextMenu("Reset Value")]
		private void ResetValue()
		{
			PlayerPrefs.DeleteKey(_playerPrefsKey);
		}

#endif

	}

}