using SweatyChair.StateManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair.UI
{

	public class MessagePanel : SingletonPanel<MessagePanel>
	{

		#region Struct / Classes

		[System.Serializable]
		public class SerializedMessagePanelList
		{
			// We legit only do this to have a nice inspector. Godamn unity, just make Reorderable list easy to add to any array
			public MessagePanelPair[] subPanelPair = new MessagePanelPair[0];
		}

		[System.Serializable]
		public struct MessagePanelPair
		{
			public MessageFormat format;
			public MessageSubPanel panel;
		}

		#endregion

		public static event UnityAction onLastMessageShown;

		[Header("General")]
		[SerializeField] private GameObject _maskGO = null;
		[SerializeField] private bool _debugMode = false;

		[Header("Panels")]
		[SerializeField] SerializedMessagePanelList _panelList = new SerializedMessagePanelList();

		private Dictionary<MessageFormat, MessageSubPanel> _messagePanelDict;

		private Message _currentMessage;

		private List<Message> _messageStack = new List<Message>(); // First come, last serve

		public static bool isInstanceShown => instanceExists && instance.isShown;

		public static Message currentMessage => instanceExists ? instance._currentMessage : null;

		public override void Init()
		{
			MessageManager.messageShown += Show;
			MessageManager.messageInsertedToFirst += PushMessageToFirst;
			MessageManager.messagePushedToLast += PushMessageStack;
			MessageManager.lastMessageCancelled += PopMessageStack;
			MessageManager.messageStackCleared += ClearMessageStack;
			MessageManager.lastMessageShown += OnBackClick;

			// initialize our sub message panels
			InitializeSubPanels();
		}

		public override void Reset()
		{
			MessageManager.messageShown -= Show;
			MessageManager.messageInsertedToFirst -= PushMessageToFirst;
			MessageManager.messagePushedToLast -= PushMessageStack;
			MessageManager.lastMessageCancelled -= PopMessageStack;
			MessageManager.messageStackCleared -= ClearMessageStack;
			MessageManager.lastMessageShown -= OnBackClick;
		}

		private void Show(Message message)
		{
			if (_debugMode)
				Debug.LogFormat("MessagePanel:Show - message={0}, _previousMessages.Count={1}", message, _messageStack.Count);

			CursorManager.LockCursor(false); // Show cursor so user can click

			if (_currentMessage != null) { // Already showing another message
				if (_currentMessage.format == MessageFormat.QuickShop) { // Special case for QuickShop, which is not a message panel but showing currency panel => force to show new message and push the quick shop message to stack
					PushMessageStack(_currentMessage); // Push current quick shop message to stack
				} else {
					if (message.dontStackMessage) { // Force to show this message now and push current message to stack
						PushMessageStack(_currentMessage);
					} else { // Queue up this message after current message gone, and return now
						PushMessageStack(message);
						return;
					}
				}
			}

			_currentMessage = message;

			// Get our Panel which will be shown
			MessageSubPanel subPanel = GetSubPanel(_currentMessage.format);
			if (subPanel == null)
				return;

			// Hide all other sub Panels
			HideAllSubPanels();

			// Then set our current panel
			subPanel.Set(_currentMessage);

			// Show ourselves
			Show();
		}

		#region Message Sub Panel

		private void InitializeSubPanels()
		{
			// Initialize our Dictionary
			_messagePanelDict = new Dictionary<MessageFormat, MessageSubPanel>();

			// Go through and enumerate all of our data into our dict
			foreach (var panelPair in _panelList.subPanelPair) {

				if (_messagePanelDict.ContainsKey(panelPair.format)) {

					Debug.LogWarning($"{GetType()} : Duplicate key '{panelPair.format}' found in sub panel pair dictionary. Skipping!");
					continue;
				}

				// Then add our data to our dict
				_messagePanelDict.Add(panelPair.format, panelPair.panel);
			}

			// Once all our panels are added we good
			HideAllSubPanels();
		}

		private void HideAllSubPanels()
		{
			foreach (var item in _messagePanelDict)
				item.Value.Hide();
		}

		private MessageSubPanel GetSubPanel(MessageFormat format)
		{
			if (_messagePanelDict.TryGetValue(format, out MessageSubPanel panel))
				return panel;

			Debug.LogWarning($"{GetType()} : Unable to find key '{format}' in message dictionary. Make sure it has been added!");
			return null;
		}

		#endregion

		#region Common Functions

		public override void Toggle(bool show)
		{
			HideAllSubPanels();

			if (show) {

				base.Toggle(show);

				ToggleMask(_currentMessage.format != MessageFormat.QuickShop);

				MessageSubPanel subPanel = GetSubPanel(_currentMessage.format);
				if (subPanel != null)
					TimeManager.WaitForFrames(1, () => subPanel.Show());    // Legacy stuff, For some reason we were waiting a frame before showing, so we continue to do as the old gods say

				if (_currentMessage.setState)
					StateManager.Set(StateManagement.State.Message);

			} else if (_currentMessage == null) { // No message shown now, try to show a stacked message

				int messageCount = _messageStack.Count;
				if (messageCount > 0) { // Show queued message, if any
					Message message = _messageStack[messageCount - 1];
					_messageStack.RemoveAt(messageCount - 1); // Remove it from the queue
					message.Show();
				} else {
					// If there are no more messages to show, hide the panel and raise an event
					base.Toggle(show);
					onLastMessageShown?.Invoke();

					if (StateManager.Compare(State.Message))
						StateManagement.StateManager.SetAsLast();
				}

			} else {

				base.Toggle(show);
				if (StateManager.Compare(State.Message))
					StateManagement.StateManager.SetAsLast();

			}
		}

		private void ClearMessageStack()
		{
			_messageStack.Clear();
			_currentMessage = null;
			Toggle(false);
		}

		private void PopMessageStack()
		{
			if (_messageStack.Count > 0) {
				_messageStack.Remove(_messageStack[_messageStack.Count - 1]);
				_currentMessage = null;
			}
		}

		private void PushMessageStack(Message message)
		{
			if (_messageStack.Contains(message)) // Just in case
				_messageStack.Remove(message);
			_messageStack.Add(message);
		}

		private void PushMessageToFirst(Message message)
		{
			_messageStack.Insert(0, message); // First come, last serve
		}

		private void ToggleMask(bool doShow = true)
		{
			if (_maskGO == null) _maskGO = gameObject;
			_maskGO.SetActive(doShow);
		}

		#endregion

		#region Button Controls

		public override void OnBackClick()
		{
			CancelMessage();
		}

		public void ConfirmMessage()
		{
			Message message = _currentMessage;
			if (message != null && message.confirmCallback != null) {
				HideAllSubPanels(); // Hide all UIs first just in case
				message.confirmCallback();
			}
			if (message == _currentMessage) { // No extra message added in _confirmAction
				_currentMessage = null;
				Toggle(false);
			}
		}

		public void CancelMessage()
		{
			Message message = _currentMessage;
			if (message != null && message.cancelCallback != null) {
				HideAllSubPanels(); // Hide all UIs first just in case
				message.cancelCallback();
			}
			if (message == _currentMessage) { // No extra message added in _cancelledAction
				_currentMessage = null;
				Toggle(false);
			}
		}

		public void ConfirmExtraMessage()
		{
			Message message = _currentMessage;
			if (message != null && message.extraCallback != null) {
				HideAllSubPanels(); // Hide all UIs first just in case
				message.extraCallback();
			}
			if (message == _currentMessage) { // No extra message added in _extraAction action
				_currentMessage = null;
				Toggle(false);
			}
		}

		public void ConfirmInputMessage(string input)
		{
			Message message = _currentMessage;
			if (message != null && message.inputConfirmCallback != null) {
				HideAllSubPanels(); // Hide all UIs first just in case
				message.inputConfirmCallback(input.Trim());
			}
			if (message == _currentMessage) { // No extra message added in _confirmStringAction
				_currentMessage = null;
				Toggle(false);
			}
		}

		#endregion

#if UNITY_EDITOR

		[ContextMenu("Print Parameters")]
		private void PrintParameters()
		{
			Debug.LogFormat("_previousMessages.Count={0}:", _messageStack.Count);
			foreach (Message message in _messageStack)
				Debug.Log(message);
			DebugUtils.Log(_currentMessage, "_currentMessage");
		}

#endif

	}

}