using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SweatyChair
{

	/// <summary>
	/// Simply takes a photo of a prefab and returns the texture or the sprite. Nothing more than that. Super simple.
	/// Used if you just want a simple way to get a preview image of a prefab.
	/// </summary>
	public class PrefabToImage : IDisposable
	{

		#region Const

		public static readonly Vector3 PLACEMENT_OFFSET = Vector3.one * 1000;

		#endregion

		#region Static

		#region Static Take Photo Methods

		#region Take Photo With Alpha (Direct) - Simple

		public static Texture2D GetTextureWithAlphaInstant(GameObject prefab, int width, int height, bool isOrthographic, Vector3 cameraRotation)
		{
			Texture2D outImage;

			using (PrefabToImage prefabToImage = new PrefabToImage(prefab)) {
				prefabToImage.SetTransparent();

				prefabToImage.SetOrthographic(isOrthographic);
				prefabToImage.AutoPositionCamera(cameraRotation);

				outImage = prefabToImage.ToTexture(width, height);
			}

			return outImage;

		}
		public static Sprite GetSpriteWithAlphaInstant(GameObject prefab, int width, int height, bool isOrthographic, Vector3 cameraRotation)
		{
			Sprite outSprite;

			using (PrefabToImage prefabToImage = new PrefabToImage(prefab)) {
				prefabToImage.SetTransparent();

				prefabToImage.SetOrthographic(isOrthographic);
				prefabToImage.AutoPositionCamera(cameraRotation);

				outSprite = prefabToImage.ToSprite(width, height);
			}

			return outSprite;
		}

		#endregion

		#endregion

		#region Create / Destroy Sprite

		public static Sprite CreateSprite(Texture2D texture)
		{
			// Our PPU will always be one unit based off of our shortest size
			int ppu = Mathf.Min(texture.width, texture.height);

			Sprite outSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, ppu);
			outSprite.name = texture.name;

			return outSprite;
		}

		public static void DestroySprite(Sprite sprite)
		{
			if (sprite != null) {
				Object.Destroy(sprite.texture);
				Object.Destroy(sprite);
			}
		}

		#endregion

		#endregion

		#region Variables

		// Environment Settings
		private static bool _hasSettings => _settings != null;
		private static PrefabToImageSettings _settings => PrefabToImageSettings.current;

		public bool isNull => _holder == null;

		// Components to track
		private GameObject _holder;
		private GameObject _cameraRig;
		private GameObject _lightingRig;

		private Camera _mainRenderCamera;
		private GameObject _prefabInstance;

		#endregion

		#region Constructor

		public PrefabToImage(GameObject prefab, bool useTemplateCamera = false, bool useTemplateLighting = false)
			: this(prefab, (useTemplateCamera) ? _settings.prefabCameraRig : null, (useTemplateLighting) ? _settings.prefabLightingRig : null) { }
		public PrefabToImage(GameObject prefab, GameObject cameraRig, GameObject lightingRig)
		{
			// Then we begin to generate our Scene References, but disabled.
			//The annoying thing about this is that it could possibly leak into the scene if the person using it does not know how to use this correctly. But it ends up making the structure 100x simpler. So we deal with for now
			CreateSceneReferences(prefab, cameraRig, lightingRig);

			// Start a time manager invoke to log an error if we still exist after a second. Which usually would mean we are leaking data into the scene cause we weren't explicitly destroyed
			// This just makes it easier to debug and for a designer to notice
			TimeManager.Invoke(() => CheckForLeaks(this), 1);
		}

		private static void CheckForLeaks(PrefabToImage instance)
		{
			if (instance != null) {
				if (!instance.isNull) {
					Debug.LogError("PrefabToImage: Prefab to image is not null and is more than 1 second old. This usually means we have leaked into the scene. Try to narrow down the cause of this", instance._holder);
				}
			}
		}

		#endregion

		#region Scene References

		#region Initialize

		private void CreateSceneReferences(GameObject prefab, GameObject cameraRig, GameObject lightingRig)
		{
			// Create a new Holder if one does not already exist
			InitializeHolderObject();

			// Initialize our Prefab
			InitializePrefab(prefab);

			// Create and initialize our camera
			InitializeCameraRig(cameraRig);
			InitializeLightingRig(lightingRig);
		}

		private void InitializeHolderObject()
		{
			// Destroy any existing holder before we create a new one
			if (_holder != null)
				Object.Destroy(_holder);

			// Then Generate a new one
			_holder = new GameObject("PrefabToImage - Instance");

			// Init transform
			_holder.transform.ResetTransform();
			_holder.transform.position = PLACEMENT_OFFSET;

			// Then Make sure it dont't get destroyed on scene change
			Object.DontDestroyOnLoad(_holder);

			_holder.SetActive(false);
		}

		private void InitializePrefab(GameObject prefab)
		{
			// Disable our prefab to avoid issues where we have a bunch of, DoXOnEnables messing up our transform
			bool cachePrefabEnable = prefab.activeSelf;
			prefab.SetActive(false);

			// Now spawn our Prefab
			_prefabInstance = InstantiatePreservingMaterials(prefab);

			// Revert our enable state
			prefab.SetActive(cachePrefabEnable);

			// Set our Transform
			_prefabInstance.transform.SetParent(_holder.transform);
			_prefabInstance.transform.localPosition = Vector3.zero;
			_prefabInstance.transform.localRotation = Quaternion.identity;

			// Force all of our Monobehaviours to be off
			TurnOffAllMono(_prefabInstance);

			// Force all trail renderer since it cause problem on rendering
			TurnOffTrailRenderer(_prefabInstance);

			_prefabInstance.SetActive(true);
		}

		private void InitializeCameraRig(GameObject presetRig)
		{
			if (presetRig != null) {
				// If we are using a template camera, and our camera rig is not null. Instantiate, and verify its all good
				_cameraRig = Object.Instantiate(presetRig);

				// Then Grab our Camera Instance from our Object
				_mainRenderCamera = _cameraRig.GetComponentInChildren<Camera>();

				// If we can't find a camera on our template. Log and error and destroy our template rig, and load another rig
				if (_mainRenderCamera == null) {
					Debug.LogError($"Prefab To Image: Unable to Initialize our CameraRig, our provided rig '{presetRig.name}' does not have a Camera Component. This is Required - We are generating a new one instead");
					DestroyCameraRig();
				}
			}

			// If our camera rig is null, we either aren't using a template, or we failed to find a needed component, generate one from scratch
			if (_cameraRig == null) {
				// Create our obj
				_cameraRig = new GameObject("Camera Rig");
				_mainRenderCamera = _cameraRig.AddComponent<Camera>();
			}

			// Finally Make sure our rig has been set up
			_cameraRig.transform.SetParent(_holder.transform);
			_cameraRig.transform.ResetTransform();

			// Disable all things which should be disabled
			_mainRenderCamera.enabled = false;
		}

		private void InitializeLightingRig(GameObject presetRig)
		{
			if (presetRig != null) {
				// If we are using a template lighting, and our lighting rig is not null. Instantiate it
				_lightingRig = Object.Instantiate(presetRig);
			}

			// if our lighting rig is null, just create an empty rig
			if (_lightingRig == null)
				_lightingRig = new GameObject("Lighting Rig");


			// Then init our lighting rig positions
			_lightingRig.transform.SetParent(_holder.transform);
			_lightingRig.transform.ResetTransform();
		}

		#endregion

		#region Destroy

		public void Dispose()
		{
			DestroySceneReferences();
		}

		private void DestroySceneReferences()
		{
			DestroyLightingRig();
			DestroyCameraRig();
			DestroyHolderObject();
			DestroyPrefab();
		}

		private void DestroyHolderObject()
		{
			if (_holder != null)
				Object.Destroy(_holder);

			// Null all references
			_holder = null;
		}

		private void DestroyPrefab()
		{
			if (_prefabInstance != null)
				Object.Destroy(_prefabInstance);

			// Null all references
			_prefabInstance = null;
		}

		private void DestroyCameraRig()
		{
			if (_mainRenderCamera != null)
				Object.Destroy(_mainRenderCamera.gameObject);

			if (_cameraRig != null) {
				_cameraRig.SetActive(false);
				Object.Destroy(_cameraRig);
			}

			// Null all references
			_mainRenderCamera = null;
			_cameraRig = null;
		}

		private void DestroyLightingRig()
		{
			if (_lightingRig != null) {
				_lightingRig.SetActive(false);
				Object.Destroy(_lightingRig);
			}

			// Null all references
			_lightingRig = null;
		}

		#endregion

		#endregion

		#region Settings

		#region Rigs

		#region Camera

		public void SetCameraRig(bool useTemplate)
		{
			SetCameraRig((useTemplate) ? _settings.prefabCameraRig : null);
		}
		public void SetCameraRig(GameObject obj)
		{
			// Destroy our existing light rig
			DestroyCameraRig();
			// Re-Initialize our lighting
			InitializeCameraRig(obj);
		}

		#endregion

		#region Lighting

		public void SetLightingRig(bool useTemplate)
		{
			SetLightingRig((useTemplate) ? _settings.prefabLightingRig : null);
		}
		public void SetLightingRig(GameObject obj)
		{
			// Destroy our existing light rig
			DestroyLightingRig();
			// Re-Initialize our lighting
			InitializeLightingRig(obj);
		}

		#endregion

		#endregion

		#region Background Colour

		public void SetTransparent()
		{
			SetBackgroundColour(Color.clear);
		}

		public void SetBackgroundColour(Color color)
		{
			_mainRenderCamera.clearFlags = CameraClearFlags.Color;
			_mainRenderCamera.backgroundColor = color;
		}

		public void SetBackgroundSkybox()
		{
			_mainRenderCamera.clearFlags = CameraClearFlags.Skybox;
		}

		#endregion

		#region Position Camera

		public void AutoPositionCamera(Vector3 rotation, float minDistance = float.MinValue, float maxDistance = float.MaxValue)
		{
			// Get our Direction Vectors
			Vector3 cameraForward = Quaternion.Euler(rotation) * Vector3.back;

			// Get our Target Distance
			Bounds instanceBounds = BoundsUtils.GetBounds(_prefabInstance);
			float targetDistance = CameraUtils.GetDistanceOfCameraFromObject(_mainRenderCamera, instanceBounds, cameraForward, 0);

			// Make sure to clamp our distance between our min and max
			if (minDistance >= 0)
				targetDistance = Mathf.Max(minDistance, targetDistance);

			if (maxDistance >= 0)
				targetDistance = Mathf.Min(maxDistance, targetDistance);

			// Calculate our final camera position
			Vector3 finalPosition = instanceBounds.center + (-cameraForward * targetDistance);

			// Then finally assign our transform back to our camera
			_mainRenderCamera.transform.position = finalPosition;
			_mainRenderCamera.transform.forward = cameraForward;


			// Then if we are orthographic, set our render size
			float orthoSize = CameraUtils.GetOrthographicSizeForObject(_prefabInstance);
			if (minDistance >= 0)
				orthoSize = Mathf.Max(minDistance, orthoSize);
			if (maxDistance >= 0)
				orthoSize = Mathf.Min(maxDistance, orthoSize);
			_mainRenderCamera.orthographicSize = orthoSize;
		}

		#endregion

		#region Generic Camera Settings

		public void SetOrthographic(bool isOrthographic)
		{
			_mainRenderCamera.orthographic = isOrthographic;
		}

		public void SetFieldOfView(float fov)
		{
			if (_mainRenderCamera.orthographic)
				_mainRenderCamera.orthographicSize = fov;
			else
				_mainRenderCamera.fieldOfView = fov;
		}

		public void SetCameraView(bool isOrthographic, float fov)
		{
			SetOrthographic(isOrthographic);
			SetFieldOfView(fov);
		}

		public void TogglePostProcessing(bool usePostProcessing)
		{
#if UNITY_2019_3_OR_NEWER && UNITY_URP

			if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null && UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset) {
				// Initialize our Custom camera data
				UnityEngine.Rendering.Universal.UniversalAdditionalCameraData customCameraData = _mainRenderCamera.GetOrAddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();

				// Toggle HDR on by default
				_mainRenderCamera.allowHDR = true;

				// Anti-Aliasing
				customCameraData.antialiasing = UnityEngine.Rendering.Universal.AntialiasingMode.FastApproximateAntialiasing;
				customCameraData.antialiasingQuality = UnityEngine.Rendering.Universal.AntialiasingQuality.High;

				// Only allow these settings when our background Color does not contain any alpha, due to below
				// Post Processing - Unable to do at the moment due to: https://issuetracker.unity3d.com/issues/urp-backgrounds-transparency-is-lost-on-render-texture-when-post-processing-is-enabled-on-the-camera
				bool isTransparentImage = _mainRenderCamera.clearFlags == CameraClearFlags.Color && _mainRenderCamera.backgroundColor.a != 1;
				bool canEnablePP = usePostProcessing && !isTransparentImage;

				// Post Processing
				customCameraData.renderPostProcessing = canEnablePP;
				if (usePostProcessing) {
					customCameraData.stopNaN = false;
					customCameraData.dithering = false;
					customCameraData.renderShadows = true;
				}
			}
#endif
		}

		public void FitClippingPlaneToBounds()
		{
			Bounds instanceBounds = BoundsUtils.GetBounds(_prefabInstance);

			// Then Fit our clipping planes to our bounds
			float extentRadius = instanceBounds.extents.magnitude;

			// Set our Front Clipping plane to what we need
			float currentDistance = Vector3.Distance(instanceBounds.center, _mainRenderCamera.transform.position);

			// Finally set our Clipping Planes
			_mainRenderCamera.nearClipPlane = Mathf.Max(0.01f, currentDistance - extentRadius);
			_mainRenderCamera.farClipPlane = currentDistance + extentRadius;
		}

		#endregion

		#region General Settings

		public void SetToLayer()
		{
			// Go through and Update our layers to use the layers we define
			if (_hasSettings)
				SetToLayer(_settings.cameraLayerMask);
		}
		public void SetToLayer(Layer layer)
		{
			_holder.SetLayerRecursively(layer);

			// Force our camera to only see this layer
			_mainRenderCamera.cullingMask = layer.mask;

#if UNITY_2019_1_OR_NEWER
			// Get all lights in our lighting rig to exclusively render to our layer
			Light[] allLights = _lightingRig.GetComponents<Light>();
			for (int i = 0; i < allLights.Length; i++)
				allLights[i].renderingLayerMask = layer.mask;
#endif
		}

		#endregion

		#region Object Settings

		public void ResetObjectTransform()
		{
			_prefabInstance.transform.ResetTransform();
		}

		#endregion

		#endregion

		#region Take Photo

		public Sprite ToSprite(int width, int height)
		{
			return CreateSprite(ToTexture(width, height));
		}
		public Texture2D ToTexture(int width, int height)
		{
			// Toggle our Object on
			_holder.SetActive(true);

			// Generate our Screenshot Texture
			Texture2D screenShotTexture = _mainRenderCamera.GetScreenshot(width, height, GetSupportedRenderTextureFormat());
			screenShotTexture.name = "[PrefabToImage] - Prefab: " + _prefabInstance.name;

			// Toggle it back off
			_holder.SetActive(false);

			return screenShotTexture;
		}

		#region Helpers

		private RenderTextureFormat GetSupportedRenderTextureFormat()
		{
#if !UNITY_EDITOR
			// Mobile seems not supporting formats other than default, please check again later @Chris
#if UNITY_ANDROID
			return RenderTextureFormat.Default;
#elif UNITY_IOS
			return RenderTextureFormat.ARGBHalf;
#endif
#endif
			if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))           // Attempt to use ARGBHalf cause we know this works for HDR on editor
				return RenderTextureFormat.ARGBHalf;
			else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR))    // If ARGBHalf not work, try to use device default HDR
				return RenderTextureFormat.DefaultHDR;
			else
				return RenderTextureFormat.Default;                                             // Otherwise if all else fails, use out default format
		}

		#endregion

		#endregion

		#region Utility

		/// <summary>
		/// Instantiates a copy of a gameobject, preserving Material Property Blocks which may also have been applied
		/// </summary>
		/// <param name=""></param>
		/// <param name=""></param>
		private GameObject InstantiatePreservingMaterials(GameObject original)
		{
			GameObject copy = Object.Instantiate(original);

			// Generate a list of all of our renderers for both our original and our copy
			Renderer[] origRenderers = original.GetComponentsInChildren<Renderer>(true);
			Renderer[] copyRenderers = copy.GetComponentsInChildren<Renderer>(true);

			// Then We have to sort both renderer lists so we correctly assign to each object - Not Particularly optimized, If it's too much of a problem, we can try to find a more optimized version
			origRenderers = origRenderers.OrderBy(renderer => renderer.transform.GetDepth()).ThenBy(renderer => renderer.transform.GetSiblingIndex()).ToArray();
			copyRenderers = copyRenderers.OrderBy(renderer => renderer.transform.GetDepth()).ThenBy(renderer => renderer.transform.GetSiblingIndex()).ToArray();

			// Then we loop through our renderers, and assign from our original to our copy
			MaterialPropertyBlock cachedPropBlock = new MaterialPropertyBlock();

			for (int i = 0; i < origRenderers.Length; i++) {
				if (origRenderers[i].HasPropertyBlock()) {
					origRenderers[i].GetPropertyBlock(cachedPropBlock);
					copyRenderers[i].SetPropertyBlock(cachedPropBlock);
				}
			}

			return copy;
		}

		private void TurnOffAllMono(GameObject prefabInstance)
		{
			// In cases where we have scale on enables and other stuff, we want to force our object to have none of them running when we take the photo by turning off all scripts that we can
			MonoBehaviour[] allBehaviours = prefabInstance.GetComponentsInChildren<MonoBehaviour>(true);
			for (int i = 0; i < allBehaviours.Length; i++)
				allBehaviours[i].enabled = false;
		}

		private void TurnOffTrailRenderer(GameObject prefabInstance)
		{
			// In cases where we have scale on enables and other stuff, we want to force our object to have none of them running when we take the photo by turning off all scripts that we can
			TrailRenderer[] trailRenderer = prefabInstance.GetComponentsInChildren<TrailRenderer>(true);
			for (int i = 0; i < trailRenderer.Length; i++)
				trailRenderer[i].enabled = false;
		}

		#endregion

	}

}
