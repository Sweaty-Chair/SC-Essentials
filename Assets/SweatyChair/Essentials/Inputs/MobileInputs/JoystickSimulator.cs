#if CROSS_PLATFORM_INPUT

using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace SweatyChair.InputSystem
{

	/// <summary>
	/// A simulator that visually simulate joystick movement with WASD keyboard in Editor.
	/// (i.e. This only shows how joystick move while using keyboard, no controlling logic at all)
	/// Author: Richard
	/// </summary>
	public class JoystickSimulator : MonoBehaviour
	{

		private Joystick _joystick;
		private RectTransform _rectTransform;

		private void Awake()
		{
#if !UNITY_EDITOR && !UNITY_STANDALONE
		Destroy(this);
#endif

			_joystick = GetComponent<Joystick>();
			_rectTransform = GetComponent<RectTransform>();
		}

#if UNITY_EDITOR && !UNITY_STANDALONE
		void Update()
		{
			// CrossPlatformInputManager not working at mobile Editor, map WASD keys for movement too
			float h = CrossPlatformInputManager.GetAxis("Horizontal");
			float v = CrossPlatformInputManager.GetAxis("Vertical");
			if (h == 0 && v == 0) {
				h = Input.GetAxis("Horizontal");
				v = Input.GetAxis("Vertical");
				_rectTransform.localPosition = (new Vector3(h, v, 0).normalized) * _joystick.MovementRange;
			}
		}
#endif

	}

}

#endif