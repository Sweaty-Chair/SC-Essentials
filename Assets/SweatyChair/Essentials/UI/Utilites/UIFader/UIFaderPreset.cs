using UnityEngine;

namespace SweatyChair.UI
{

	[CreateAssetMenu(menuName = "Sweaty Chair/UI/UIFaderPreset", fileName = "UIFaderPreset", order = 201)]
	public class UIFaderPreset : BaseFaderPreset
	{
		#region Variables

		[Header("Settings")]
		[SerializeField] private bool _isInteractableWhenInvisible = true;
		[SerializeField] private bool _blocksRaycastsWhenInvisible = true;

		// Public getters
		public bool isInteractableWhenInvisible => _isInteractableWhenInvisible;
		public bool blocksRaycastsWhenInvisible => _blocksRaycastsWhenInvisible;

		#endregion
	}

}