#if CROSS_PLATFORM_INPUT

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SweatyChair.InputSystem
{

	public class CallEventOnAxis : MonoBehaviour
	{

#region Class

		[System.Serializable]
		private class AxisEvent : UnityEvent<float> { }

#endregion

#region Event

		[Header("Settings")]
		[SerializeField] private string _listeningAxis;

		[Header("Events")]
		[FormerlySerializedAs("axisChangeEvent")]
		[SerializeField] private AxisEvent _axisChanged;
		[Space()]
		[FormerlySerializedAs("inverseAxisChangeEvent")]
		[SerializeField] private AxisEvent _inverseAxisChanged;

#endregion

#region OnEnable / OnDisable

		private void OnEnable()
		{
			InputManager.AddListenerToAxisValue(_listeningAxis, OnAxisChange);
		}

		private void OnDisable()
		{
			InputManager.RemoveListenerFromAxisValue(_listeningAxis, OnAxisChange);
		}

#endregion

#region Event Callbacks

		private void OnAxisChange(float axisVal)
		{
			_axisChanged?.Invoke(axisVal);
			_inverseAxisChanged?.Invoke(-axisVal);
		}

#endregion

	}


}

#endif