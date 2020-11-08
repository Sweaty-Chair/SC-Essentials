using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	// AUTHOR: RV
	[CreateAssetMenu(menuName = "Sweaty Chair/UI/BouncyButtonPreset", fileName = "ButtonButtonPreset", order = 101)]
	public class BouncyButtonPreset : ScriptableObject
	{

		[Header("Settings")]
		public bool eatInteractionEvents = false;

		[Header("Ease Function")]
		[FormerlySerializedAs("allowHighlighting")] public bool allowHover = false;
		public EaseInterpBlock curves = EaseInterpBlock.defaultCurveBlock;

		[Header("Utility")]
		public bool overrideSortingOrder = false;
		public int normalSortingOrder = -1;
		public int hoveredSortingOrder = 5;

		[Header("Audio")]
		public BouncyButtonAudioProvider hoverAudioProvider = null;
		public BouncyButtonAudioProvider clickAudioProvider = null;
	}

}