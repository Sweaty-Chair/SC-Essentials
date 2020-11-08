using UnityEngine;

namespace SweatyChair
{
	/// <summary>
	/// Manager for all Timescale related events. Alternative to Unity implementation for more options to mess with.
	/// </summary>
	/// TODO: Ideally this script shouldn't rely on Unity at all, other than base unmodified time scale.
	/// it should implement its own solutions for timescale, rather than piggyback off of Unity.
	/// http://www.asteroidbase.com/devlog/9-pausing-without-pausing/#more-569
	/// This shows a pretty simple and straightforward way to allow for multiple objects to have different timescales, and look pretty sicc
	public static class TimeScaleManager
	{

		#region Variables

		public static float timeScaleModifier { get; private set; } // The final multiplier to be applied to our timescale

		public static float timeScale => Time.timeScale; // Default Unity Timescale. This is what most scripts will be manipulating
		public static float modifiedTimeScale => Time.timeScale * timeScaleModifier;

		// Delta Time
		public static float unscaledDeltaTime => Time.unscaledDeltaTime; // Base Unity Implementation of unscaledDeltatime
		public static float scaledDeltaTime => Time.deltaTime; // The base Unity deltaTime. Which is only affected by our timescale
		public static float modifiedDeltaTime => Time.deltaTime * timeScaleModifier; // The final implementation of delta Time after it has been modified by our modifier
		public static float modifiedUnscaledDeltaTime => Time.unscaledDeltaTime * timeScaleModifier;

		// Fixed DeltaTime
		public static float unscaledFixedDeltaTime => Time.fixedUnscaledDeltaTime;
		public static float scaledFixedDeltaTime => Time.fixedDeltaTime;
		public static float modifiedFixedDeltaTime => Time.fixedDeltaTime * timeScaleModifier;
		public static float modifiedUnscaledFixedDeltaTime => Time.fixedUnscaledDeltaTime * timeScaleModifier;

		private static TicketPriorityQueue<float> timeScaleQueue = new TicketPriorityQueue<float>(1f, (data) => -data.item);

		private static float initialFixedTimeStep;
		private static float initialMaximumDeltaTime;

		#endregion

		#region Constructor

		static TimeScaleManager()
		{
			timeScaleModifier = 1;
			initialFixedTimeStep = Time.fixedDeltaTime;
			initialMaximumDeltaTime = Time.maximumDeltaTime;
		}

		#endregion

		#region Timescale Management

		public static void SetTimescale(ref int identifier, Priority priority, float timescale)
		{
			SetTimescale(ref identifier, (int)priority, timescale);
		}

		public static void SetTimescale(ref int identifier, float priority, float timescale)
		{
			identifier = timeScaleQueue.Add(identifier, priority, timescale);   //Add our timescale to queue
			Time.timeScale = timeScaleQueue.value;  //Set current timescale to the highest priority timescale
			Time.fixedDeltaTime = initialFixedTimeStep * timeScaleQueue.value;  //We also change the fixed timestep so the slow motion does not look jerky
			Time.maximumDeltaTime = initialMaximumDeltaTime * timeScaleQueue.value;
		}

		public static void RemoveTimescale(ref int identifier)
		{
			identifier = timeScaleQueue.Remove(identifier); //Remove our timescale from the queue
			Time.timeScale = timeScaleQueue.value;  //Set our current timescale to the highest priority timescale
			Time.fixedDeltaTime = initialFixedTimeStep * timeScaleQueue.value;  //We also change the fixed timestep so the slow motion does not look jerky
			Time.maximumDeltaTime = initialMaximumDeltaTime * timeScaleQueue.value;
		}

		public static void ResetTimescale()
		{
			timeScaleQueue.Reset();
		}

		#endregion

		#region Legacy Timescale Management
		//Remove this when we finally implement proper timescale modification. This just makes it easier to track and control how each object modifies timescale

		/// <summary>
		/// Controls timescale as well as modifying our fixed timestep so that all movement looks smooth
		/// </summary>
		/// <param name="newTimescale"></param>
		public static void SetTimescale(float newTimescale)
		{
			Time.timeScale = newTimescale;
			Time.fixedDeltaTime = initialFixedTimeStep * newTimescale;
			Time.maximumDeltaTime = initialMaximumDeltaTime * newTimescale;
		}

		#endregion

		#region Timescale Modifier Management

		/// <summary>
		/// Sets the overraching Timescale modifier, affecting all data. Only use this in editor for now, while it still uses Unity Systems
		/// </summary>
		/// <param name="value"></param>
		public static void SetTimescaleModifier(float value)
		{
			timeScaleModifier = value;
		}

		#endregion

	}

}