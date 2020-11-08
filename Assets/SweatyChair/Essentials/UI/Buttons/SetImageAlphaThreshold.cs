using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetImageAlphaThreshold : MonoBehaviour
{

	#region Variables

	[Header("Settings")]
	[SerializeField] [Range(0, 1)] private float alphaTestThreshold = 0;

	private Image image;

	#endregion

	#region OnEnable / OnDisable

	private void OnEnable() {
		//Get our image component
		image = GetComponent<Image>();
		//Set our hit threshold to our set one
		image.alphaHitTestMinimumThreshold = alphaTestThreshold;

		if (!image.mainTexture.isReadable) { Debug.LogWarningFormat("Unable to change Alpha hit threshold for object '{0}'. Sprites texture is not marked as readable in the import settings", gameObject.name); }
	}

	private void OnDisable() {
		//Reset back to the default alpha test threshold
		image.alphaHitTestMinimumThreshold = 0;
	}

	#endregion

}
