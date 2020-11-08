using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair
{

	public class PrefabToImageOnEnable : MonoBehaviour
	{
		#region Variables

		[Header("Core Settings")]
		public GameObject prefab;

		[Header("Sprite Settings")]
		public int spriteWidth = 128;
		public int spriteHeight = 128;

		[Header("Default Settings")]
		public GameObjectParameter useCustomCamera;
		public GameObjectParameter useCustomLighting;

		[Header("Positioning Settings")]
		public BoolParameter isOrthographic;
		public Vector3Parameter cameraAngle = new Vector3Parameter() { value = new Vector3(-25, -45, 0) };
		public Vector2Parameter previewFraming;

		[Header("Rendering Settings")]
		public BoolParameter usePostProcessing = new BoolParameter() { value = true };
		public ColorParameter backgroundColour = new ColorParameter() { value = Color.clear };

		[Header("Debug Settings")]
		public bool isDebug = false;

		// Cached Data
		private Sprite _iconSprite;

		#endregion

		#region OnEnable / OnDisable

		// Start is called before the first frame update
		void OnEnable()
		{
			// Create our Object
			// Implement the IDisposable through a 'using' statement to correctly call our dispose method on complete
			using (PrefabToImage prefabToImage = CreatePrefabToImage()) {
				SetPrefabToImageSettings(prefabToImage);

				// Then Screenshot our prefab
				_iconSprite = prefabToImage.ToSprite(spriteWidth, spriteHeight);
				SetToRenderer(_iconSprite);

				if (isDebug)
					return;
			}
		}

		private void OnDisable()
		{
			RemoveFromRenderer();

			// Destroy our sprite and our icon
			PrefabToImage.DestroySprite(_iconSprite);
			_iconSprite = null;
		}

		#endregion

		#region Prefab To image Wrapper

		public PrefabToImage CreatePrefabToImage()
		{
			// Create a new Prefab to Image, and setup all of our data
			PrefabToImage prefabToImage = new PrefabToImage(prefab, true, true);

			// Then Toggle our template rig settings if we are using custom rigs
			if (useCustomCamera.overrideState)
				prefabToImage.SetCameraRig(useCustomCamera.value);

			if (useCustomLighting.overrideState)
				prefabToImage.SetLightingRig(useCustomLighting.value);

			return prefabToImage;
		}

		public void SetPrefabToImageSettings(PrefabToImage prefabToImage)
		{
			// Set our data
			if (isOrthographic.overrideState)
				prefabToImage.SetOrthographic(isOrthographic);

			// Position our camera if we are overriding our state
			if (cameraAngle.overrideState) {
				if (!previewFraming.overrideState)
					prefabToImage.AutoPositionCamera(cameraAngle);
				else
					prefabToImage.AutoPositionCamera(cameraAngle, previewFraming.value.x, previewFraming.value.y);
			}

			// Toggle our background colour
			if (backgroundColour.overrideState)
				prefabToImage.SetBackgroundColour(backgroundColour);

			// Toggle post processing
			if (usePostProcessing.overrideState)
				prefabToImage.TogglePostProcessing(usePostProcessing);

			prefabToImage.SetToLayer();
		}

		#endregion

		#region Set Sprite

		private void SetToRenderer(Sprite sprite)
		{
			// Sprite Renderer
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			if (sr != null)
				sr.sprite = _iconSprite;

			// Image
			Image image = GetComponent<Image>();
			if (image != null)
				image.sprite = _iconSprite;

			// RawImage
			RawImage rawImage = GetComponent<RawImage>();
			if (rawImage != null)
				rawImage.texture = _iconSprite.texture;
		}

		private void RemoveFromRenderer()
		{
			// Sprite Renderer
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			if (sr != null)
				sr.sprite = null;

			// Image
			Image image = GetComponent<Image>();
			if (image != null)
				image.sprite = null;

			// RawImage
			RawImage rawImage = GetComponent<RawImage>();
			if (rawImage != null)
				rawImage.texture = null;
		}

		#endregion

	}

}