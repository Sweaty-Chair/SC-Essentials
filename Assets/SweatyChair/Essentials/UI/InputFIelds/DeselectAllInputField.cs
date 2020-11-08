using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	// This is a hack child script, to cancel the default behaviour that InputField select all text on focus
	public class DeselectAllInputField : InputField, IPointerEnterHandler, IPointerExitHandler
	{

		private bool _focused = false;
		private bool _deactivated = false;

		new public void ActivateInputField()
		{
			_focused = true;
			base.ActivateInputField();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			_deactivated = true;
			DeactivateInputField();
			base.OnDeselect(eventData);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (_deactivated) {
				MoveTextEnd(true);
				_deactivated = false;
			}
			base.OnPointerClick(eventData);
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (_focused) {
				MoveTextEnd(true);
				_focused = false;
			}
		}

	}

}