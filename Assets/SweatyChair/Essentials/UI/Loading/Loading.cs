using UnityEngine.Events;

namespace SweatyChair.UI
{

	public class Loading
	{

		public string content { get; set; }

		public string extraText { get; set; }

		public string cancelButtonText { get; set; }

		public UnityAction cancelCallback { get; set; }

		public float timeout { get; set; }

		// Execute in update while loading is shown, used to check if meeting the 
		public System.Predicate<float> checkHidePredicate { get; set; }

		public bool hasProgressBar { get; set; } = true;

		#region Constructor

		public Loading() { }

		public Loading(string content, bool hasProgressBar)
		{
			this.content = content;
			this.hasProgressBar = hasProgressBar;
		}

		#endregion

		public void Show()
		{
			LoadingManager.Show(this);
		}

		// A one line wrapper function for easier usage
		public static void Toggle(bool doShow, string content = "", UnityAction onCancelled = null)
		{
			if (doShow) {
				new Loading {
					content = content,
					cancelCallback = onCancelled
				}.Show();
			}
			else {
				LoadingManager.Hide();
			}
		}

		public static void Display(string content = "", UnityAction onCancelled = null)
		{
			Toggle(true, content, onCancelled);
		}

		public static void Hide()
		{
			LoadingManager.Hide();
		}

		public static void Hide(float waitTime)
		{
			LoadingManager.Hide(waitTime);
		}

		public static void SetProgress(float progress)
		{
			LoadingManager.SetProgress(progress);
		}

		public override string ToString()
		{
			return string.Format("[Loading: content={0}, extraText={1}, cancelButtonText={2}", content, extraText, cancelButtonText);
		}

	}

}