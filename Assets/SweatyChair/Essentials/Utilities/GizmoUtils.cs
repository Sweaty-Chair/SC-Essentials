using UnityEngine;

namespace SweatyChair
{

	public static class GizmoUtils
	{

		#region DrawRect

		public static void Draw2DRect(Vector2 bottomLeft, Vector2 topRight)
		{
			Vector2 bottomRight = new Vector2(bottomLeft.x, topRight.y);
			Vector2 topLeft = new Vector2(topRight.x, bottomLeft.y);
			// Draw our rect clockwise
			Gizmos.DrawLine(bottomLeft, bottomRight);
			Gizmos.DrawLine(bottomRight, topRight);
			Gizmos.DrawLine(topRight, topLeft);
			Gizmos.DrawLine(topLeft, bottomLeft);
		}

		public static void Draw2DRect(Vector2 bottomLeft, Vector2 topRight, Color gizmoColour)
		{
			// Store our previous colour
			Color tempColour = Gizmos.color;
			Gizmos.color = gizmoColour;

			// Draw our rect
			Draw2DRect(bottomLeft, topRight);

			// Reset our colour
			Gizmos.color = tempColour;
		}

		public static void Draw2DRect(Vector3 position, Vector2 size)
		{
			Vector3 bottomRight = position - (Vector3)size / 2;
			Vector3 topLeft = position + (Vector3)size / 2;

			Vector3 bottomLeft = new Vector3(topLeft.x, bottomRight.y, position.z); 
			Vector3 topRight = new Vector3(bottomRight.x, topLeft.y, position.z);

			// Draw our rect clockwise
			Gizmos.DrawLine(bottomLeft, bottomRight);
			Gizmos.DrawLine(bottomRight, topRight);
			Gizmos.DrawLine(topRight, topLeft);
			Gizmos.DrawLine(topLeft, bottomLeft);
		}
		public static void Draw2DRect(Vector3 position, Vector2 size, Color gizmoColour)
		{
			// Store our previous colour
			Color tempColour = Gizmos.color;
			Gizmos.color = gizmoColour;

			// Draw our rect
			Draw2DRect(position, size);

			// Reset our colour
			Gizmos.color = tempColour;
		}

		#endregion

		#region Draw Box

		public static void DrawWorldSpaceBox(Vector3 minPosition, Vector3 maxPosition, Color gizmoColour)
		{
			// Store our previous colour
			Color tempColour = Gizmos.color;
			Gizmos.color = gizmoColour;

			// Draw our rect
			DrawWorldSpaceBox(minPosition, maxPosition);

			// Reset our colour
			Gizmos.color = tempColour;
		}

		public static void DrawWorldSpaceBox(Vector3 minPosition, Vector3 maxPosition)
		{
			// Calculate all of our corners
			Vector3 leftBottomBack = new Vector3(minPosition.x, minPosition.y, minPosition.z);
			Vector3 leftBottomFront = new Vector3(minPosition.x, minPosition.y, maxPosition.z);
			Vector3 leftTopFront = new Vector3(minPosition.x, maxPosition.y, maxPosition.z);
			Vector3 leftTopBack = new Vector3(minPosition.x, maxPosition.y, minPosition.z);

			Vector3 rightBottomFront = new Vector3(maxPosition.x, minPosition.y, maxPosition.z);
			Vector3 rightBottomBack = new Vector3(maxPosition.x, minPosition.y, minPosition.z);
			Vector3 rightTopBack = new Vector3(maxPosition.x, maxPosition.y, minPosition.z);
			Vector3 rightTopFront = new Vector3(maxPosition.x, maxPosition.y, maxPosition.z);


			// Draw our Bounds manually
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(leftBottomBack, leftBottomFront);
			Gizmos.DrawLine(leftBottomFront, leftTopFront);
			Gizmos.DrawLine(leftTopFront, leftTopBack);
			Gizmos.DrawLine(leftTopBack, leftBottomBack);

			Gizmos.DrawLine(rightBottomBack, rightBottomFront);
			Gizmos.DrawLine(rightBottomFront, rightTopFront);
			Gizmos.DrawLine(rightTopFront, rightTopBack);
			Gizmos.DrawLine(rightTopBack, rightBottomBack);

			Gizmos.DrawLine(rightBottomBack, leftBottomBack);
			Gizmos.DrawLine(rightBottomFront, leftBottomFront);
			Gizmos.DrawLine(rightTopFront, leftTopFront);
			Gizmos.DrawLine(rightTopBack, leftTopBack);
		}

		#endregion

		#region Draw / Debug Plane

		public static void DebugPlane(Plane plane, Color color, float size, float normalLength)
		{
			Vector3 planeCenterAroundOrigin = plane.normal * plane.distance;
			// Draw our Plane normal
			Debug.DrawLine(planeCenterAroundOrigin, planeCenterAroundOrigin + (plane.normal * normalLength), color);

			Vector3 normalPerp;
			if (plane.normal != Vector3.forward)
				normalPerp = Vector3.Cross(plane.normal, Vector3.forward).normalized * size;
			else
				normalPerp = Vector3.Cross(plane.normal, Vector3.up).normalized * size;

			var corner0 = planeCenterAroundOrigin + normalPerp;
			var corner2 = planeCenterAroundOrigin - normalPerp;
			normalPerp = Quaternion.AngleAxis(90f, plane.normal) * normalPerp;
			var corner1 = planeCenterAroundOrigin + normalPerp;
			var corner3 = planeCenterAroundOrigin - normalPerp;

			Debug.DrawLine(corner0, corner2, color);
			Debug.DrawLine(corner1, corner3, color);
			Debug.DrawLine(corner0, corner1, color);
			Debug.DrawLine(corner1, corner2, color);
			Debug.DrawLine(corner2, corner3, color);
			Debug.DrawLine(corner3, corner0, color);
			Debug.DrawLine(planeCenterAroundOrigin, planeCenterAroundOrigin + (plane.normal * normalLength), color);
		}

		#endregion

	}
}
