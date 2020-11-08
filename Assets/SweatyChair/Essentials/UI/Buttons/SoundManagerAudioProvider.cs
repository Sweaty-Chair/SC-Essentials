using SweatyChair.UI;
using UnityEngine;

[CreateAssetMenu(menuName = "Sweaty Chair/UI/Sound Manager Audio Provider", fileName = "Sound Manager Audio Provider", order = 103)]
public class SoundManagerAudioProvider : ClipAudioProvider
{

	#region Play SFX

	public override void PlaySFX(AudioClip clip)
	{
		SoundManager.PlaySFX(clip, volume: _volume);
	}

	#endregion

}
