using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{
	/// <summary>
	/// Layout element for Labels so that our parent fits to the size of our label with a specified min and max to limit size.
	/// Solves issue where using traditional layout elements to achieve a similar effect would not work, as it would effect other sibling elements
	/// </summary>
	[ExecuteAlways]
	public class LabelLayout : LayoutGroup
	{

		#region Variables

		[Header("Label")]
		[SerializeField] private RectTransform _labelTransform;

		[Header("Label Settings")]
		[SerializeField] private Vector2 _minSize;
		[SerializeField] private Vector2 _maxSize = new Vector2(100, 40);

		#endregion

		#region Unity Lifecycle

#if UNITY_EDITOR

		protected override void OnValidate()
		{
			base.OnValidate();

			// Validate our min and our max size so max can never be less than min
			_maxSize.x = Mathf.Max(_minSize.x, _maxSize.x);
			_maxSize.y = Mathf.Max(_minSize.y, _maxSize.y);
		}

		protected override void Reset()
		{
			base.Reset();

			// Set our max size to not be 0,0 cause thats weird
			_maxSize = new Vector2(100, 40);
		}

		protected void OnDrawGizmosSelected()
		{
			if (UnityEditor.Selection.activeGameObject != gameObject)
				return;

			if (_labelTransform != null) {
				// Draw the 2D rect of our Gizmo UI to show off max size and Min Size
				Vector3[] cornerArray = new Vector3[4];
				_labelTransform.GetWorldCorners(cornerArray);
				Vector3 minWorldPosition = cornerArray[0];

				Vector2 minSize = Vector2.Max(_minSize, Vector2.zero);
				Vector2 maxSize = Vector2.Max(_maxSize, Vector2.zero);

				Vector3 labelMinSizeWorldPosition = cornerArray[0] + (Vector3)(minSize);
				Vector3 labelMaxSizeWorldPosition = cornerArray[0] + (Vector3)(maxSize);

				// Draw our box
				GizmoUtils.Draw2DRect((Vector2)minWorldPosition, labelMinSizeWorldPosition, Color.green);
				GizmoUtils.Draw2DRect((Vector2)minWorldPosition, labelMaxSizeWorldPosition, Color.red);
			}
		}

#endif

		#endregion

		#region Calculate

		/// <summary>
		/// Calculate the layout element properties for this layout element along the given axis.
		/// </summary>
		/// <param name="axis">The axis to calculate for. 0 is horizontal and 1 is vertical.</param>
		/// <param name="isVertical">Is this group a vertical group?</param>
		protected void CalcAlongAxis(int axis)
		{
			// Get our padding for our specified axis
			float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);

			// Get our default data
			float totalMin = combinedPadding;
			float totalPreferred = combinedPadding;
			float totalFlexible = 0;

			// If we have our label, get our min and preferred label size and add it to our total size
			if (_labelTransform != null) {
				GetChildSizes(_labelTransform, axis, out float min, out float preferred, out float flexible);

				// If we are calculating our vertical axis
				if (axis == 1) {
					totalMin = Mathf.Max(min + combinedPadding, totalMin);
					totalPreferred = Mathf.Max(preferred + combinedPadding, totalPreferred);
					totalFlexible = Mathf.Max(flexible, totalFlexible);
				} else {
					totalMin += min;
					totalPreferred += preferred;
					totalFlexible += flexible;
				}
			}

			// Limit our min and preferred size to within our min and max bounds
			float minAxisSize = (axis == 0) ? _minSize.x : _minSize.y;
			float maxAxisSize = (axis == 0) ? _maxSize.x : _maxSize.y;
			minAxisSize += combinedPadding;
			maxAxisSize += combinedPadding;

			totalMin = Mathf.Clamp(totalMin, minAxisSize, maxAxisSize);
			totalPreferred = Mathf.Clamp(totalPreferred, totalMin, maxAxisSize);

			// Then set our rect size to our correct size
			SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, axis);
		}

		/// <summary>
		/// Gets and returns all of our child sizes we need for calculation
		/// </summary>
		/// <param name="child"></param>
		/// <param name="axis"></param>
		/// <param name="min"></param>
		/// <param name="preferred"></param>
		/// <param name="flexible"></param>
		private void GetChildSizes(RectTransform child, int axis, out float min, out float preferred, out float flexible)
		{
			min = LayoutUtility.GetMinSize(child, axis);
			preferred = LayoutUtility.GetPreferredSize(child, axis);

			// We force our flexible size to at least 1 so it fits into our parent correctly
			flexible = Mathf.Max(LayoutUtility.GetFlexibleSize(child, axis), 1);
		}

		#endregion

		#region Set Along Axis

		/// <summary>
		/// Set the positions and sizes of the child layout elements for the given axis.
		/// </summary>
		/// <param name="axis">The axis to handle. 0 is horizontal and 1 is vertical.</param>
		/// <param name="isVertical">Is this group a vertical group?</param>
		protected void SetChildrenAlongAxis(int axis)
		{
			// Only set our children size if we have one
			if (_labelTransform != null) {

				// Get our size and our sizes
				float size = rectTransform.rect.size[axis];
				float alignmentOnAxis = GetAlignmentOnAxis(axis);

				// If we are on out vertical axis
				if ((axis == 1)) {

					// Set our Child Size along our Vertical Axis
					float innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);
					GetChildSizes(_labelTransform, axis, out float min, out float preferred, out float flexible);

					float requiredSpace = Mathf.Clamp(innerSize, min, flexible > 0 ? size : preferred);
					float startOffset = GetStartOffset(axis, requiredSpace);
					SetChildAlongAxis(_labelTransform, axis, startOffset, requiredSpace);

				} else {

					// Get our combined padding
					float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);

					// Calulate our Min and our Preferred sizes clamped by our min and max
					float minAxisSize = (axis == 0) ? _minSize.x : _minSize.y;
					float maxAxisSize = (axis == 0) ? _maxSize.x : _maxSize.y;
					minAxisSize += combinedPadding;
					maxAxisSize += combinedPadding;

					float totalMin = Mathf.Clamp(GetTotalMinSize(axis), minAxisSize, maxAxisSize);
					float totalPreferred = Mathf.Clamp(GetTotalPreferredSize(axis), totalMin, maxAxisSize - combinedPadding);

					// Calculate our label position within our child
					float pos = (axis == 0 ? padding.left : padding.top);
					if (GetTotalFlexibleSize(axis) == 0 && totalPreferred < size)
						pos = GetStartOffset(axis, totalPreferred - combinedPadding);

					// If our min is not equal to our preferred size, we lerp between the two to get the correct size of our label
					float minMaxLerp = 0;
					if (totalMin != totalPreferred)
						minMaxLerp = Mathf.Clamp01((size - totalMin) / (totalPreferred - totalMin));

					// Finally get our child Sizes
					GetChildSizes(_labelTransform, axis, out float min, out float preferred, out float flexible);

					// Limit our child size
					min = Mathf.Max(min, totalMin) - combinedPadding;
					preferred = Mathf.Min(preferred, totalPreferred);

					// Get and finalize our child size
					float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
					childSize = Mathf.Max(childSize, 0);

					SetChildAlongAxis(_labelTransform, axis, pos, childSize);
				}
			}
		}

		#endregion

		#region ILayout

		/// <summary>
		/// Called by the layout system. Also see ILayoutElement
		/// </summary>
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			CalcAlongAxis(0);
		}

		/// <summary>
		/// Called by the layout system. Also see ILayoutElement
		/// </summary>
		public override void CalculateLayoutInputVertical()
		{
			CalcAlongAxis(1);
		}

		/// <summary>
		/// Called by the layout system. Also see ILayoutElement
		/// </summary>
		public override void SetLayoutHorizontal()
		{
			SetChildrenAlongAxis(0);
		}

		/// <summary>
		/// Called by the layout system. Also see ILayoutElement
		/// </summary>
		public override void SetLayoutVertical()
		{
			SetChildrenAlongAxis(1);
		}

		#endregion

	}

}
