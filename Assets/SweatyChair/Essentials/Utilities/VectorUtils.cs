using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A function extend class to operate Vector.
	/// </summary>
	public static class VectorUtils
	{

		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;
			return Mathf.Clamp(angle, min, max);
		}

		/// <summary>
		/// Multiplies both vectors by (xMultiplier, yMultiplier, zMultiplier) and returns the distance between those products.
		/// </summary>
		public static float DistanceScale(Vector3 a, Vector3 b, float xMultiplier = 1f, float yMultiplier = 1f, float zMultiplier = 1f)
		{
			return DistanceScale(a, b, new Vector3(xMultiplier, yMultiplier, zMultiplier));
		}

		/// <summary>
		/// Multiplies both vectors by 'multiplier' and returns the distance between those products.
		/// </summary>
		public static float DistanceScale(Vector3 a, Vector3 b, Vector3 multiplier)
		{
			return Vector3.Distance(Vector3.Scale(a, multiplier), Vector3.Scale(b, multiplier));
		}

		/// <summary>
		/// Returns the Mid Point between two vectors.
		/// </summary>
		public static Vector3 MidPoint(Vector3 a, Vector3 b)
		{
			return Vector3.Lerp(a, b, 0.5f);
		}

		/// <summary>
		/// Clamps an angle used in a rotation between a known min and max
		/// </summary>
		/// <param name="minAngle"></param>
		/// <param name="maxAngle"></param>
		/// <param name="angle"></param>
		/// <param name="angleOffset"></param>
		public static float ClampRotation(float angle, float minAngle, float maxAngle, float angleOffset = 0)
		{
			angleOffset += 180;
			angle -= angleOffset;
			angle = WrapAngle(angle);
			angle -= 180;
			angle = Mathf.Clamp(angle, minAngle, maxAngle);
			angle += 180;
			return angle + angleOffset;
		}

		/// <summary>
		/// Wraps an angle to always be between 0 and 360 degrees
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static float WrapAngle(float angle)
		{
			while (angle < 0)
				angle += 360;

			return Mathf.Repeat(angle, 360);
		}

		public static Vector3 Abs(Vector3 vector)
		{
			vector.x = Mathf.Abs(vector.x);
			vector.y = Mathf.Abs(vector.y);
			vector.z = Mathf.Abs(vector.z);

			return vector;
		}

		public static Vector3 Clamp(Vector3 vector, Vector3 min, Vector3 max)
		{
			vector.x = Mathf.Clamp(vector.x, min.x, max.x);
			vector.y = Mathf.Clamp(vector.y, min.y, max.y);
			vector.z = Mathf.Clamp(vector.z, min.z, max.z);

			return vector;
		}
		public static Vector2 Clamp(Vector2 vector, Vector2 min, Vector2 max)
		{
			vector.x = Mathf.Clamp(vector.x, min.x, max.x);
			vector.y = Mathf.Clamp(vector.y, min.y, max.y);

			return vector;
		}

		/// <summary>
		/// Aligns a vector to its nearest world axis
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static Vector3 AlignToNearestWorldAxis(Vector3 v)
		{
			if (Mathf.Abs(v.x) < Mathf.Abs(v.y)) {
				v.x = 0;
				if (Mathf.Abs(v.y) < Mathf.Abs(v.z))
					v.y = 0;
				else
					v.z = 0;
			} else {
				v.y = 0;
				if (Mathf.Abs(v.x) < Mathf.Abs(v.z))
					v.x = 0;
				else
					v.z = 0;
			}
			return v;
		}

		#region Sign

		public static Vector3 Sign(Vector3 v)
		{
			v.x = Mathf.Sign(v.x);
			v.y = Mathf.Sign(v.y);
			v.z = Mathf.Sign(v.z);

			return v;
		}

		#endregion

		#region Snap To Grid

		/// <summary>
		/// Snaps a Vector to specific increments on a grid.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="gridSizeX"></param>
		/// <param name="gridSizeY"></param>
		/// <param name="gridSizeZ"></param>
		/// <param name="offsetX"></param>
		/// <param name="offsetY"></param>
		/// <param name="offsetZ"></param>
		/// <returns></returns>
		public static Vector3 SnapToGrid(Vector3 vector, float gridSizeX, float gridSizeY, float gridSizeZ, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
		{
			float x = Mathf.Round(vector.x / gridSizeX) * gridSizeX;
			float y = Mathf.Round(vector.y / gridSizeY) * gridSizeY;
			float z = Mathf.Round(vector.z / gridSizeZ) * gridSizeZ;

			return new Vector3(x + offsetX, y + offsetY, z + offsetZ);
		}

		/// <summary>
		/// Floors a Vector to specific increments on a grid.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="gridSizeX"></param>
		/// <param name="gridSizeY"></param>
		/// <param name="gridSizeZ"></param>
		/// <param name="offsetX"></param>
		/// <param name="offsetY"></param>
		/// <param name="offsetZ"></param>
		/// <returns></returns>
		public static Vector3 FloorToGrid(Vector3 vector, float gridSizeX, float gridSizeY, float gridSizeZ, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
		{
			float x = Mathf.Floor(vector.x / gridSizeX) * gridSizeX;
			float y = Mathf.Floor(vector.y / gridSizeY) * gridSizeY;
			float z = Mathf.Floor(vector.z / gridSizeZ) * gridSizeZ;

			return new Vector3(x + offsetX, y + offsetY, z + offsetZ);
		}

		/// <summary>
		/// Ceils a Vector to specific increments on a grid.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="gridSizeX"></param>
		/// <param name="gridSizeY"></param>
		/// <param name="gridSizeZ"></param>
		/// <param name="offsetX"></param>
		/// <param name="offsetY"></param>
		/// <param name="offsetZ"></param>
		/// <returns></returns>
		public static Vector3 CeilToGrid(Vector3 vector, float gridSizeX, float gridSizeY, float gridSizeZ, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
		{
			float x = Mathf.Ceil(vector.x / gridSizeX) * gridSizeX;
			float y = Mathf.Ceil(vector.y / gridSizeY) * gridSizeY;
			float z = Mathf.Ceil(vector.z / gridSizeZ) * gridSizeZ;

			return new Vector3(x + offsetX, y + offsetY, z + offsetZ);
		}

		#endregion

		#region WrapBetweenBounds

		/// <summary>
		/// Wraps each component of a vector between a min and a max.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="minPosition"></param>
		/// <param name="maxPosition"></param>
		/// <returns></returns>
		public static Vector3 WrapVector(Vector3 position, Vector3 min, Vector3 max)
		{
			position.x = MathUtils.Wrap(position.x, min.x, max.x);
			position.y = MathUtils.Wrap(position.y, min.y, max.y);
			position.z = MathUtils.Wrap(position.z, min.z, max.z);
			return position;
		}

		#endregion

		#region To Colour 

		/// <summary>
		/// Converts a vector to its colour representation, ignoring alpha
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Color ToColor(this Vector3 vector)
		{
			Vector4 colVec = new Vector4(vector.x, vector.y, vector.z, 1);
			return colVec.ToColour();
		}

		/// <summary>
		/// Converts a vector to its colour representation
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Color ToColour(this Vector4 vector)
		{
			return new Color(vector.x, vector.y, vector.z, vector.w);
		}

		#endregion

		#region Min + Max

		public static Vector2 Min(Vector2 vector1, Vector2 vector2)
		{
			Vector2 resultVector = new Vector2();
			resultVector.x = Mathf.Min(vector1.x, vector2.x);
			resultVector.y = Mathf.Min(vector1.y, vector2.y);

			return resultVector;
		}
		public static Vector3 Min(Vector3 vector1, Vector3 vector2)
		{
			Vector3 resultVector = new Vector3();
			resultVector.x = Mathf.Min(vector1.x, vector2.x);
			resultVector.y = Mathf.Min(vector1.y, vector2.y);
			resultVector.z = Mathf.Min(vector1.z, vector2.z);

			return resultVector;
		}

		public static Vector2 Max(Vector2 vector1, Vector2 vector2)
		{
			Vector2 resultVector = new Vector2();
			resultVector.x = Mathf.Max(vector1.x, vector2.x);
			resultVector.y = Mathf.Max(vector1.y, vector2.y);

			return resultVector;
		}
		public static Vector3 Max(Vector3 vector1, Vector3 vector2)
		{
			Vector3 resultVector = new Vector3();
			resultVector.x = Mathf.Max(vector1.x, vector2.x);
			resultVector.y = Mathf.Max(vector1.y, vector2.y);
			resultVector.z = Mathf.Max(vector1.z, vector2.z);

			return resultVector;
		}

		// Helper function that returns the min and max based of two vectors
		public static void GetMinMax(ref Vector3 pos1, ref Vector3 pos2)
		{
			float minX = Mathf.Min(pos1.x, pos2.x);
			float maxX = Mathf.Max(pos1.x, pos2.x);

			float minY = Mathf.Min(pos1.y, pos2.y);
			float maxY = Mathf.Max(pos1.y, pos2.y);

			float minZ = Mathf.Min(pos1.z, pos2.z);
			float maxZ = Mathf.Max(pos1.z, pos2.z);

			//Bounds MirrorX
			pos1 = new Vector3(minX, minY, minZ);
			pos2 = new Vector3(maxX, maxY, maxZ);
		}

		public static Vector3 VectorToWorldDirection(float x, float y, float z)
		{
			return VectorToWorldDirection(new Vector3(x, y, z));
		}
		public static Vector3 VectorToWorldDirection(Vector3 vector)
		{
			Quaternion cameraRotation = Quaternion.Euler(vector);
			return cameraRotation * Vector3.forward;
		}

		#endregion

		#region As Range

		/// <summary>
		/// Returns whether a specified number is within a min and max determined by a vector2 acting as bounds
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool Contains(this Vector2 vector, float value)
		{
			return value >= vector.x & value <= vector.y;
		}

		#endregion

	}

}