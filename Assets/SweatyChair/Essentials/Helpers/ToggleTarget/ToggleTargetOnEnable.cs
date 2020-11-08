using System.Collections;
using UnityEngine;

namespace SweatyChair
{

	public class ToggleTargetOnEnable : MonoBehaviour
	{

		[SerializeField] private GameObject _target;
		[SerializeField] private bool _toggle = true;
		[SerializeField] private float _delay = 0;

		private void OnEnable()
		{
			Invoke("ToogleTarget", _delay);
		}

		private void ToogleTarget()
		{
			_target.SetActive(_toggle);
		}

	}

}