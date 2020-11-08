using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//AUTHOR: RV
// Creates a rendertexture that's mainly use for UI that requires to show gameObjects that are created at runtime.
// Usage: Normally you want to place the generated gameObject to the holder then Generate the RenderTexture
// Make sure to destroy or remove the children of holderTF after generating RenderTexture
namespace SweatyChair
{
	public class RenderTextureCreator : PresetSingleton<RenderTextureCreator>
	{
		private static int _generatedRenderTextureCounter = 0;
		[SerializeField] private Camera _myCamera;
		[SerializeField] private Transform _holderTF;

		public static Transform holderTF {
			get {
				if (instance._holderTF == null)
					return instance.transform;
				return instance._holderTF;
			}
		}

		public static RenderTexture GetRenderTexture(Transform tf, Vector3Int size, RenderTextureFormat format = RenderTextureFormat.ARGB32, bool destroyChildAfterUse = true)
		{
			if (tf == null)
				return null;

			tf.parent = holderTF;
			tf.localPosition = Vector3.zero;
			RenderTexture renderTexture = GetRenderTexture(size, format);
			return renderTexture;
		}

		public static RenderTexture GetRenderTexture(Vector3Int size, RenderTextureFormat format = RenderTextureFormat.ARGB32)
		{
			return instance.GenerateRenderTexture(size, format);
		}

		private RenderTexture GenerateRenderTexture(Vector3Int size, RenderTextureFormat format = RenderTextureFormat.ARGB32)
		{
			RenderTexture renderTexture = new RenderTexture(size.x, size.y, size.z, format);
			renderTexture.antiAliasing = 2;
			renderTexture.name = string.Format("GeneratedRenderTexture{0}", _generatedRenderTextureCounter);
			renderTexture.Create();
			_generatedRenderTextureCounter++;
			_myCamera.targetTexture = renderTexture;
			return renderTexture;
		}

		public static void Reset()
		{
			instance._myCamera.targetTexture = null;
		}

	}
}
