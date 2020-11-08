using UnityEngine;
using System;

namespace SweatyChair
{
	/// <summary>
	/// A simple version of singleton class, where don't intantiate if none and destroy if exists.
	/// </summary>
	/// <typeparam name="T">Type of the singleton</typeparam>
	public abstract class SimpleSingleton<T> : MonoBehaviour where T : SimpleSingleton<T>
	{
		
		/// <summary>
		/// The static reference to the instance
		/// </summary>
		public static T instance { get; private set; }

		/// <summary>
		/// Gets whether an instance of this singleton exists
		/// </summary>
		public static bool instanceExists => instance != null;

		public static event Action instanceSetEvent;

		/// <summary>
		/// Awake method to associate singleton with instance
		/// </summary>
		protected virtual void Awake()
		{
			instance = (T)this;
			instanceSetEvent?.Invoke();
		}

		/// <summary>
		/// OnDestroy method to clear singleton association
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (instance == this)
				instance = null;
		}
	}

}