using UnityEngine;

namespace SweatyChair
{

	public static class MathUtils
	{

		#region Const

		public const float ROUNDING_EPSILON = 0.0001f;

		#endregion

		#region Wrap

		/// <summary>
		/// Wraps a value between a min and a max, no matter the size of the number
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static float Wrap(float value, float min, float max) {
			//Based off Wrapping function found here https://stackoverflow.com/questions/14415753/wrap-value-into-range-min-max-without-division
			return value - (max - min) * Mathf.Floor(value / (max - min));
		}

		public static int Wrap(int value, int min, int max) {
			return Mathf.RoundToInt(Wrap(value, min, (float)max));
		}

		#endregion

		#region Floor

		/// <summary>
		/// Flooring function, which floors floating point numbers a lil bit nicer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static float ApproxFloor(float value) {
			// Based off this post https://mortoray.com/2016/01/05/the-trouble-with-floor-and-ceil/
			return ApproxFloor(value, ROUNDING_EPSILON);
		}
		public static float ApproxFloor(float value, float threshold) {
			return Mathf.Floor(value + threshold);
		}

		/// <summary>
		/// Flooring function, which ceils floating point numbers a lil bit nicer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static float ApproxCeil(float value) {
			// Based off this post https://mortoray.com/2016/01/05/the-trouble-with-floor-and-ceil/
			return ApproxCeil(value, ROUNDING_EPSILON);
		}
		public static float ApproxCeil(float value, float threshold) {
			return Mathf.Ceil(value - threshold);
		}

		#endregion

		#region Power of Two

		public static int CeilToPow2(float inValue) {
			return (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(inValue) / Mathf.Log(2)));
		}

		#endregion

		#region Double Arithmetic

		/// <summary>
		/// Calculates the linear parameter 't' that produces the interpolant value withing the range [a,b].
		/// </summary>
		/// <param name="a">Start value.</param>
		/// <param name="b">End Value.</param>
		/// <param name="value">Value between start and end.</param>
		/// <returns></returns>
		public static double InverseLerp(double a, double b, double value) {
			return (value - a) / (b - a);
		}

		/// <summary>
		/// Lineraly interpolates between a and b by t.
		/// </summary>
		/// <param name="a">Start Value.</param>
		/// <param name="b">End Value.</param>
		/// <param name="t">The interpolation value between the two values</param>
		/// <returns></returns>
		public static double Lerp(double a, double b, double t) {
			return a + (b - a) * t;
		}

		/// <summary>
		/// Clamps the given value between the given minimum and maximum values. Returns the given value if it is within the min and max range.
		/// </summary>
		/// <param name="value">The value to restrict inside the range defined bu the min and max values.</param>
		/// <param name="min">The minimum value to compare against.</param>
		/// <param name="max">The maximum value to compare against.</param>
		/// <returns></returns>
		public static double Clamp(double value, double min, double max) {
			return System.Math.Min(System.Math.Max(value, min), max);
		}

		#endregion

	}
}