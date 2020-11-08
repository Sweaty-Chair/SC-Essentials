using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace SweatyChair.UI
{

	public enum MessageFormat
	{
		Notice, // Only "OK" button
		Confirm, // Ok and X button
		Decision, // 2 Choices and an X
		ConfirmPurchase, // Confirm to purchase
		QuickShop, // Use to show currency shop with restriction
		Reward, // Deprecated
		Input,  // Allow for user to input a string
		ResourceConfirm, // Use for buying or upgrading stuff that requires resources
		Extra, // Format that not fitting the rest
		DecisionAd, // 2 Choices and an X, to decide watching an ad
	}

	//[System.Serializable]
	public class Message
	{

		public MessageFormat format { get; set; }

		public string title { get; set; }

		public string content { get; set; }

		public string extraContent { get; set; }

		public string extraContent2 { get; set; }

		public string confirmButtonText { get; set; }

		public string cancelButtonText { get; set; }

		public string extraButtonText { get; set; }

		public Color confirmButtonColor { get; set; }

		public Color cancelButtonColor { get; set; }

		public string iconSpriteName { get; set; }

		public string confirmIconSpriteName { get; set; }

		public Sprite iconSprite { get; set; }

		public string inputString { get; set; }

		public string inputPlaceholderString { get; set; }

		public UnityAction confirmCallback { get; set; }

		public UnityAction<List<Item>> confirmCallbackWithItems { get; set; }

		public UnityAction cancelCallback { get; set; }

		public UnityAction extraCallback { get; set; }

		public UnityAction quickShopSuccessCallback { get; set; }

		public UnityAction<string> inputChangeCallback { get; set; }

		public UnityAction<string> inputConfirmCallback { get; set; }

		/// <summary>
		/// Shop data when buying an item, used in PurchaseConfirm.
		/// </summary>
		public ShopData shopData { get; set; }

		/// <summary>
		/// Cost currency when buying an item, used in PurchaseConfirm and QuickShop.
		/// </summary>
		public CurrencyItem cost { get; set; }

		/// <summary>
		/// Item type when buying without enough currency, used in QuickShop.
		/// </summary>
		public ItemType buyingItemType { get; set; }

		/// <summary>
		/// List of shop data when buying without enough currency, Used in QuickShop.
		/// </summary>
		public List<ShopData> buyingShopDataList { get; set; }

		/// <summary>
		/// List of rewaded items, used for Reward.
		/// </summary>
		public List<Item> rewardItemList { get; set; }

		/// <summary>
		/// Rewaded item, used for Reward.
		/// </summary>
		public Item rewardItem { get; set; }

		/// <summary>
		/// Used in String input, used for InputString.
		/// </summary>
		public InputFieldValidationData inputValidationData { get; set; }

		/// <summary>
		/// Only only on next enable, used for notice on scene load, e.g. show kicked notice when back to main menu after AFK.
		/// </summary>
		public bool showOnEnableOnly { get; set; }

		/// <summary>
		/// Clicking background is same as clicking cancel button, disable this behaviour using this bolean.
		/// </summary>
		public bool isBackgroundHideDisabled { get; set; }

		/// <summary>
		/// Disable stacking this message, used for messages such as not enough coins/gems which we won't want to show again when player going back from futher messages.
		/// Enabling this will push the current message (if any) into stack, and show this message immediately
		/// </summary>
		public bool dontStackMessage { get; set; }

		/// <summary>
		/// Disable showing quick shop panel if it's in stack.
		/// </summary>
		public bool dontShowPreviousQuickShop { get; set; }

		/// <summary>
		/// Used for confirm purchase so that it won't close the message box after purchase failed.
		/// </summary>
		public bool dontCloseOnFailed { get; set; }

		/// <summary>
		/// Used in Reward, such as weapon.
		/// </summary>
		public bool hasRewardExtra { get; set; }

		/// <summary>
		/// Used in Reward, such as lucky title.
		/// </summary>
		public bool hasTitleExtra { get; set; }

		/// <summary>
		/// Set the state to State.Message, useful for in-game message where you want the state to be a non-Game state, so player control is disabled, etc.
		/// </summary>
		public bool setState { get; set; }

		public void Show()
		{
			MessageManager.Show(this);
		}

		public override string ToString()
		{
			return string.Format("[Message: format={0}, title={1}, content={2}, confirmButtonText={3}, cancelButtonText={4}, extraButtonText={5}, iconSpriteName={6}, confirmCallback={7}, cancelCallback={8}, extraCallback={9}, cost={10}, buyingItemType={11}, buyingShopDataList={12}, shopData={13}, showOnEnableOnly={14}, isBackgroundHideDisabled={15}]", format, title, content, confirmButtonText, cancelButtonText, extraButtonText, iconSpriteName, confirmCallback, cancelCallback, extraCallback, cost, buyingItemType, StringUtils.ListToString(buyingShopDataList), shopData, showOnEnableOnly, isBackgroundHideDisabled);
		}

	}

}