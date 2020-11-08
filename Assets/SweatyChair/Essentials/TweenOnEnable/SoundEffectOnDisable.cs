using UnityEngine;
using UnityEngine.Serialization;

public class SoundEffectOnDisable : MonoBehaviour
{

	[FormerlySerializedAs("_soundEffect")] // 20.02.01
	[SerializeField] private AudioClip _audioClip;

	private void OnDisable()
	{
		if (_audioClip != null)
			SoundManager.PlaySFX(_audioClip);
	}

}