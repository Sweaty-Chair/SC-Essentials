using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SweatyChair
{

	public static class ColourUtils
	{

		public static Color ColorFromHSL(float h, float s, float l) {
			float r = 0, g = 0, b = 0;
			if (l != 0) {
				if (s == 0)
					r = g = b = l;
				else {
					float temp2;
					if (l < 0.5f)
						temp2 = l * (1.0f + s);
					else
						temp2 = l + s - (l * s);

					float temp1 = 2.0f * l - temp2;

					r = GetColorComponent(temp1, temp2, h + 1.0f / 3.0f);
					g = GetColorComponent(temp1, temp2, h);
					b = GetColorComponent(temp1, temp2, h - 1.0f / 3.0f);
				}
			}

			return new Color(r, g, b);

		}

		private static float GetColorComponent(float temp1, float temp2, float temp3) {
			if (temp3 < 0.0f)
				temp3 += 1.0f;
			else if (temp3 > 1.0f)
				temp3 -= 1.0f;

			if (temp3 < 1.0f / 6.0f)
				return temp1 + (temp2 - temp1) * 6.0f * temp3;
			else if (temp3 < 0.5f)
				return temp2;
			else if (temp3 < 2.0f / 3.0f)
				return temp1 + ((temp2 - temp1) * ((2.0f / 3.0f) - temp3) * 6.0f);
			else
				return temp1;
		}

		#region Darken / Lighten

		/// <summary>
		/// Darkens a colour.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="darkenAmount"></param>
		/// <returns></returns>
		public static Color Darken(this Color color, float darkenAmount) {
			Color newColour = new Color(color.r, color.g, color.b, color.a);

			newColour.r = Mathf.Clamp01(newColour.r - darkenAmount);
			newColour.g = Mathf.Clamp01(newColour.g - darkenAmount);
			newColour.b = Mathf.Clamp01(newColour.b - darkenAmount);

			return newColour;
		}

		/// <summary>
		/// Lightens a colour.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="darkenAmount"></param>
		/// <returns></returns>
		public static Color Lighten(this Color color, float lightenAmount) {
			Color newColour = new Color(color.r, color.g, color.b, color.a);

			newColour.r = Mathf.Clamp01(newColour.r + lightenAmount);
			newColour.g = Mathf.Clamp01(newColour.g + lightenAmount);
			newColour.b = Mathf.Clamp01(newColour.b + lightenAmount);

			return newColour;
		}
		#endregion

		#region Get Closest Colour

		// closed match in RGB space
		public static int GetClosestColourFromRGB(List<Color32> colors, Color32 target) {
			var colorDiffs = colors.Select(n => ColorDiff(n, target)).Min(n => n);
			return colors.FindIndex(n => ColorDiff(n, target) == colorDiffs);
		}

		// weighed distance using hue, saturation and brightness
		public static int GetClosestWeightedColour(List<Color32> colors, Color32 target) {
			float targetHue, targetSaturation, targetValue;
			Color.RGBToHSV(target, out targetHue, out targetSaturation, out targetValue);

			float hue1 = targetHue;
			var num1 = ColorNum(target);
			var diffs = colors.Select(n => Mathf.Abs(ColorNum(n) - num1) + GetHueDistance(n.GetHue(), hue1));
			var diffMin = diffs.Min(x => x);
			return diffs.ToList().FindIndex(n => n == diffMin);
		}

		#endregion

		#region Get HSV Single

		public static float GetHue(this Color color) {
			float hue, saturation, value;
			Color.RGBToHSV(color, out hue, out saturation, out value);
			return hue;
		}
		public static float GetHue(this Color32 color) {
			float hue, saturation, value;
			Color.RGBToHSV(color, out hue, out saturation, out value);
			return hue;
		}

		public static float GetSaturation(this Color color) {
			float hue, saturation, value;
			Color.RGBToHSV(color, out hue, out saturation, out value);
			return saturation;
		}
		public static float GetSaturation(this Color32 color) {
			float hue, saturation, value;
			Color.RGBToHSV(color, out hue, out saturation, out value);
			return saturation;
		}

		public static float GetValue(this Color color) {
			float hue, saturation, value;
			Color.RGBToHSV(color, out hue, out saturation, out value);
			return value;
		}
		public static float GetValue(this Color32 color) {
			float hue, saturation, value;
			Color.RGBToHSV(color, out hue, out saturation, out value);
			return value;
		}

		#endregion

		#region Is Colour Equal

		public static bool IsEqualTo(this Color me, Color other) {
			return Mathf.Approximately(me.r, other.r) && Mathf.Approximately(me.g, other.g) && Mathf.Approximately(me.b, other.b) && Mathf.Approximately(me.a, other.a);
		}

		public static bool IsEqualTo(this Color32 me, Color32 other) {
			return me.r == other.r && me.g == other.g && me.b == other.b && me.a == other.a;
		}

		#endregion

		#region Utility
		// distance between two hues:
		private static float GetHueDistance(float hue1, float hue2) {
			float d = Mathf.Abs(hue1 - hue2); return d > 0.5 ? 1 - d : d;
		}

		//Weighed only by saturation and brightness
		private static float ColorNum(Color c) {
			float cHue, cSat, cVal;
			Color.RGBToHSV(c, out cHue, out cSat, out cVal);

			return cSat * 100 +
						cVal * 100;
		}

		//Get Colour Distance in RBG Space
		private static int ColorDiff(Color32 c1, Color32 c2) {
			return (int)Mathf.Sqrt((c1.r - c2.r) * (c1.r - c2.r)
								   + (c1.g - c2.g) * (c1.g - c2.g)
								   + (c1.b - c2.b) * (c1.b - c2.b));
		}

		#endregion

	}

}
