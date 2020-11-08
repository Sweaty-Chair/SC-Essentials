using UnityEngine;  
using UnityEngine.EventSystems;  

namespace SweatyChair
{

	/// <summary>
	/// Use this on Button texts to have some color transition on the text as well without corrupting button's behaviour.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class ButtonToggler : MonoBehaviour, IPointerClickHandler {
		
		[SerializeField] private GameObject _tagetGO;
		[SerializeField] private bool _toogleEnable = true;
		[SerializeField] private int _undoAfterSeconds; // Undo the toogle after seconds, 0 to disable

		public void OnPointerClick(PointerEventData eventData)
		{
			_tagetGO.SetActive(_toogleEnable);
			if (_undoAfterSeconds > 0)
				Invoke("Undo", _undoAfterSeconds);
		}

		private void Undo()
		{
			_tagetGO.SetActive(!_toogleEnable);
		}
		
	}
}