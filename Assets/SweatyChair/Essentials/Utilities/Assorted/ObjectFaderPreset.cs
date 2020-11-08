using UnityEngine;

namespace SweatyChair
{

	[CreateAssetMenu(menuName = "Sweaty Chair/UI/ObjectFaderPreset", fileName = "ObjectFaderPreset", order = 202)]
	public class ObjectFaderPreset : BaseFaderPreset
	{
		#region Variables

		[Header("Settings")]
		[SerializeField] private bool _getChildrenRenderers = true;
		[SerializeField] private bool _disableRenderersOnFullFade = true;
		[SerializeField] private bool _disableCollidersOnFullFade = false;

		// Public getters
		public bool getChildrenRenderers => _getChildrenRenderers;
		public bool disableRenderersOnFullFade => _disableRenderersOnFullFade;
		public bool disableCollidersOnFullFade => disableCollidersOnFullFade;

		#endregion
	}

}