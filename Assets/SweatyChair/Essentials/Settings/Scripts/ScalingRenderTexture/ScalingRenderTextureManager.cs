using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SweatyChair
{

	/// <summary>
	/// Update the texture in a raw image to the render camera, use this for performance (camera rednering).
	/// Author: RV
	/// </summary>
	public static class ScalingRenderTextureManager
	{

		private static readonly Dictionary<SettingManager.GraphicsSetting, int> RENDER_TEXTURE_HEIGHT_DICT = new Dictionary<SettingManager.GraphicsSetting, int> {
			{ SettingManager.GraphicsSetting.Best, 720 },
			{ SettingManager.GraphicsSetting.Good, 480 },
			{ SettingManager.GraphicsSetting.Fast, 480 },
			{ SettingManager.GraphicsSetting.Fastest, 360 },
		};

		private static float _rendererTextureHeight;

		private static float _rendererTextureWidth => _rendererTextureHeight * SettingManager.screenRatio;

		public static float resolutionRatio => _rendererTextureWidth / Screen.width;

		public static event UnityAction<RenderTexture> renderTextureCreated;

		public static Camera renderCamera { get; private set; }

		static ScalingRenderTextureManager()
		{
			SettingManager.graphicsQualityChanged += (dump) => InitRenderTexture();
		}

		public static void InitRenderTexture()
		{
			_rendererTextureHeight = RENDER_TEXTURE_HEIGHT_DICT[SettingManager.currentGraphicsSetting];
			SetRendererTexture(Mathf.RoundToInt(_rendererTextureWidth), Mathf.RoundToInt(_rendererTextureHeight));
			//Debug.LogFormat("ScalingRenderTextureManager:InitRenderTexture - width={0}, height={1}", Mathf.RoundToInt(_rendererTextureWidth), Mathf.RoundToInt(_rendererTextureHeight));
		}

		public static void SetRenderCamera(Camera camera)
		{
			renderCamera = camera;
		}

		private static void SetRendererTexture(int width, int height)
		{
			RenderTexture renderTexture = new RenderTexture(width, height, 24) {
				name = string.Format("GameTexture{0}_{1}", width, height),
				antiAliasing = 8
			};
			if (renderCamera == null)
				renderCamera = Camera.main;
			renderCamera.targetTexture?.Release();
			renderCamera.targetTexture = renderTexture;
			renderTextureCreated?.Invoke(renderTexture);
			TimeManager.Start(ResetCameraCoroutine());
		}

		private static IEnumerator ResetCameraCoroutine()
		{
			renderCamera.enabled = false;
			yield return null;
			renderCamera.enabled = true;
		}

	}

}