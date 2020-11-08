using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair.UI
{

	public class FloatingTextHudPanel : Panel
	{

		[SerializeField] private FloatingTextPosition _position;
		[SerializeField] private GameObject _floatingTextHudSlotPrefab;
		[SerializeField] private float _minInterval = 1.5f;

		private float _lastShownTime;
		private Queue<FloatingText> _floatingTextQueue = new Queue<FloatingText>();
		private IEnumerator _showNextFloatingTextCoroutine;

		public override void Init()
		{
			FloatingTextManager.shownEvent += Show;
			FloatingTextManager.hidEvent += Hide;
		}

		public override void Reset()
		{
			FloatingTextManager.shownEvent -= Show;
			FloatingTextManager.hidEvent -= Hide;
		}

		private void Show(FloatingText floatingText)
		{
			if (floatingText == null) // Just in case
				return;

			if (floatingText.position == _position)
			{
				Show(); // Always show

				if (Time.time < _lastShownTime + _minInterval)
				{ // 1s between each text
					_floatingTextQueue.Enqueue(floatingText);
					StartCoroutine(WaitAndShowNextFloatingText(Time.time - _lastShownTime));
					return;
				}

				GameObject go = Instantiate<GameObject>(_floatingTextHudSlotPrefab, transform);
				FloatingTextHudSlot slot = go.GetComponent<FloatingTextHudSlot>();
				slot.Set(floatingText.text);

				_lastShownTime = Time.time;
			}
		}

		private void Hide(FloatingTextPosition position)
		{
			if (position == _position)
				Hide();
		}

		private IEnumerator WaitAndShowNextFloatingText(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			Show(_floatingTextQueue.Dequeue());
		}

#if UNITY_EDITOR

		[ContextMenu("Show Random Text")]
		private void ShowRandomText()
		{
			Show(new FloatingText { text = RandomUtils.GetRandomName(12) });
		}

#endif

	}

}