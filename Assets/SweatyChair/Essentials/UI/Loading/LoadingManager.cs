using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair.UI
{

	public class LoadingManager
	{

		public static event UnityAction<Loading> loadingShown;
		public static event UnityAction<float> loadingPogressUpdated;
		public static event UnityAction<string> loadingContentUpdated;
		public static event UnityAction onLoadingHidden;

		private static float _lastShownTime;

		public static void Show(Loading loading)
		{
			_lastShownTime = Time.unscaledTime;
			loadingShown?.Invoke(loading);
		}

		public static void Hide()
		{
			// Avoid flicker, add 0.5 minimum show time
			if (Time.unscaledTime < _lastShownTime + 0.5f)
				TimeManager.Invoke(Hide, _lastShownTime + 0.51f - Time.unscaledTime);
			else
				onLoadingHidden?.Invoke();
		}

		public static void Hide(float waitTime)
		{
			TimeManager.Invoke(Hide, waitTime);
		}

		// Set the progress bar
		public static void SetProgress(float progress)
		{
			loadingPogressUpdated?.Invoke(progress);
		}

		// Set the content text directly, useful when you need to change the content text at while showing
		public static void SetContent(string content)
		{
			loadingContentUpdated?.Invoke(content);
		}

	}

}