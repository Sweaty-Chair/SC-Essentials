using UnityEngine;

namespace SweatyChair {

	/// <summary>
	/// Manager to simplify cursor state setting. Taking into account fullscreen type and other factors.
	/// </summary>
	public class CursorTogger : MonoBehaviour
	{

		[SerializeField] private bool _setCursorLockedOnEnabled = false;
		[SerializeField] private bool _resetOnDisable = true;

		private bool _prevIsCursorLocked;

		private void OnEnable()
		{
			_prevIsCursorLocked = CursorManager.IsCursorLocked();
			CursorManager.LockCursor(_setCursorLockedOnEnabled);
		}

		private void OnDisable()
		{
			if (_resetOnDisable) {
				CursorManager.LockCursor(_prevIsCursorLocked);
			}
		}

	}

}
