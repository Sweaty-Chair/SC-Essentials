using UnityEngine;
using System.Collections;
using SweatyChair.UI;

namespace SweatyChair
{

	public class InitializeCheckManager : PersistentSingleton<InitializeCheckManager>
	{

		[SerializeField] private int[] androidSignatureHashs;

		private static bool _isChecked = false;

		private void Start()
		{
			if (_isChecked)
				return;

#if !UNITY_EDITOR // Just to skip checking on Editor to save server brandwidth
			StartVersionCheck();
#if UNITY_ANDROID
			StartAndroidIllegalCopyCheck();
#endif
#endif

			_isChecked = true;
		}

		#region Version Check

		private void StartVersionCheck()
		{
			ServerManager.Get("games/{gameId}",
				(Hashtable ht) => {
					if (ht == null) // Just in case
						return;
					string serverVersionCodeString = "";
#if UNITY_IOS
					serverVersionCodeString = ht["version_ios"].ToString();
#elif UNITY_ANDROID
					serverVersionCodeString = ht["version_android"].ToString();
#elif UNITY_TVOS
					serverVersionCodeString = ht["version_tvos"].ToString ();
#endif
					int serverVersionCode = System.Convert.ToInt32(serverVersionCodeString);
					if (serverVersionCode > 0 && StringUtils.ParseToInt(Application.version) < serverVersionCode)
						ShowUpdateWarning();
				}
			);
		}

		[ContextMenu("Debug show update warning")]
		private void ShowUpdateWarning()
		{
			new Message {
				title = LocalizeUtils.Get(TermCategory.Message, CommonTexts.MSG_NEW_UPDATE_TITLE),
				content = LocalizeUtils.Get(TermCategory.Message, CommonTexts.MSG_NEW_UPDATE_CONTENT),
				confirmCallback = ConfirmUpdate,
			}.Show();
		}

		private void ConfirmUpdate()
		{
			StoreReviewManager.OpenStore();
			Invoke("ShowUpdateWarning", 0.5f);
		}

		#endregion

		#region Android Illegal Copy Check

#if UNITY_ANDROID && !UNITY_EDITOR
		
		private const string PREFS_CHECK_HACK = "CheckHack";

		private void StartAndroidIllegalCopyCheck()
		{
			Debug.LogFormat("AndroidUtils.signatureHash={0}", AndroidUtils.signatureHash);
		
#if !CHS // Don't check if that's released in China, because Chinese app stores are too complacated
			ServerManager.Get("settings/check_hack?game_id={gameId}",
				(Hashtable ht) => {
					if (ht == null) // Just in case
						return;
					int checkHack = DataUtils.GetInt(ht["value"] as string);
					if (checkHack == 1)
						CheckAndroidIllegalCopy();
					PlayerPrefs.SetInt(PREFS_CHECK_HACK, checkHack);
				},
				(string error) => { // No internet, use last setting
					if (PlayerPrefs.GetInt(PREFS_CHECK_HACK) == 1)
						CheckAndroidIllegalCopy();
				}
			);
#endif
		}

		private void CheckAndroidIllegalCopy()
		{
			if (androidSignatureHashs.Length > 0 && System.Array.IndexOf(androidSignatureHashs, AndroidUtils.signatureHash) <= -1) {
				new Message {
					title = LocalizeUtils.Get(TermCategory.Message, CommonTexts.MSG_ILLEGAL_COPY_TITLE),
					content = LocalizeUtils.GetFormat(TermCategory.Message, CommonTexts.MSG_ILLEGAL_COPY_CONTENT),
					confirmCallback = CheckAndroidIllegalCopy,
					cancelCallback = CheckAndroidIllegalCopy
				}.Show();
			}
		}

#endif

		#endregion

	}

}