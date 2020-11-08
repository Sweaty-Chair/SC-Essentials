using UnityEngine;
using UnityEngine.Events;
using System;

namespace SweatyChair.Events
{

	[Flags]
	public enum MonoEventType
	{
		Awake = 1 << 0,
		OnEnable = 1 << 1,
		Start = 1 << 2,
		FixedUpdate = 1 << 3,
		Update = 1 << 4,
		LateUpdate = 1 << 5,
		OnDisable = 1 << 6,
		OnDestroy = 1 << 7
	}

	/// <summary>
	/// A event helpe that simply fire callbacks on Unity's MonoBehaviour lifecycle events.
	/// Author: Christ
	/// https://docs.unity3d.com/Manual/ExecutionOrder.html
	/// </summary>
	public class MonoBehaviourEventHandler : MonoBehaviour
	{

		[EnumFlag("Events")]
		public MonoEventType selectedMonoEvents;

		[Header("Initialisation")]
		public UnityEvent awake;
		public UnityEvent onEnable;
		public UnityEvent start;

		[Header("Physics")]
		public UnityEvent fixedUpdate;

		[Header("Game Logic")]
		public UnityEvent update;
		public UnityEvent lateUpdate;

		[Header("Decommissioning")]
		public UnityEvent onDisable;
		public UnityEvent onDestroy;

		#region Initialisation

		private void Awake()
		{
			awake?.Invoke();
		}

		void OnEnable()
		{
			onEnable?.Invoke();
		}

		void Start()
		{
			start?.Invoke();
		}

		#endregion

		#region Physics

		void FixedUpdate()
		{
			fixedUpdate?.Invoke();
		}

		#endregion

		#region Game Logic

		void Update()
		{
			update?.Invoke();
		}

		void LateUpdate()
		{
			lateUpdate?.Invoke();
		}

		#endregion

		#region Decommissioning

		void OnDisable()
		{
			onDisable?.Invoke();
		}

		void OnDestroy()
		{
			onDestroy?.Invoke();
		}

		#endregion

		/// <summary>
		/// Removes all non-persistent listeners from all events.
		/// </summary>
		public void RemoveAllListeners()
		{
			awake.RemoveAllListeners();
			onEnable.RemoveAllListeners();
			start.RemoveAllListeners();
			fixedUpdate.RemoveAllListeners();
			update.RemoveAllListeners();
			lateUpdate.RemoveAllListeners();
			onDisable.RemoveAllListeners();
			onDestroy.RemoveAllListeners();
		}

	}

}