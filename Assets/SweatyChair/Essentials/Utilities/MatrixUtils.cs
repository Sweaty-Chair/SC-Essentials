using UnityEngine;

namespace SweatyChair
{

	public static class MatrixUtils
	{

		#region Decompose

		/// <summary>
		/// Decomposes a valid TRS matrix into its component position, rotation and scale values.
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="scale"></param>
		/// <returns>Whether or not the provided matrix is a valid TRS Matrix</returns>
		public static bool DecomposeMatrix(this Matrix4x4 matrix, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			// Extract new local position
			position = matrix.GetColumn(3);

			// Extract new local rotation
			rotation = Quaternion.LookRotation(
				matrix.GetColumn(2),
				matrix.GetColumn(1)
			);

			// Extract new local scale
			scale = new Vector3(
				matrix.GetColumn(0).magnitude,
				matrix.GetColumn(1).magnitude,
				matrix.GetColumn(2).magnitude
			);

			return true;
		}

		#endregion

	}

}
