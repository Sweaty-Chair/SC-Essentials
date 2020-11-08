using UnityEngine;

namespace SweatyChair
{
	public enum Rarity
	{
		Common,
        Rare,
        Epic,
        Legendary,
        Mythical,
    }

	public static class RarityExtensions
	{

		// Rarity Colours sourced from https://fortnite.gamepedia.com/Rarity

		#region Consts

		public const string COMMON_COLOUR_HEX = "#7b7b7b";
		public const string RARE_COLOUR_HEX = "#3a7913";
		public const string EPIC_COLOUR_HEX = "#1258a2";
		public const string LEGENDARY_COLOUR_HEX = "#bd3ffa";
		public const string MYTHICAL_COLOUR_HEX = "#ff7605";

		#endregion

		#region Static

		public static Color COMMON_COLOUR {
			get {
				ColorUtility.TryParseHtmlString(COMMON_COLOUR_HEX, out Color outColor);
				return outColor;
			}
		}

		public static Color RARE_COLOUR {
			get {
				ColorUtility.TryParseHtmlString(RARE_COLOUR_HEX, out Color outColor);
				return outColor;
			}
		}

		public static Color EPIC_COLOUR {
			get {
				ColorUtility.TryParseHtmlString(EPIC_COLOUR_HEX, out Color outColor);
				return outColor;
			}
		}

		public static Color LEGENDARY_COLOUR {
			get {
				ColorUtility.TryParseHtmlString(LEGENDARY_COLOUR_HEX, out Color outColor);
				return outColor;
			}
		}

		public static Color MYTHICAL_COLOUR {
			get {
				ColorUtility.TryParseHtmlString(MYTHICAL_COLOUR_HEX, out Color outColor);
				return outColor;
			}
		}

		#endregion

		public static Color ToRarityColour(this Rarity rarity)
		{
			switch (rarity) {
			case Rarity.Common:
				return COMMON_COLOUR;

			case Rarity.Rare:
				return RARE_COLOUR;

			case Rarity.Epic:
				return EPIC_COLOUR;

			case Rarity.Legendary:
				return LEGENDARY_COLOUR;

			case Rarity.Mythical:
				return MYTHICAL_COLOUR;

			default:
				return Color.magenta;
			}
		}

	}

}