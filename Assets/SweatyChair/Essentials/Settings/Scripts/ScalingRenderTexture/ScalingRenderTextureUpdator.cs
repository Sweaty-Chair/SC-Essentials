using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	/// <summary>
	/// Update the texture in a raw image to the render camera, use this for performance (scaling rendering).
	/// Author: RV
	/// </summary>
	[RequireComponent(typeof(RawImage))]
	public class ScalingRenderTextureUpdator : MonoBehaviour
	{

		private RawImage _texture;

		private void Awake()
		{
			_texture = GetComponent<RawImage>();
			ScalingRenderTextureManager.renderTextureCreated += ChangeTexture;
		}

		private void OnDestroy()
		{
			ScalingRenderTextureManager.renderTextureCreated -= ChangeTexture;
		}

		private void Start()
		{
			SweatyChair.ScalingRenderTextureManager.InitRenderTexture();
		}

		private void ChangeTexture(Texture texture)
		{
			if (_texture != null)
				_texture.texture = texture;
		}

	}

}