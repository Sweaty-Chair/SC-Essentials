using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A utility that keep track of the variables mapping from screen to world in 2D game, this only works with orthographic camera.
	/// </summary>
	public static class ScreenUtils
	{

		public static int initScreenWidth { get; private set; }

		public static int initScreenHeight { get; private set; }

		public static float ratio { get; private set; }

		public static float minX { get; private set; }

		public static float maxX { get; private set; }

		public static float minY { get; private set; }

		public static float maxY { get; private set; }

		// The screen min x fit all aspects (i.e. minX in 3:4 on portrait, or 16:9 on landscape), e.g. used for bullet spawn edge position so spawn point is same in all devices
		public static float minXFitAll { get; private set; }

		// The screen max x fit all aspects (i.e. maxX in 3:4 on portrait, or 16:9 on landscape), e.g. used for bullet spawn edge position so spawn point is same in all devices
		public static float maxXFitAll { get; private set; }

		// The screen min x all aspects can see (i.e. minX in 16:9 on portrait, or 4:3 on landscape), e.g. used for loot spawn position so all devices can see
		public static float minXSeeAll { get; private set; }

		// The screen max x all aspects can see (i.e. maxX in 16:9 on portrait, or 4:3 on landscape), e.g. used for loot spawn position so all devices can see
		public static float maxXSeeAll { get; private set; }

		public static float width { get; private set; }

		public static float height { get; private set; }

		public static float diagonal { get; private set; }

		public static float halfDiagonal { get; private set; }

		public static Vector2 randomPosition {
			get { return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY)); }
		}

		public static Vector2 randomPositionFitAll {
			get {
				return new Vector2(
					Random.Range(minXFitAll, maxXFitAll),
					Random.Range(minY, maxY)
				);
			}
		}

		public static Vector2 randomPositionSeeAll {
			get {
				return new Vector2(
					Random.Range(minXSeeAll, maxXSeeAll),
					Random.Range(minY, maxY)
				);
			}
		}

		static ScreenUtils()
		{
			initScreenWidth = Screen.width;
			initScreenHeight = Screen.height;

			ratio = 1f * Screen.width / Screen.height;

			Vector3 screenBottomLeftPos = Camera.main.ScreenToWorldPoint(Vector2.zero);
			Vector3 screenTopRightPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

			minX = screenBottomLeftPos.x;
			maxX = screenTopRightPos.x;
			minY = screenBottomLeftPos.y;
			maxY = screenTopRightPos.y;

			minXFitAll = minX / ratio * (ratio > 1 ? (16f / 9) : (3f / 4)); // 16:9 on landscape or 3:4 on portrait
			maxXFitAll = maxX / ratio * (ratio > 1 ? (16f / 9) : (3f / 4)); // 16:9 on landscape or 3:4 on portrait

			minXSeeAll = minX / ratio * (ratio > 1 ? (4f / 3) : (9f / 16)); // 4:3 on landscape or 9:16 on portrait
			maxXSeeAll = maxX / ratio * (ratio > 1 ? (4f / 3) : (9f / 16)); // 4:3 on landscape or 9:16 on portrait

			width = maxX - minX;
			height = maxY - minY;
			diagonal = new Vector2(width, height).magnitude;
			halfDiagonal = diagonal / 2;
		}

		public static void Init()
		{
		}

		// Given a float from [-1,1], map the y value in screen
		public static float GetProportionalY(float prop)
		{
			prop = Mathf.Clamp(prop, -1, 1); // Make sure prop is -1 to 1
			return minY + (1 + prop) * height / 2;
		}

		// Check if a world position is within the screen
		public static bool IsWithinScreen(Vector2 v)
		{
			return v.x >= minX && v.x <= maxX && v.y >= minY && v.y <= maxY;
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Screen/Print Parameters")]
		public static void PrintParameters()
		{
			Debug.Log("ratio=" + ratio);
			Debug.Log("minX=" + minX);
			Debug.Log("maxX=" + maxX);
			Debug.Log("minY=" + minY);
			Debug.Log("maxY=" + maxY);
			Debug.Log("minXFitAll=" + minXFitAll);
			Debug.Log("maxXFitAll=" + maxXFitAll);
		}

#endif

	}

}