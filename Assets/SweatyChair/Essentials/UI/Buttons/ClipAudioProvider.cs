using UnityEngine;

namespace SweatyChair.UI
{

	[CreateAssetMenu(menuName = "Sweaty Chair/UI/Clip Audio Provider", fileName = "Clip Audio Provider", order = 102)]
	public class ClipAudioProvider : BouncyButtonAudioProvider
	{

		#region Variables

		[Header("Audio Clips")]
		[SerializeField] protected AudioClip[] _audioClips = null;
		[SerializeField] [Range(0, 1)] protected float _volume = 1; // Default Clip Audio

		#endregion

		#region Play SFX

		public override void PlaySFX()
		{
			// For now we keep it simple as heck, and just play an audio clip with our provided settings
			if (_audioClips.Length > 0)
				PlaySFX(_audioClips.GetRandom());
			else
				Debug.LogError($"{GetType()}: Could not play SFX , no clips assigned.", this);
		}

		public virtual void PlaySFX(AudioClip clip)
		{
			// Approximate a 2D audio source, by playing an audio clip at our camera's positions
			AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, _volume);
		}

		#endregion

	}

}
