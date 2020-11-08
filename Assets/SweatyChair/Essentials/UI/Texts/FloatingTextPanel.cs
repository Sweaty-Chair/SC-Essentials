using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class FloatingTextPanel : Panel
	{

		[SerializeField] private FloatingTextPosition _position;
		[SerializeField] private Text _text;

		private IEnumerator _waitAndHideCoroutine;

		public override void Init()
		{
			if (_text == null)
				_text = GetComponent<Text>();
			if (_text == null)
			{
				Debug.LogErrorFormat("{0}:UIFloatingText:Awake - _label=null", name);
				Destroy(this);
			}
			FloatingTextManager.shownEvent += Show;
			FloatingTextManager.hidEvent += Hide;
		}

		public override void Reset()
		{
			FloatingTextManager.shownEvent -= Show;
			FloatingTextManager.hidEvent -= Hide;
		}

		void OnDisable()
		{
			_text.text = string.Empty; // Make sure when disable, text is reset
		}

		private void Show(FloatingText floatingText)
		{
			if (floatingText.position == _position)
			{
				if (isShown) // Hide first if already shown, to trigger and OnEnable effect
					Hide();
				Show();
				_text.text = floatingText.text;
				if (floatingText.duration > 0)
				{
					if (!gameObject.activeInHierarchy) // Just to make sure
						return;
					if (_waitAndHideCoroutine != null)
						StopCoroutine(_waitAndHideCoroutine);
					_waitAndHideCoroutine = WaitAndHideCoroutine(floatingText.duration);
					StartCoroutine(_waitAndHideCoroutine);
				}
			}
		}

		private void Hide(FloatingTextPosition position)
		{
			if (position == _position)
				Hide();
		}

		private IEnumerator WaitAndHideCoroutine(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			Hide();
		}

	}

}