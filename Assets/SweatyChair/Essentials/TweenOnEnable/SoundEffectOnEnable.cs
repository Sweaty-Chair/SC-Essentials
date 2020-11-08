using UnityEngine;
using UnityEngine.Serialization;

public class SoundEffectOnEnable : MonoBehaviour
{

	[FormerlySerializedAs("_soundEffect")] // 20.02.01
	[SerializeField] private AudioClip _audioClip;

	private void OnEnable()
	{
		if (_audioClip != null)
			SoundManager.PlaySFX(_audioClip);
	}

}