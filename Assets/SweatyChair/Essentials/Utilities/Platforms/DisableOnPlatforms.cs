using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Simply disables current game object on specific platforms.
	/// </summary>
	public class DisableOnPlatforms : MonoBehaviour
	{

		#region Variables

		[Header("Settings")]
		[SerializeField] private bool destroyObject;

		[Header("Platforms")]
		[SerializeField] private bool disableOnStandalone;
		[SerializeField] private bool disableOnMac;
		[SerializeField] private bool disableOnWindows;
		[SerializeField] private bool disableOnLinux;
		[Space]
		[SerializeField] private bool disableOnMobile;
		[SerializeField] private bool disableOnIOS;
		[SerializeField] private bool disableOnAndroid;
		[SerializeField] private bool disableOnTizen;
		[Space]
		[SerializeField] private bool disableOnTvOS;
		[Space]
		[SerializeField] private bool disableOnConsole;
		[SerializeField] private bool disableOnWii;
		[SerializeField] private bool disableOnPs4;
		[SerializeField] private bool disableOnXbox;

		#endregion

		#region Start

		private void Start()
		{

			#region Standalone

#if UNITY_STANDALONE
		if (disableOnStandalone)
			DisableContent();
#endif

#if UNITY_STANDALONE_MAC
		if (disableOnMac)
			DisableContent();
#endif

#if UNITY_STANDALONE_WIN
		if (disableOnWindows)
			DisableContent();
#endif

#if UNITY_STANDALONE_LINUX
		if (disableOnLinux)
			DisableContent();
#endif

			#endregion

			#region Mobile

#if UNITY_IOS || UNITY_ANDROID || UNITY_TIZEN
			if (disableOnMobile)
				DisableContent();
#endif

#if UNITY_IOS
			if (disableOnIOS)
				DisableContent();
#endif

#if UNITY_ANDROID
		if (disableOnAndroid)
			DisableContent();
#endif

#if UNITY_TIZEN
		if (disableOnTizen)
			DisableContent();
#endif

			#endregion

			#region TVOS

#if UNITY_TVOS
		if (disableOnTvOS)
			DisableContent();
#endif

			#endregion

			#region Console

#if UNITY_WII || UNITY_PS4 || UNITY_XBOXONE
		if (disableOnConsole)
			DisableContent();
#endif

#if UNITY_WII
		if (disableOnWii)
			DisableContent();
#endif

#if UNITY_PS4
		if (disableOnPs4)
			DisableContent();
#endif

#if UNITY_XBOXONE
		if (disableOnXbox)
			DisableContent();
#endif

			#endregion

		}

		#endregion

		#region Disable

		private void DisableContent()
		{
			// If we have toggled destroy, we instead just destroy ourselves
			if (destroyObject) {
				Destroy(gameObject);
			} else {
				// First we check if we are governed by a toggle children one by one, we get it and ignore it
				ToggleChildrenOneByOne parentToggle = transform.parent?.GetComponent<ToggleChildrenOneByOne>();
				if (parentToggle != null)
					parentToggle.AddToIgnoreList(gameObject);
				//Then toggle our object off
				gameObject.SetActive(false);
			}
		}

		#endregion

	}

}