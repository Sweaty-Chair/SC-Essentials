using UnityEngine.Events;

namespace SweatyChair.UI
{

	public static class MessageManager
	{

		public static event UnityAction<Message> messageShown; // Show and add a message to the stack (push and show)
		public static event UnityAction<Message> messagePushedToLast; // Add a message to the top of the message stack without showing (push and not show)
		public static event UnityAction<Message> messageInsertedToFirst; // Add a message to the bottom of message stack without showing (insert to first and not show)

		public static event UnityAction lastMessageShown; // Show the last message in the stack (pop and show)
		public static event UnityAction lastMessageCancelled; // Cancel the last message in the stack (pop and cancel)
		public static event UnityAction messageStackCleared; // Clear the entire message stack

		private static Message _messageOnEnable;

		public static void OnUIEnabled()
		{
			if (_messageOnEnable != null)
			{
				if (messageShown != null)
					Show(_messageOnEnable);
				_messageOnEnable = null;
			}
		}

		/// <summary>
		/// Show a message and add it to stack.
		/// </summary>
		public static void Show(Message message)
		{
			if (messageShown == null || message.showOnEnableOnly) // No UI instance presented, or message is shown on enable only
				_messageOnEnable = message;
			else
				messageShown(message);
		}

		/// <summary>
		/// Stack up a message to the first stack. Note that message is first come last serve, so putting it to first mean it will be shown at last.
		/// </summary>
		public static void StackMessageToFirst(Message message)
		{
			messageInsertedToFirst?.Invoke(message);
		}

		/// <summary>
		/// Push down a message to the last stack. Note that message is first come last serve, so putting it to last mean it will be shown at first.
		/// </summary>
		public static void PushMessageToStack(Message message)
		{
			messagePushedToLast?.Invoke(message);
		}
		
		public static void ShowLastMessage()
		{
			lastMessageShown?.Invoke();
		}

		public static void CancelLastMessage()
		{
			lastMessageCancelled?.Invoke();
		}
		
		public static void ClearMessageStack()
		{
			messageStackCleared?.Invoke();
		}

	}

}