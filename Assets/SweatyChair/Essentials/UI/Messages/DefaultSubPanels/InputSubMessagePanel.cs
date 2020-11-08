using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class InputSubMessagePanel : MessageSubPanel
	{

		#region Variables

		[Header("Core")]
		[SerializeField] private Text _titleText = null;
		[SerializeField] private Text _contentText = null;

		[Header("Input")]
		[SerializeField] private InputField _inputField = null;

		[Header("Buttons")]
		[SerializeField] private Text _cancelButtonText = null;
		[SerializeField] private Text _confirmButtonText = null;


		#endregion

		#region Set

		protected override void OnSet()
		{
			SetText(_titleText, _currentMessage.title);
			SetText(_contentText, _currentMessage.content);

			SetInputField(_inputField, _currentMessage.inputString);
			SetText((Text)_inputField.placeholder, LocalizeUtils.Get(TermCategory.Message, _currentMessage.inputPlaceholderString), LocalizeUtils.Get(TermCategory.Message, "Enter here"));

			SetText(_cancelButtonText, LocalizeUtils.Get(TermCategory.Button, _currentMessage.cancelButtonText), LocalizeUtils.Get(TermCategory.Button, "Cancel"));
			SetText(_confirmButtonText, LocalizeUtils.Get(TermCategory.Button, _currentMessage.confirmButtonText), LocalizeUtils.Get(TermCategory.Button, "Enter"));

			// Set our validation Data
			_inputField.SetValidationData(_currentMessage.inputValidationData);
		}

		private void ActivateInputField()
		{
			_inputField.Select();
			_inputField.ActivateInputField();
		}

		#endregion

		#region OnEnable / OnDisable

		protected override void OnEnable()
		{
			base.OnEnable();

			ActivateInputField();   // InputField's ActivateInputField need to be run when the game object is active
		}

		#endregion


		#region UI Callbacks

		public override void OnConfirmClick()
		{
			CallInParent((panel) => panel.ConfirmInputMessage(_inputField.text));
		}

		public void OnInputChange(string text)
		{
			if (_currentMessage.inputValidationData.removeSpecialCharacters) {
				text = System.Text.RegularExpressions.Regex.Replace(text, "[^a-zA-Z0-9_. /-]+", "", System.Text.RegularExpressions.RegexOptions.Compiled);
				_inputField.text = text;
			}
			_currentMessage?.inputChangeCallback?.Invoke(text);
		}

		#endregion

	}

}
