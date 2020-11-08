using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class LoadingPanel : Panel
	{

		private static LoadingPanel _instance;

		[SerializeField] private Text _contentText, _extraText;
		[SerializeField] private Transform _progressBarTF;

		[SerializeField] private GameObject _cancelButtonGO;
		[SerializeField] private Text _cancelButtonText;

		[SerializeField] private bool _isCrossScene = true;

		private Loading _loading;

		private void OnDisable()
		{
			CancelInvoke();
		}

		public override void Init()
		{
			if (_isCrossScene) {
				if (_instance != null) {
					Destroy(this);
					return;
				}
				DontDestroyOnLoad(gameObject);
			}
			_instance = this;
			LoadingManager.loadingShown += Show;
			LoadingManager.loadingPogressUpdated += UpdateProgress;
			LoadingManager.loadingContentUpdated += UpdateContent;
			LoadingManager.onLoadingHidden += Hide;
		}

		public override void Reset()
		{
			LoadingManager.loadingShown -= Show;
			LoadingManager.loadingPogressUpdated -= UpdateProgress;
			LoadingManager.loadingContentUpdated -= UpdateContent;
			LoadingManager.onLoadingHidden -= Hide;
		}

		private void Update()
		{
			if (_loading != null && _loading.checkHidePredicate != null) {
				if (_loading.checkHidePredicate(shownDuration))
					Hide();
			}

			// Check if we are recieving a back action - and are able to cancel, invoke our cancel event
			if (Input.GetKeyUp(KeyCode.Escape)) {
				if (_loading != null && _loading.cancelCallback != null)
					OnCancelClick();
			}
		}

		private void Show(Loading loading)
		{
			base.Show();
			_loading = loading;
			_contentText.text = string.IsNullOrEmpty(loading.content) ? LocalizeUtils.Get("Loading") : loading.content;
			InvokeRepeating("UpdateText", 1f, 1f);
			if (_cancelButtonGO != null) {
				_cancelButtonGO.SetActive(loading.cancelCallback != null);
				if (_cancelButtonText && loading.cancelCallback != null)
					_cancelButtonText.text = loading.cancelButtonText ?? LocalizeUtils.Get(TermCategory.Button, "Cancel");
			}
			_progressBarTF.parent.gameObject.SetActive(loading.hasProgressBar);
			UpdateProgress(0);
			if (_extraText)
				_extraText.text = loading.extraText;
		}

		public override void Hide()
		{
			base.Hide();
			CancelInvoke();
		}

		private void UpdateProgress(float progress)
		{
			progress = Mathf.Clamp(progress, 0, 1);
			_progressBarTF.localScale = new Vector3(progress, _progressBarTF.localScale.y, 1);
		}

		private void UpdateContent(string content)
		{
			_loading.content = content;
			_contentText.text = string.IsNullOrEmpty(content) ? LocalizeUtils.Get("Loading") : content;
		}

		private void UpdateText()
		{
			string content = _contentText.text;
			string rawContent = content.Trim('.');
			int dotCount = content.Length - rawContent.Length;
			if (dotCount >= 4)
				dotCount = -1;
			_contentText.text = rawContent + new string('.', dotCount + 1);
		}

		public void OnCancelClick()
		{
			_loading.cancelCallback?.Invoke();
			Hide();
		}

	}

}