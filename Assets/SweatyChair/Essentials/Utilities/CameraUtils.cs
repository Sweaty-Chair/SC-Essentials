using UnityEngine;

namespace SweatyChair
{

	public static class CameraUtils
	{

		#region Get Camera Position From Object

		public static float GetDistanceOfCameraFromObject(Camera camera, GameObject obj, Vector3 direction, float padding)
		{
			Bounds collectiveBounds = BoundsUtils.GetBounds(obj);
			return GetDistanceOfCameraFromObject(camera, collectiveBounds, direction, padding);
		}
		public static float GetDistanceOfCameraFromObject(Camera camera, Bounds bounds, Vector3 direction, float padding)
		{
			// Get the distance from our object our camera has to be to encompass it within our view
			float circleDist = bounds.size.magnitude;
			// Calculate our frustrum height from our bounds diameter distance + Padding
			float frustrumHeight = circleDist + (circleDist * padding);
			var distance = frustrumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

			return distance;
		}

		/// <summary>
		/// Gets the position in world space where a camera would need to be placed to be looking at an object
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="obj"></param>
		/// <param name="direction"></param>
		/// <param name="padding"></param>
		/// <param name="xPerc"></param>
		/// <param name="yPerc"></param>
		/// <returns></returns>
		public static Vector3 GetCameraPositionFromObject(Camera camera, GameObject obj, Vector3 direction, float padding)
		{
			Bounds collectiveBounds = BoundsUtils.GetBounds(obj);
			return GetCameraPositionFromObject(camera, collectiveBounds, direction, padding);
		}
		/// <summary>
		/// Gets the position in world space where a camera would need to be placed to be looking at an object
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="bounds"></param>
		/// <param name="direction"></param>
		/// <param name="padding"></param>
		/// <param name="xPerc"></param>
		/// <param name="yPerc"></param>
		/// <returns></returns>
		public static Vector3 GetCameraPositionFromObject(Camera camera, Bounds bounds, Vector3 direction, float padding)
		{
			// Get the distance from our object our camera has to be to encompass it within our view
			float distance = GetDistanceOfCameraFromObject(camera, bounds, direction, padding);

			// Then Calculate our final position
			return bounds.center + (-direction.normalized * distance);   //Move the camera back the specified number of units
		}

		#endregion

		#region Get Orthographic Size For Fit

		public static float GetOrthographicSizeForObject(GameObject obj, float padding = 0)
		{
			Bounds collectiveBounds = BoundsUtils.GetBounds(obj);
			return GetOrthographicSizeForObject(collectiveBounds, padding);
		}
		public static float GetOrthographicSizeForObject(Bounds bounds, float padding = 0)
		{
			// Get the circle distance size for our object
			float circleDist = bounds.size.magnitude;
			// Calculate our frustrum height from our bounds diameter distance + Padding
			float frustrumDiameter = circleDist + (circleDist * padding);

			// Then return half our size
			return frustrumDiameter / 2;
		}

		#endregion

		#region Copy Values Between Cameras

		/// <summary>
		/// Copies over core camera values, which are most of the ones set in the inspector.
		/// </summary>
		/// <param name="donorCam"></param>
		/// <param name="recieveCam"></param>
		public static void CopyCoreCameraValues(Camera donorCam, Camera recieveCam)
		{
			recieveCam.clearFlags = donorCam.clearFlags;
			recieveCam.backgroundColor = donorCam.backgroundColor;
			recieveCam.cullingMask = donorCam.cullingMask;
			recieveCam.orthographic = donorCam.orthographic;
			recieveCam.orthographicSize = donorCam.orthographicSize;
			recieveCam.fieldOfView = donorCam.fieldOfView;

			// Physical cam stuff
			recieveCam.usePhysicalProperties = donorCam.usePhysicalProperties;
			recieveCam.focalLength = donorCam.focalLength;
			recieveCam.sensorSize = donorCam.sensorSize;
			recieveCam.lensShift = donorCam.lensShift;
			recieveCam.gateFit = donorCam.gateFit;

			//Rest of stuff
			recieveCam.nearClipPlane = donorCam.nearClipPlane;
			recieveCam.farClipPlane = donorCam.farClipPlane;
			recieveCam.rect = donorCam.rect;
			recieveCam.depth = donorCam.depth;
			recieveCam.renderingPath = donorCam.renderingPath;
			recieveCam.useOcclusionCulling = donorCam.useOcclusionCulling;
			recieveCam.allowHDR = donorCam.allowHDR;
			recieveCam.allowMSAA = donorCam.allowMSAA;
			recieveCam.allowDynamicResolution = donorCam.allowDynamicResolution;

			recieveCam.targetDisplay = donorCam.targetDisplay;
		}

		#endregion

		#region Get Screenshot

		public static Texture2D GetScreenshot(this Camera camera, int width, int height, RenderTextureFormat texFormat = RenderTextureFormat.ARGB32)
		{
			// Create a new render texture and apply it to our camera
			RenderTexture curRenderTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor(width, height, texFormat, 16));
			curRenderTexture.name = $"[CameraUtils] - GetScreenshot : {System.DateTime.Now.ToString("HH: mm: ss")}";        // Give it a name so it is easier to track in memory

			// Finally render our camera
			camera.RenderCameraToRenderTexture(curRenderTexture);

			// Read all of our data into a new image
			Texture2D image = curRenderTexture.ToTexture2D(true);
			image.name = $"[CameraUtils] - GetScreenshot : {System.DateTime.Now.ToString("HH:mm:ss")}";      // Give it a name so it is easier to track in memory

			// Release our temporary texture since we no longer need it
			RenderTexture.ReleaseTemporary(curRenderTexture);

			//Invoke our event
			return image;
		}

		private static void RenderCameraToRenderTexture(this Camera camera, RenderTexture texture)
		{
			// Set the target for our camera to our texture, then reset our aspect
			camera.targetTexture = texture;

			// Modify our aspect
			float cachedCameraAspect = camera.aspect;
			camera.aspect = texture.width / (float)texture.height;

			// Now we should just be able to take a photo of our object
			RenderTexture cachedActiveTex = RenderTexture.active;
			RenderTexture.active = camera.targetTexture;

			// Clear our screen
			GL.Clear(true, true, Color.white);

			// Hack to avoid the overhead of re-rendering canvases everytime we try to create a screenshot
			// Code sourced from : https://forum.unity.com/threads/camera-render-seems-to-trigger-canvas-sendwillrendercanvases.462099/
#if !UNITY_IOS && !UNITY_ANDROID
			var canvasHackField = typeof(Canvas).GetField("willRenderCanvases", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			var canvasHackObject = canvasHackField.GetValue(null);
			canvasHackField.SetValue(null, null);
#endif

			// Force our camera to render
			camera.Render();

#if !UNITY_IOS && !UNITY_ANDROID
			canvasHackField.SetValue(null, canvasHackObject);
#endif

			// Return our Original active texture
			RenderTexture.active = cachedActiveTex;

			// Reset our changed values
			camera.targetTexture = null;
			camera.ResetAspect();
		}



		#endregion

	}

}