using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class FloatingTextHudSlot : MonoBehaviour
	{

		[SerializeField] private Text _text;

		public void Set(string text)
		{
			_text.text = text;
			RectTransform rt = GetComponent<RectTransform>();
			// Tween position
			rt.localPosition = Vector3.zero;
			LeanTween.moveLocalY(gameObject, 65, 3).setEase(LeanTweenType.easeOutCubic).setOnComplete(DestroySelf);
			// Tween scale
			transform.localScale = Vector3.one * .5f;
			LeanTween.scale(gameObject, Vector3.one, .3f).setEase(LeanTweenType.easeOutBack);
			//// Tween alpha
			//LeanTween.alphaText(rt, 0, 3);
		}

		private void DestroySelf()
		{
			Destroy(gameObject);
		}

	}

}