using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

	/// <summary>
	/// Author: RV
	/// Capture the camera image with just 1 function. The output image is pre-set in the prefab.
	/// </summary>
	public class ImageCreator : PresetSingleton<ImageCreator>
	{

		[SerializeField] private Camera _camera;
		[SerializeField] private GameObject _backgroundGO;
		[SerializeField] private GameObject _watermarkGO;

		[SerializeField] private int _defaultWidth = 256;
		[SerializeField] private int _defaultHeight = 256;

		private RenderTexture _renderTexture;

		public Camera renderCamera => _camera;

		protected override void Init()
		{
			if (_camera == null)
				_camera = GetComponent<Camera>();
			if (_renderTexture == null) // Skip if already set before Init (very less likely)
				SetRenderTexture(_defaultWidth, _defaultHeight);
			_camera.Render();
		}

		/// <summary>
		/// Creates a render texture on the fly. If you want to create image not equal to the default width and height, you must call this method first.
		/// </summary>
		public static RenderTexture SetRenderTexture(int width, int height)
		{
			if (!instanceExists) // Make sure the instance is created and initialized
				instance.Init();
			// Clear any existing render texture just in case
			if (instance._renderTexture != null && instance._camera != null) {
				instance._camera.targetTexture = null;
				instance._renderTexture.Release();
			}

			instance._renderTexture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32) {
				name = "ImageCreator_RenderTex"
			};
			instance._renderTexture.Create();
			instance._camera.targetTexture = instance._renderTexture;
			ToggleCamera(true);
			
			return instance._renderTexture;
		}

		/// <summary>
		/// Toggles the camera.
		/// </summary>
		public static void ToggleCamera(bool isEnabled)
		{
			if (instance._camera != null) // Avoid false error on application quit
				instance._camera.enabled = isEnabled;
		}

		/// <summary>
		/// Toggles the background.
		/// </summary>
		public static void ToggleBackground(bool isEnabled)
		{
			if (instance._camera != null) // Avoid false error on application quit
				instance._backgroundGO.SetActive(isEnabled);
		}

		/// <summary>
		/// Toggles the watermark.
		/// </summary>
		public static void ToggleWatermark(bool isEnabled)
		{
			if (instance._camera != null)
				instance._watermarkGO.SetActive(isEnabled);
		}

		/// <summary>
		/// Sets the clear flags of the camera.
		/// </summary>
		public static void SetClearFlags(CameraClearFlags clearFlag)
		{
			instance._camera.clearFlags = clearFlag;
			if (clearFlag == CameraClearFlags.Skybox)
				instance._backgroundGO.SetActive(false);
		}

		/// <summary>
		/// Sets the alpha of the background.
		/// </summary>
		public static void SetBackgroundAlpha(float backgroundAlpha)
		{
			if (instance._camera != null) { // Avoid false error on application quit
				Color tempColor = instance._camera.backgroundColor;
				tempColor.a = backgroundAlpha;
				SetBackgroundColour(tempColor);
			}
		}
		public static void SetBackgroundColour(Color backgroundColour)
		{
			if (instance._camera != null) // Avoid false error on application quit
				instance._camera.backgroundColor = backgroundColour;
		}

		/// <summary>
		/// Matchs the camera settings with a given camera.
		/// </summary>
		public static void MatchCameraSettings(Camera camera, bool forceNoAlpha)
		{
			// Set all of our camera values to match our current rendering camera
			instance._camera.clearFlags = camera.clearFlags;
			Color backgroundColor = camera.backgroundColor;
			backgroundColor.a = (forceNoAlpha) ? 1 : backgroundColor.a;
			instance._camera.backgroundColor = backgroundColor;
			instance._camera.fieldOfView = camera.fieldOfView;
			instance._camera.nearClipPlane = camera.nearClipPlane;
			instance._camera.farClipPlane = camera.farClipPlane;
			instance._camera.orthographic = camera.orthographic;
		}

		// TODO:  Objectized all this functions below

		/// <summary>
		/// Creates image from a transform, with current render texture size.
		/// </summary>
		public static void CreateImage(Transform transform, bool watermark, bool instant, UnityAction<Texture2D> completeCallback = null)
		{
			FollowTransform(transform);
			instance._backgroundGO.SetActive(true);
			instance._watermarkGO.SetActive(watermark);
			Texture2D texture = new Texture2D(instance._renderTexture.width, instance._renderTexture.height, TextureFormat.RGB24, false);
			if (instant)
				CreateImageInternal(texture, completeCallback);
			else
				TimeManager.Start(CreateImageCoroutine(texture, completeCallback));
		}

		/// <summary>
		/// Creates image at current position and rotation, with current texture size and input texture, use this if CreateImage is called in Update.
		/// </summary>
		public static void CreateImage(Texture2D texture, bool watermark, bool instant, UnityAction<Texture2D> completeCallback = null)
		{
			instance._backgroundGO.SetActive(true);
			instance._watermarkGO.SetActive(watermark);
			if (instant)
				CreateImageInternal(texture, completeCallback);
			else
				TimeManager.Start(CreateImageCoroutine(texture, completeCallback));
		}

		/// <summary>
		/// Creates image from a position and rotation, with current render texture size.
		/// </summary>
		public static void CreateImage(Vector3 position, Vector3 eulerAngles, bool watermark, bool instant, UnityAction<Texture2D> completeCallback = null)
		{
			Transform myTransform = instance.transform;
			myTransform.position = position;
			myTransform.localEulerAngles = eulerAngles;
			instance._backgroundGO.SetActive(true);
			instance._watermarkGO.SetActive(watermark);
			Texture2D texture = new Texture2D(instance._renderTexture.width, instance._renderTexture.height, TextureFormat.RGB24, false);
			if (instant)
				CreateImageInternal(texture, completeCallback);
			else
				TimeManager.Start(CreateImageCoroutine(texture, completeCallback));
		}

		/// <summary>
		/// Creates image from a transform, with current render texture size.
		/// </summary>
		public static void CreateImage(Transform transform, int width, int height, bool watermark, UnityAction<Texture2D> completeCallback = null)
		{
			SetRenderTexture(width, height);
			CreateImage(transform, watermark, false, completeCallback);
		}

		/// <summary>
		/// Gets the texture with current settings.
		/// </summary>
		public static Texture2D GetTexture()
		{
			Texture2D texture = new Texture2D(instance._renderTexture.width, instance._renderTexture.height, TextureFormat.RGB24, false);
			CreateImageInternal(texture, null);
			return texture;
		}

		/// <summary>
		/// Gets the texture with current settings as the given texture.
		/// </summary>
		public static void GetTexture(Texture2D texture)
		{
			CreateImageInternal(texture, null);
		}

		#region Create Image Coroutines

		private static IEnumerator CreateImageCoroutine(Texture2D texture, UnityAction<Texture2D> completeCallback = null)
		{
			ToggleCamera(true);
			yield return null;
			CreateImageInternal(texture, completeCallback);
		}

		private static void CreateImageInternal(Texture2D texture, UnityAction<Texture2D> completeCallback = null)
		{
			RenderTexture.active = instance._renderTexture;

			// Then Clear our active texture to nothing
			GL.Clear(true, true, Color.clear);
			// Discard the contents of our render texture
			instance._renderTexture.DiscardContents(true, true);

			// Force our camera to render the brand new Screen
			instance._camera.Render();

			Rect renderTextureRect = new Rect(0, 0, instance._renderTexture.width, instance._renderTexture.height);
			texture.ReadPixels(renderTextureRect, 0, 0);
			texture.Apply();

			RenderTexture.active = null;

			completeCallback?.Invoke(texture);
		}

		#endregion

		/// <summary>
		/// Sets the image creator transform following a given transform.
		/// </summary>
		public static void FollowTransform(Transform transform)
		{
			instance.transform.position = transform.position;
			instance.transform.rotation = transform.rotation;
		}

		/// <summary>
		/// Sets the image creator transform offset from a given position.
		/// </summary>
		public static void SetPositionOffset(Vector3 position)
		{
			instance.transform.position += position;
		}


	}

}