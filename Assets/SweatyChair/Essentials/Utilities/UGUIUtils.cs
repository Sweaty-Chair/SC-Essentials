using UnityEngine.UI;

public static class UGUIUtils
{

	#region Set Without Callback

	static Slider.SliderEvent emptySliderEvent = new Slider.SliderEvent();
	public static void SetValueNoCallback(this Slider instance, float value)
	{
		var originalEvent = instance.onValueChanged;
		instance.onValueChanged = emptySliderEvent;
		instance.value = value;
		instance.onValueChanged = originalEvent;
	}

	static Toggle.ToggleEvent emptyToggleEvent = new Toggle.ToggleEvent();
	public static void SetValueNoCallback(this Toggle instance, bool value)
	{
		var originalEvent = instance.onValueChanged;
		instance.onValueChanged = emptyToggleEvent;
		instance.isOn = value;
		instance.onValueChanged = originalEvent;
	}

	static InputField.OnChangeEvent emptyInputFieldEvent = new InputField.OnChangeEvent();
	public static void SetValueNoCallback(this InputField instance, string value)
	{
		var originalEvent = instance.onValueChanged;
		instance.onValueChanged = emptyInputFieldEvent;
		instance.text = value;
		instance.onValueChanged = originalEvent;
	}

	#endregion

}
