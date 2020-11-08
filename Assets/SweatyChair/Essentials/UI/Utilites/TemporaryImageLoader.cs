using SweatyChair;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TemporaryImageLoader : MonoBehaviour
{

	#region Enum

	private enum LoadType
	{
		Resources,
		Direct
	}

	#endregion

	#region Variables

	[Header("Settings")]
	[SerializeField] private bool _loadOnEnable = false;
	[Space]
	[SerializeField] private LoadType _loadType;
	[SerializeField] private string _loadLocation;
	[SerializeField] private Sprite _loadSprite;

	// Ref
	private Image _image;
	private Image image {
		get {
			if (_image == null)
				_image = GetComponent<Image>();
			return _image;
		}
	}

	// Cached loaded data
	private Texture2D _cachedTexture;
	private Sprite _cachedSprite;

	#endregion

	#region OnEnable / OnDisable

	private void OnEnable()
	{
		// Then If we load on enable, we do our stuff
		if (_loadOnEnable)
			Set();
	}

	private void OnDisable()
	{
		// Destroy all lingering data
		DestroyCachedRefs();

		// Then null our image Reference
		image.sprite = null;
	}

	#endregion

	#region Set

	/// <summary>
	/// Sets the image to be an image loaded from resources
	/// </summary>
	/// <param name="location"></param>
	public void SetFromResources(string location)
	{
		_loadType = LoadType.Resources;
		_loadLocation = location;

		Set();
	}

	/// <summary>
	/// Sets the image to be an existing Sprite
	/// </summary>
	/// <param name="sprite"></param>
	public void SetFromExisting(Sprite sprite)
	{
		_loadType = LoadType.Direct;
		_loadSprite = sprite;

		Set();
	}

	public void Set()
	{
		// Enforce we destroy all old data first
		DestroyCachedRefs();

		// Then Based off our data, load our image into our sprite
		Sprite outSprite = null;

		switch (_loadType) {
			case LoadType.Resources:
				outSprite = LoadFromResources(_loadLocation);
				break;
			case LoadType.Direct:
				outSprite = LoadSpriteDirect(_loadSprite);
				break;
		}

		// Finally assign our sprite to our image
		image.sprite = outSprite;
	}

	#region Internal

	private Sprite LoadFromResources(string location)
	{
		// Do some error Checking
		if (string.IsNullOrEmpty(location))
			Debug.LogWarning($"{GetType()} : Issues loading sprite, provided resources location is null or empty");

		// Then Attempt to load our sprite from resources
		Sprite tempSprite = Resources.Load<Sprite>(location);
		if (tempSprite == null) {

			// if loading the sprite was a bust, try loading a texture 2D from our location
			Texture2D cachedTexture = Resources.Load<Texture2D>(location);

			// Then if we are not null, attempt to create a sprite from this texture
			if (cachedTexture != null)
				_cachedSprite = PrefabToImage.CreateSprite(cachedTexture);

			// Make sure we assign back into our temp sprite variable, as this is what we return
			tempSprite = _cachedSprite;
		}

		// Do a final check to see if our sprite is still null, and if so, log error
		if (tempSprite == null)
			Debug.LogWarning($"{GetType()} : Issues loading sprite, Could not find texture or sprite in resources with path {location}");

		// Now that is all loaded, and cached, return our loaded data
		return tempSprite;
	}

	private Sprite LoadSpriteDirect(Sprite sprite)
	{
		// We just do some quick error checking here
		if (sprite == null)
			Debug.LogWarning($"{GetType()} : Issues loading sprite, provided sprite is null. Are you sure one has been assigned?");

		return sprite;
	}

	#endregion

	#endregion

	#region Utility

	private void DestroyCachedRefs()
	{
		// Destroy our texture
		if (_cachedTexture != null)
			Destroy(_cachedTexture);
		_cachedTexture = null;

		// Destroy our Sprite
		if (_cachedSprite != null)
			Destroy(_cachedSprite);
		_cachedSprite = null;
	}

	#endregion

}
