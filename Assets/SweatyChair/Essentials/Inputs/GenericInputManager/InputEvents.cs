using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair.InputSystem {

	[System.Serializable]
	public class ButtonStateChangeEvent : UnityEvent<ButtonState> { }

	[System.Serializable]
	public class ButtonValueChangeEvent : UnityEvent<bool> { }


	[System.Serializable]
	public class AxisValueChangeEvent : UnityEvent<float> { }

	[System.Serializable]
	public class GenericVector2Event : UnityEvent<Vector2> { }

	[System.Serializable]
	public class DragPayloadEvent : UnityEvent<InputDragPayload> { }

}