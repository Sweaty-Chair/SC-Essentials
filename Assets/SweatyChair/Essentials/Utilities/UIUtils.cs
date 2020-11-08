using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SweatyChair
{

	public static class UIUtils
	{

		#region Scroll Rect

		/// <summary>
		/// Transform the bounds of the current rect transform to the space of another transform.
		/// Source: https://gist.github.com/sttz/c406aec3ace821738ecd4fa05833d21d
		/// </summary>
		/// <param name="source">The rect to transform</param>
		/// <param name="target">The target space to transform to</param>
		/// <returns>The transformed bounds</returns>
		public static Bounds TransformBoundsTo(this RectTransform source, Transform target)
		{
			// Based on code in ScrollRect's internal GetBounds and InternalGetBounds methods
			var bounds = new Bounds();
			Vector3[] corners = new Vector3[4];
			if (source != null) {
				source.GetWorldCorners(corners);

				var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
				var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

				var matrix = target.worldToLocalMatrix;
				for (int j = 0; j < 4; j++) {
					Vector3 v = matrix.MultiplyPoint3x4(corners[j]);
					vMin = Vector3.Min(v, vMin);
					vMax = Vector3.Max(v, vMax);
				}

				bounds = new Bounds(vMin, Vector3.zero);
				bounds.Encapsulate(vMax);
			}
			return bounds;
		}

		/// <summary>
		/// Normalize a distance to be used in verticalNormalizedPosition or horizontalNormalizedPosition.
		/// Source: https://gist.github.com/sttz/c406aec3ace821738ecd4fa05833d21d
		/// </summary>
		/// <param name="axis">Scroll axis, 0 = horizontal, 1 = vertical</param>
		/// <param name="distance">The distance in the scroll rect's view's coordiante space</param>
		/// <returns>The normalized scoll distance</returns>
		public static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance)
		{
			// Based on code in ScrollRect's internal SetNormalizedPosition method
			var viewport = scrollRect.viewport;
			var viewRect = viewport ?? scrollRect.GetComponent<RectTransform>();
			var viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);

			var content = scrollRect.content;
			var contentBounds = content != null ? content.TransformBoundsTo(viewRect) : new Bounds();

			var hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];
			return distance / hiddenLength;
		}

		public static void ScrollToTop(this ScrollRect scrollRect, RectTransform.Axis axis = RectTransform.Axis.Vertical)
		{
			if (axis == RectTransform.Axis.Vertical)
				scrollRect.verticalNormalizedPosition = 1;
			else
				scrollRect.horizontalNormalizedPosition = 1;
		}

		/// <summary>
		/// Scroll the target element to the vertical/horizontal center of the scroll rect's viewport.
		/// Assumes the target element is part of the scroll rect's contents.
		/// Source: https://gist.github.com/sttz/c406aec3ace821738ecd4fa05833d21d
		/// Note: Make sure to call this 2+ frames after Awake, so the scroll view and children slots are positioned.
		/// </summary>
		/// <param name="scrollRect">Scroll rect to scroll</param>
		/// <param name="target">Element of the scroll rect's content to center vertically</param>
		public static void ScrollToCenter(this ScrollRect scrollRect, RectTransform target, RectTransform.Axis axis = RectTransform.Axis.Vertical)
		{
			// The scroll rect's view's space is used to calculate scroll position
			var view = scrollRect.viewport ?? scrollRect.GetComponent<RectTransform>();

			// Calcualte the scroll offset in the view's space
			var viewRect = view.rect;
			var elementBounds = target.TransformBoundsTo(view);

			// Normalize and apply the calculated offset
			if (axis == RectTransform.Axis.Vertical) {
				var offset = viewRect.center.y - elementBounds.center.y;
				var scrollPos = scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, offset);
				scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollPos, 0, 1);
			} else {
				var offset = viewRect.center.x - elementBounds.center.x;
				var scrollPos = scrollRect.horizontalNormalizedPosition - scrollRect.NormalizeScrollDistance(0, offset);
				scrollRect.horizontalNormalizedPosition = Mathf.Clamp(scrollPos, 0, 1);
			}
		}

		/// <summary>
		/// Scrolls the target element to the vertical center of the scroll rect's viewport.
		/// Assumes the target element is part of the scroll rect's contents.
		/// </summary>
		/// <param name="scrollRect">Scroll rect to scroll</param>
		/// <param name="target">Element of the scroll rect's content to center vertically</param>
		public static void ScrollToContent(this ScrollRect scrollRect, RectTransform target, RectTransform.Axis axis, float normalizedViewOffset, float normalizedContentOffset)
		{
			Vector2 offset = GetScrollAmountWithOffset(scrollRect, target, new Vector2(normalizedViewOffset, normalizedViewOffset), new Vector2(normalizedContentOffset, normalizedContentOffset));

			if (axis == RectTransform.Axis.Horizontal)
				scrollRect.horizontalNormalizedPosition = offset.x;
			else
				scrollRect.verticalNormalizedPosition = offset.y;
		}

		#region Vector2 ScrollAmnt

		/// <summary>
		/// Scrolls the target element to the vertical center of the scroll rect's viewport.
		/// Assumes the target element is part of the scroll rect's contents.
		/// </summary>
		/// <param name="scrollRect">Scroll rect to scroll</param>
		/// <param name="target">Element of the scroll rect's content to center vertically</param>
		public static float GetScrollAmountWithOffset(this ScrollRect scrollRect, RectTransform target, RectTransform.Axis axis, float normalizedViewOffset, float normalizedContentOffset)
		{
			Vector2 offset = GetScrollAmountWithOffset(scrollRect, target, new Vector2(normalizedViewOffset, normalizedViewOffset), new Vector2(normalizedContentOffset, normalizedContentOffset));
			return (axis == RectTransform.Axis.Horizontal) ? offset.x : offset.y;
		}

		/// <summary>
		/// Scrolls the target element to the vertical center of the scroll rect's viewport.
		/// Assumes the target element is part of the scroll rect's contents.
		/// </summary>
		/// <param name="scrollRect">Scroll rect to scroll</param>
		/// <param name="target">Element of the scroll rect's content to center vertically</param>
		public static Vector2 GetScrollAmountWithOffset(this ScrollRect scrollRect, RectTransform target, Vector2 normalizedViewOffset, Vector2 normalizedContentOffset)
		{
			var view = scrollRect.viewport ?? scrollRect.GetComponent<RectTransform>();

			var elementBounds = target.TransformBoundsTo(view);

			Vector2 viewOffset = new Vector2(Mathf.Lerp(view.rect.min.x, view.rect.max.x, normalizedViewOffset.x), Mathf.Lerp(view.rect.min.y, view.rect.max.y, normalizedViewOffset.y));
			Vector2 contentOffset = new Vector2(Mathf.Lerp(elementBounds.min.x, elementBounds.max.x, normalizedContentOffset.x), Mathf.Lerp(elementBounds.min.y, elementBounds.max.y, normalizedContentOffset.y));

			Vector2 offset = viewOffset - contentOffset;

			var scrollPos = new Vector2(scrollRect.horizontalNormalizedPosition - scrollRect.NormalizeScrollDistance(0, offset.x), scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, offset.y));
			return VectorUtils.Clamp(scrollPos, Vector2.zero, Vector2.one);
		}

		#endregion

		#endregion

		#region Get Interactable State

		private static readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();
		/// <summary>
		/// Returns whether this gameobject in the hierarchy is interactable, determined through parent Canvasgroups
		/// </summary>
		public static bool GetIsObjectInteractable(Transform transform)
		{
			// RAW COPY PASTE FROM https://bitbucket.org/Unity-Technologies/ui/src/2019.1/UnityEngine.UI/UI/Core/Selectable.cs
			// Figure out if parent groups allow interaction If no interaction is alowed... then we need to not do that :)
			var groupAllowInteraction = true;
			Transform t = transform;
			while (t != null) {
				t.GetComponents(m_CanvasGroupCache);
				bool shouldBreak = false;
				for (var i = 0; i < m_CanvasGroupCache.Count; i++) {
					// if the parent group does not allow interaction
					// we need to break
					if (!m_CanvasGroupCache[i].interactable) {
						groupAllowInteraction = false;
						shouldBreak = true;
					}
					// if this is a 'fresh' group, then break
					// as we should not consider parents
					if (m_CanvasGroupCache[i].ignoreParentGroups)
						shouldBreak = true;
				}
				if (shouldBreak)
					break;

				t = t.parent;
			}

			return groupAllowInteraction;
		}

		#endregion

		#region Pointer Over

		/// <summary>
		/// Works like EventSystem.current.IsPointerOverGameObject(), but in UI and detecting touch.
		/// https://answers.unity.com/questions/1115464/ispointerovergameobject-not-working-with-touch-inp.html
		/// </summary>
		public static bool IsPointerOverUIObject()
		{
			Vector2 pointerPosition = Input.mousePosition;
#if UNITY_IOS || UNITY_ANDROID
			if (Input.touchCount > 0)
				pointerPosition = Input.GetTouch(0).position;
#endif
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
				position = pointerPosition
			};
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			return results.Count > 0;
		}

		#endregion

	}

}