using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

	#region UnityEvents

	[System.Serializable] public class BoolUnityEvent : UnityEvent<bool> { }
	[System.Serializable] public class FloatUnityEvent : UnityEvent<float> { }
	[System.Serializable] public class IntUnityEvent : UnityEvent<int> { }
	[System.Serializable] public class Vector2UnityEvent : UnityEvent<Vector2> { }
	[System.Serializable] public class Vector3UnityEvent : UnityEvent<Vector3> { }

	#endregion

	public static class EventUtils { }

}
