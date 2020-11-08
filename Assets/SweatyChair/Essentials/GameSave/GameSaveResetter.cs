using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A resetter to reset GameSave, use this very extra caution.
	/// </summary>
	public class GameSaveResetter : MonoBehaviour
	{

		private void Awake()
		{
			if (PlayerPrefs.GetInt("GameSaveResset") == 0) {
				ES3.DeleteFile();
				PlayerPrefs.SetInt("GameSaveResset", 1);
			}
		}

	}

}