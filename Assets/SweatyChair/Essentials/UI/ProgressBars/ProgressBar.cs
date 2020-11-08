using UnityEngine.Events;

namespace SweatyChair
{

	public struct ProgressBarData
	{

		#region Vars

		public string Title;
		public string Descriptor;

		public float NormalizedProgress;

		public bool IsCancellable;
		public UnityAction OnCancelEvent;

		#endregion

		#region Constructor

		public ProgressBarData(string title, string descriptor, bool isCancellable, UnityAction onCancelEvent, float normalizedProgress)
		{
			Title = title;
			Descriptor = descriptor;
			IsCancellable = isCancellable;
			OnCancelEvent = onCancelEvent;
			NormalizedProgress = normalizedProgress;
		}

		#endregion
	}

	public static class ProgressBar
	{
		#region Events

		public static event UnityAction<ProgressBarData> progressBarShown;
		public static event UnityAction progressBarCleared;

		#endregion

		#region ProgressBar

		/// <summary>
		/// Displays or Updates a progress bar.
		/// If no progress is supplied, the progress bar will play a simple animation.
		/// </summary>
		public static void DisplayProgressBar(string title, string descriptor, float normalizedProgress = -1)
		{
			DisplayProgressBarInternal(title, descriptor, false, null, normalizedProgress);
		}

		/// <summary>
		/// Displays or updates a progress bar, which also has a cancel button.
		/// If no progress is supplied, the progress bar will play a simple animation.
		/// </summary>
		public static void DisplayCancellableProgressBar(string title, string descriptor, UnityAction onCancelAction = null, float normalizedProgress = -1)
		{
			DisplayProgressBarInternal(title, descriptor, true, onCancelAction, normalizedProgress);
		}

		/// <summary>
		/// Clears any currently showing progressbar from the screen.
		/// </summary>
		public static void ClearProgressBar()
		{
			progressBarCleared?.Invoke();
		}

		#endregion

		#region Internal ProgressBar

		private static void DisplayProgressBarInternal(string title, string descriptor, bool isCancellable, UnityAction onCancelEvent, float normalizedProgress)
		{
			ProgressBarData progressBarData = new ProgressBarData(title, descriptor, isCancellable, onCancelEvent, normalizedProgress);
			DisplayProgressBarInternal(progressBarData);
		}

		private static void DisplayProgressBarInternal(ProgressBarData progressData)
		{
			// Validate our data before showing our progressBar
			progressBarShown?.Invoke(progressData);

		}

		#endregion

	}

}