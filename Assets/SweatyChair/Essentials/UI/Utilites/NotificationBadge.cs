using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SweatyChair;

public class NotificationBadge : MonoBehaviour
{

	#region Consts

	public const string PP_ALL_BADGE_KEYS = "all_badge_keys";

	#endregion

	#region Events

	public static event UnityAction<string> AddBadgeRequestEvent;
	public static event UnityAction<string> RemoveBadgeRequestEvent;

	#endregion

	#region Variables

	[Header("Required")]
	[SerializeField] private GameObject _badgeObject;

	[Header("Settings")]
	[Tooltip("Unique key used to referenece this badge, please make sure the key is not used in anywhere else.")]
	[SerializeField] private string _badgeKey = "badge_unique_key";
	[SerializeField] private bool _clearBadgeOnUiInteract = true;

	// References to the UI we are attached to
	private Button _attachedButton;
	private Toggle _attachedToggle;

	#endregion

	#region Static Methods

	/// <summary>
	/// Requests a badge to be shown for a given unique key.
	/// </summary>
	/// <param name="badgeKey">Badge Key for object. Try to use the prefix 'badge_' to help separate badge names from other settings</param>
	public static void AddBadge(string badgeKey)
	{
		// Check if our playerprefs already has an entry for this key. And if not add one
		if (!HasBadge(badgeKey))
			PlayerPrefs.SetInt(badgeKey, 1);

		// Add it to the list of all of our badges
		AddToAllBadgeKeys(badgeKey);

		// Send our badgeRequest event
		AddBadgeRequestEvent?.Invoke(badgeKey);
	}

	/// <summary>
	/// Requests a badge to be removed for a given unique key
	/// </summary>
	/// <param name="badgeKey">Badge Key for object. Try to use the prefix 'badge_' to help separate badge names from other settings</param>
	public static void RemoveBadge(string badgeKey)
	{
		// Check if our playerprefs has an entry for this key, if not return
		if (!HasBadge(badgeKey))
			return;

		// Delete all references we have to this key
		PlayerPrefs.DeleteKey(badgeKey);
		RemoveFromAllBadgeKeys(badgeKey);

		// Send our Badge Remove event
		RemoveBadgeRequestEvent?.Invoke(badgeKey);
	}

	/// <summary>
	/// Requests that all badges should be removed.
	/// </summary>
	public static void DismissAllBadges()
	{
		// Go through all of our Badge keys and remove our badges
		string[] allBadgeKeys = GetAllBadgeKeys();
		for (int i = 0; i < allBadgeKeys.Length; i++) {
			RemoveBadge(allBadgeKeys[i]);
		}
	}

	/// <summary>
	/// Gets whether we have a badge for a given badge key
	/// </summary>
	/// <param name="badgeKey">Badge Key for object. Try to use the prefix 'badge_' to help separate badge names from other settings</param>
	/// <returns></returns>
	public static bool HasBadge(string badgeKey)
	{
		return PlayerPrefs.HasKey(badgeKey);
	}

	#region All Badge Keys

	private static string[] GetAllBadgeKeys()
	{
		if (PlayerPrefs.HasKey(PP_ALL_BADGE_KEYS))
			return DataUtils.GetStringArray(PlayerPrefs.GetString(PP_ALL_BADGE_KEYS));

		return new string[0];
	}

	/// <summary>
	/// Adds a unique key to our badge list so we can manage it later on.
	/// </summary>
	/// <param name="badgeKey"></param>
	private static void AddToAllBadgeKeys(string badgeKey)
	{
		string[] allBadgeKeys = GetAllBadgeKeys();
		if (!allBadgeKeys.Contains(badgeKey)) {
			// Add our badge to our array
			List<string> allBadgeKeyList = new List<string>(allBadgeKeys);
			allBadgeKeyList.Add(badgeKey);

			// Then Set to our list
			PlayerPrefs.SetString(PP_ALL_BADGE_KEYS, string.Join("|", allBadgeKeyList));
		}
	}

	/// <summary>
	/// Removes a unique key from our badge list, so we no longer need to manage it.
	/// </summary>
	/// <param name="badgeKey"></param>
	private static void RemoveFromAllBadgeKeys(string badgeKey)
	{
		string[] allBadgeKeys = GetAllBadgeKeys();
		if (allBadgeKeys.Contains(badgeKey)) {
			// Remove our badge from our array
			List<string> allBadgeKeyList = new List<string>(allBadgeKeys);
			allBadgeKeyList.Remove(badgeKey);

			// Then Set to our list
			PlayerPrefs.SetString(PP_ALL_BADGE_KEYS, string.Join("|", allBadgeKeyList));
		}
	}

	#endregion

	#endregion

	#region Public Methods

	/// <summary>
	/// Requests a badge for this current notification. 
	/// </summary>
	public void GiveBadge()
	{
		AddBadge(_badgeKey);
	}

	/// <summary>
	/// Requests a badge to be removed from this current notification
	/// </summary>
	public void ClearBadge()
	{
		RemoveBadge(_badgeKey);
	}

	#endregion

	#region On Enable / On Disable

	private void OnEnable()
	{
		// Subscribe to our notification events
		AddBadgeRequestEvent += OnAddBadgeRequested;
		RemoveBadgeRequestEvent += OnRemoveBadgeRequested;

		// Then make sure we have the correct initial sate on show.
		ToggleNotification();

		// If needed Get a Reference to our Buttons and Toggles
		if (_clearBadgeOnUiInteract)
			SubscribeToUIEvents();
	}

	private void SubscribeToUIEvents()
	{
		_attachedButton = GetComponent<Button>();
		if (_attachedButton != null)
			_attachedButton.onClick.AddListener(OnButtonClicked);

		_attachedToggle = GetComponent<Toggle>();
		if (_attachedToggle != null)
			_attachedToggle.onValueChanged.AddListener(OnToggleToggled);
	}

	private void OnDisable()
	{
		// Unsubscribe from our notification events
		AddBadgeRequestEvent -= OnAddBadgeRequested;
		RemoveBadgeRequestEvent -= OnRemoveBadgeRequested;

		// Unsubscribe from our UI events
		UnsubscribeFromUIEvents();
	}

	private void UnsubscribeFromUIEvents()
	{
		if (_attachedButton != null)
			_attachedButton.onClick.RemoveListener(OnButtonClicked);

		if (_attachedToggle != null)
			_attachedToggle.onValueChanged.RemoveListener(OnToggleToggled);
	}

	#endregion

	#region Event Callbacks

	private void OnAddBadgeRequested(string badgeKey)
	{
		ToggleNotification();
	}

	private void OnRemoveBadgeRequested(string badgeKey)
	{
		ToggleNotification();
	}

	#endregion

	#region UI Callbacks

	private void OnToggleToggled(bool toggle)
	{
		ClearBadge();
	}

	private void OnButtonClicked()
	{
		ClearBadge();
	}

	#endregion

	#region Toggle Notification

	public void ToggleNotification()
	{
		// Set our badge object to active if we have a badge for this key
		_badgeObject.SetActive(HasBadge(_badgeKey));
	}

	#endregion

	#region Editor Methods

#if UNITY_EDITOR

	[ContextMenu("Give Badge")]
	private void EDITOR_GiveSelfBadge()
	{
		GiveBadge();
	}

	[ContextMenu("Clear Badge")]
	private void EDITOR_ClearSelfBadge()
	{
		ClearBadge();
	}

#endif

	#endregion
}
