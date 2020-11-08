using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{

	[AddComponentMenu("Layout/Fitted Grid Layout Group", 152)]
	public class FittedGridLayoutGroup : GridLayoutGroup
	{

		#region Variables

		[SerializeField] protected Vector2 m_PrefferedCellSize = new Vector2(100, 100);
		public Vector2 prefferedCellSize { get { return m_PrefferedCellSize; } set { SetProperty(ref m_PrefferedCellSize, value); } }

		[SerializeField] protected bool m_fitToSize = false;
		public bool fitToSize { get { return m_fitToSize; } set { SetProperty(ref m_fitToSize, value); } }

		[SerializeField] protected bool m_useRatio = false;
		public bool useRatio { get { return m_useRatio; } set { SetProperty(ref m_useRatio, value); } }
		[SerializeField] protected Vector2 m_prefferedRatio = new Vector2(1, 1);
		public Vector2 prefferedRatio { get { return m_prefferedRatio; } set { SetProperty(ref m_prefferedRatio, value); } }

		#endregion

		#region Calculate UI Fitting Size

		public override void CalculateLayoutInputHorizontal() {

			base.CalculateLayoutInputHorizontal();

			//End cell size
			Vector2 endCellSize = Vector2.zero;

			if (m_Constraint == Constraint.FixedColumnCount || m_Constraint == Constraint.FixedRowCount) {
				//Get our final size for each item taking into account all padding and spacing
				endCellSize.x = (rectTransform.rect.size.x - (padding.horizontal + (spacing.x * (constraintCount - 1)))) / constraintCount;
				//Then only if our size is less than the preffered cell size, then we update the size
				endCellSize.x = (endCellSize.x < prefferedCellSize.x || fitToSize) ? endCellSize.x : prefferedCellSize.x;

				//Get our final size for each item taking into account all padding and spacing
				endCellSize.y = (rectTransform.rect.size.y - (padding.vertical + (spacing.y * (constraintCount - 1)))) / Mathf.CeilToInt(rectChildren.Count / (float)constraintCount - 0.001f);
				//Then only if our size is less than the preffered cell size, then we update the size
				endCellSize.y = (endCellSize.y < prefferedCellSize.y || fitToSize) ? endCellSize.y : prefferedCellSize.y;
				endCellSize.y = (fitToSize) ? endCellSize.y : prefferedCellSize.y;

				endCellSize.y = (useRatio && prefferedRatio.x != 0 && prefferedRatio.y != 0) ? endCellSize.x * (prefferedRatio.x / prefferedRatio.y) : endCellSize.y;

				//If our layout is supposed to be flexible. We dont worry, and assign our size to the preffered cell size
			} else {
				endCellSize = cellSize;
			}

			//Finally set our cell size
			cellSize = endCellSize;
		}

		#endregion

	}
}
