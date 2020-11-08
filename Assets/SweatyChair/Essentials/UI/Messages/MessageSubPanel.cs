using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public abstract class MessageSubPanel : Slot<Message>
	{

		#region Variables

		protected Message _currentMessage;

		#endregion

		#region Set Override

		public sealed override void Set(Message t)
		{
			base.Set(t);

			_currentMessage = t;

			OnSet();
		}

		#endregion

		#region Public

		public void Show()
		{
			Toggle(true);
		}

		public void Hide()
		{
			Toggle(false);
		}

		#endregion

		#region Message Sub Panel Callbacks

		/// <summary>
		/// Called when we activate this message panel and have provided it data. 
		/// </summary>
		protected abstract void OnSet();

		#endregion

		#region UI Callbacks

		public virtual void OnConfirmClick()
		{
			CallInParent((panel) => panel.ConfirmMessage());
		}

		public virtual void OnCancelClick()
		{
			CallInParent((panel) => panel.CancelMessage());
		}

		public virtual void OnExtraClick()
		{
			CallInParent((panel) => panel.ConfirmExtraMessage());
		}

		#endregion

		#region Utility

		#region Set Utility

		protected void ToggleGameObject(GameObject obj, bool value)
		{
			if (obj == null)
				return;

			obj.SetActive(value);
		}

		protected void SetTemporaryImage(TemporaryImageLoader loader, Sprite icon)
		{
			SetTemporaryImage(loader, icon, string.Empty);
		}
		protected void SetTemporaryImage(TemporaryImageLoader loader, string location)
		{
			SetTemporaryImage(loader, null, location);
		}
		/// <summary>
		/// Wrapper for setting Temporary image sprites. Used to easily handle nulls without having to explicitly define a null check
		/// </summary>
		/// <param name="loader"></param>
		/// <param name="icon"></param>
		/// <param name="location"></param>
		protected void SetTemporaryImage(TemporaryImageLoader loader, Sprite icon, string location)
		{
			if (loader == null)
				return;

			// Enforce it is active
			loader.gameObject.SetActive(true);

			// If we have a sprite, use the sprite
			if (icon != null)
				loader.SetFromExisting(icon);

			// If we have a location, use a location
			else if (!string.IsNullOrEmpty(location))
				loader.SetFromResources(location);

			// Otherwise disable our loader
			else
				loader.gameObject.SetActive(false);
		}


		/// <summary>
		/// Wrapper for setting images. Used to easily handle nulls without having to explicitly define a null check
		/// </summary>
		/// <param name="image"></param>
		/// <param name="sprite"></param>
		protected void SetImage(Image image, Sprite sprite)
		{
			if (image == null)
				return;

			// Toggle and assign our sprite
			image.gameObject.SetActive(sprite != null);
			image.sprite = sprite;
		}


		/// <summary>
		/// Wrapper for setting text on Text UI objects. Used to handle nulls without having to do an if check in line
		/// </summary>
		/// <param name="text"></param>
		/// <param name="content"></param>
		protected void SetText(Text text, string content, string defaultContent = "")
		{
			if (text == null)
				return;

			// Toggle and assign our Text
			text.text = (!string.IsNullOrEmpty(content)) ? content : defaultContent;

			// Toggle our Game Object on or off based on whether we have an empty string or not
			text.gameObject.SetActive(!string.IsNullOrEmpty(text.text));
		}

		/// <summary>
		/// Wrapper for setting text on Inputfield UI objects. Used to handle nulls without having to do an if check in line
		/// </summary>
		/// <param name="inputField"></param>
		/// <param name="content"></param>
		protected void SetInputField(InputField inputField, string content, string defaultContent = "")
		{
			if (inputField == null)
				return;

			// Toggle and assign our Text
			inputField.text = (!string.IsNullOrEmpty(content)) ? content : defaultContent;
		}

		#endregion

		protected void CallInParent(UnityAction<MessagePanel> panelAction)
		{
			MessagePanel panelParent = GetComponentInParent<MessagePanel>();
			if (panelParent != null)
				panelAction?.Invoke(panelParent);
			else
				Debug.LogError($"{GetType()}: Unable to invoke Panel Manager Method, Could not find parent message panel!", this);
		}

		#endregion

	}

}
