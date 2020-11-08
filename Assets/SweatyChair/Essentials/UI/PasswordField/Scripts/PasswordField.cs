using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class PasswordField : MonoBehaviour
	{

		#region Variables

		[SerializeField] private InputField _inputField;
		protected InputField inputField {
			get {
				if (_inputField == null) { _inputField = GetComponentInChildren<InputField>(true); }
				return _inputField;
			}
		}

		[SerializeField] private BetterToggle _showPasswordToggle;
		protected BetterToggle showPasswordToggle {
			get {
				if (_showPasswordToggle == null)
					_showPasswordToggle = GetComponentInChildren<BetterToggle>(true);
				SubscribeToToggleChange();
				return _showPasswordToggle;
			}
		}

		public bool interactable {
			get { return _inputField.interactable; }
			set { _inputField.interactable = value; }
		}

		private bool _subscribedToToggleChange = false;


		//Properties
		public bool IsPassShowing {
			get { return showPasswordToggle.isOn; }
			set { showPasswordToggle.isOn = value; }
		}

		public string PasswordText {
			get { return inputField.text; }
			set { inputField.text = value; }
		}

		#endregion

		#region Init

		private void Awake()
		{
			SubscribeToToggleChange();
			_showPasswordToggle.Set(false);
		}

		private void OnDestroy()
		{
			UnSubscribeFromToggleChange();
		}

		#region Subscribe to event

		private void SubscribeToToggleChange()
		{
			if (!_subscribedToToggleChange) {
				_showPasswordToggle.onValueChanged.AddListener(OnToggleChanged);
				_subscribedToToggleChange = true;
			}
		}

		private void UnSubscribeFromToggleChange()
		{
			if (_subscribedToToggleChange) {
				_showPasswordToggle.onValueChanged.RemoveListener(OnToggleChanged);
				_subscribedToToggleChange = false;
			}
		}

		#endregion

		#endregion

		#region OnToggleChange

		private void OnToggleChanged(bool value)
		{
			_inputField.contentType = (value) ? InputField.ContentType.Standard : InputField.ContentType.Password;
			_inputField.ForceLabelUpdate();
		}

		#endregion

	}

}