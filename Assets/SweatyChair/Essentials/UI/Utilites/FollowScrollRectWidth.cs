using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SweatyChair.UI
{

	/*****
	 * A simple script that make sure a LayoutElement has the same width as the ScrollRect
	 *****/
	[RequireComponent(typeof(LayoutElement))]
	public class FollowScrollRectWidth : UIBehaviour
	{

		private RectTransform _rectTransform, _scrollRectRectTransform;

		private void Init()
		{
			if (_rectTransform == null) {
				_rectTransform = GetComponent<RectTransform>();
			}
			if (_scrollRectRectTransform == null) {
				ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
				if (scrollRect != null)
					_scrollRectRectTransform = scrollRect.GetComponent<RectTransform>();
			}
		}

		protected override void OnEnable()
		{
			UpdateWidth();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			UpdateWidth(); // Update every time if parent changed
		}

		private void UpdateWidth()
		{
			Init();
			if (_scrollRectRectTransform != null)
				_rectTransform.sizeDelta = new Vector2(_scrollRectRectTransform.rect.size.x, _rectTransform.sizeDelta.y);
		}

	}

}