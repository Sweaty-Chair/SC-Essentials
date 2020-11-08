using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair
{

	/// <summary>
	/// Use this on Button texts to have some color transition on the text as well without corrupting button's behaviour.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class ToggleTargetButton : MonoBehaviour
	{

		[SerializeField] private GameObject _tagetGO;
		[SerializeField] private bool _activate = true;
		[SerializeField] private bool _undoOnMouseUp = true; // Undo the toogle after seconds, 0 to disable

		private bool _toggled = false;

		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void Update()
		{
			if (_undoOnMouseUp && _toggled && Input.GetMouseButtonUp(0)) {
				Invoke("Undo", 0.1f); // Delay a bit in case if click on target
				_toggled = false;
			}
		}

		protected virtual void OnClick()
		{
			_tagetGO.SetActive(_activate);
			Invoke("SetToggled", 0.1f);
		}

		protected void Undo()
		{
			_tagetGO.SetActive(!_activate);
		}

		protected void SetToggled()
		{
			_toggled = true;
		}

	}
}