using SweatyChair.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair
{

	public class ProgressBarPanel : SingletonPanel<ProgressBarPanel>
	{

		#region Variables

		[Header("Objects")]
		[SerializeField] private Text _titleText;
		[SerializeField] private Text _descriptorText;
		[SerializeField] private Image _progressImage;
		[SerializeField] private GameObject _cancelButtonGO;

		private ProgressBarData _progressData;

		#endregion

		#region Init / Reset

		public void Prewarm() { }

		public override void Init()
		{
			base.Init();
			ProgressBar.progressBarShown += Show;
			ProgressBar.progressBarCleared += Clear;
			Clear();
		}

		protected void OnDestroy()
		{
			ProgressBar.progressBarShown -= Show;
			ProgressBar.progressBarCleared -= Clear;
		}

		#endregion

		#region Show / Hide

		public void Show(ProgressBarData progressData)
		{
			//Store a local reference to our progress data
			_progressData = progressData;

			//Then assign all our text and images
			_titleText.text = progressData.Title;
			_descriptorText.text = progressData.Descriptor;
			_progressImage.fillAmount = progressData.NormalizedProgress;

			_cancelButtonGO.SetActive(progressData.IsCancellable);

			gameObject.SetActive(true);
		}

		public new void Clear()
		{
			// Clear our current var
			_progressData = new ProgressBarData();

			// Reset all our text and buttons to default
			_titleText.text = string.Empty;
			_descriptorText.text = string.Empty;
			_progressImage.fillAmount = 0;

			_cancelButtonGO.SetActive(false);

			// Then disable ourselves until we next need to show
			gameObject.SetActive(false);
		}

		#endregion

		#region OnClickCallbacks

		public void OnCancelButtonClick()
		{
			// Call our cancel event
			_progressData.OnCancelEvent?.Invoke();
			// And hide ourselves
			Clear();
		}

		#endregion

	}

}