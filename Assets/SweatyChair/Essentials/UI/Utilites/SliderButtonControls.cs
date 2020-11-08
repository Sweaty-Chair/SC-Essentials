using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(Slider))]
	public class SliderButtonControls : MonoBehaviour
	{

		#region Variables

		[Header("Slider Settings")]
		[Tooltip("The default amount to decrement our slider by on call.")]
		[SerializeField] private float _sliderIncrementAmount = 1;

		[Space()]
		[Tooltip("Whether on button click we should snap to the nearest increment or simply just add our value")]
		[SerializeField] private bool _snapToClosestIncrement = true;
		[Tooltip("The value to which we will attempt to snap to if 'Snap To Closest Increment is enabled'")]
		[SerializeField] private float _sliderSnapValue = 0.1f;

		// We want to cache our value, but also not have it show in inspector
		[SerializeField] [HideInInspector] private Slider _slider;

		#endregion

		#region Reset

		private void Reset()
		{
			// Get our slider component if it is null
			if (_slider == null)
				_slider = GetComponent<Slider>();
		}

		#endregion

		#region OnEnable

		private void OnEnable()
		{
			// Get our slider component if it is null
			if (_slider == null)
				_slider = GetComponent<Slider>();
		}

		#endregion

		#region Increment / Decrement Slider

		/// <summary>
		/// Increments the value of the attached slider by a default or specified amount
		/// </summary>
		public void IncrementSlider()
		{
			IncrementSlider(_sliderIncrementAmount);
		}
		public void IncrementSlider(float value)
		{
			SetSlider(_slider.value + value);
		}

		/// <summary>
		/// Decrements the value of the attached slider by a default or specified amount
		/// </summary>
		public void DecrementSlider()
		{
			DecrementSlider(_sliderIncrementAmount);
		}
		public void DecrementSlider(float value)
		{
			SetSlider(_slider.value - value);
		}


		/// <summary>
		/// Sets the final value on the slider taking into account all our settings on this object
		/// </summary>
		/// <param name="value"></param>
		public void SetSlider(float finalValue)
		{
			if (_snapToClosestIncrement) {
				finalValue /= _sliderSnapValue;
				finalValue = Mathf.Round(finalValue);
				finalValue *= _sliderSnapValue;
			}

			_slider.value = finalValue;
		}

		#endregion

	}

}
