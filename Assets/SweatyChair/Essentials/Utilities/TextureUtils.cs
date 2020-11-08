using UnityEngine;
using System.IO;

namespace SweatyChair
{

	public static class TextureUtils
	{

		#region Save

		/// <summary>
		/// Save a texture into an image in a given path.
		/// FilePath example: /User/ABC/image.jpg
		/// </summary>
		public static void Save(this Texture2D texture, string filePath)
		{
			if (string.IsNullOrEmpty(filePath)) {
				Debug.LogErrorFormat("TextureUtils:Save - filePath cannot be empty");
				return;
			}
			string filePathLower = filePath.ToLower();
			bool isJPG = filePathLower.EndsWith(".jpg", System.StringComparison.Ordinal) || filePathLower.EndsWith("jpeg", System.StringComparison.Ordinal);
			File.WriteAllBytes(filePath, isJPG ? texture.EncodeToJPG() : texture.EncodeToPNG());
		}

		/// <summary>
		/// Save a texture into an image in persisten data path, with a given file name.
		/// </summary>
		/// <param name="filename">Example: image.jpg, image.png</param>
		public static void SaveToPersistentDataPath(this Texture2D texture, string filename)
		{
			if (string.IsNullOrEmpty(filename)) {
				Debug.LogErrorFormat("TextureUtils:SaveInPersistentDataPath - filename cannot be empty");
				return;
			}
			texture.Save(Path.Combine(Application.persistentDataPath, filename));
		}

		/// <summary>
		/// Save a texture into an image in persisten data path, with a given file name; and save the file name in PlayerPrefs.
		/// </summary>
		/// <param name="filename">Example: image.jpg, image.png</param>
		/// <param name="key">Example: ScreenshotImage</param>
		public static void SaveAsPlayerPrefs(this Texture2D texture, string filename, string key)
		{
			if (string.IsNullOrEmpty(filename)) {
				Debug.LogErrorFormat("TextureUtils:SaveAsPlayerPrefs - filename cannot be empty");
				return;
			}
			PlayerPrefs.SetString(key, filename);
			SaveToPersistentDataPath(texture, PlayerPrefs.GetString(key));
		}

		/// <summary>
		/// Save an array of textures into images in persisten data path, with a given file name; and save the file names in PlayerPrefs.
		/// </summary>
		/// <param name="fileName">Example: image.jpg, image.png --> will saved as image001.jpg, image002.jpg, etc</param>
		public static void SaveAsPlayerPrefs(this Texture2D[] textures, string fileName, string key)
		{
			if (string.IsNullOrEmpty(fileName)) {
				Debug.LogErrorFormat("TextureUtils:SaveAsPlayerPrefs - filename cannot be empty");
				return;
			}
			int lastDotIndex = fileName.LastIndexOf('.');
			string filenameRaw = fileName.Substring(0, lastDotIndex);
			string filenameExtension = fileName.Substring(lastDotIndex);
			string[] filenameArray = new string[textures.Length];
			for (int i = 0, imax = textures.Length; i < imax; i++) {
				Texture2D texture = textures[i];
				string filename = filenameRaw + i.ToString("D3") + filenameExtension;
				filenameArray[i] = filename;
				texture.SaveToPersistentDataPath(filename);
			}
			PlayerPrefsX.SetStringArray(key, filenameArray);
		}

		#endregion

		#region Load

		/// <summary>
		/// Gets a image path from a PlayerPrefs key.
		/// </summary>
		public static string GetPlayerPrefsTexturePath(string key)
		{
			if (string.IsNullOrEmpty(key)) {
				Debug.LogErrorFormat("TextureUtils:GetPlayerPrefsTexturePath - key cannot be empty");
				return null;
			}
			if (!PlayerPrefs.HasKey(key)) {
				Debug.LogErrorFormat("TextureUtils:GetPlayerPrefsTexturePath - key not found");
				return null;
			}
			return Path.Combine(Application.persistentDataPath, PlayerPrefs.GetString(key));
		}

		/// <summary>
		/// Load a texture from an image with a given path.
		/// </summary>
		public static Texture2D LoadTexture(string filePath)
		{
			if (string.IsNullOrEmpty(filePath)) {
				Debug.LogErrorFormat("TextureUtils:LoadImage - filePath cannot be empty");
				return null;
			}
			Texture2D tex = null;
			if (File.Exists(filePath)) {
				byte[] bytes = File.ReadAllBytes(filePath);
				tex = new Texture2D(2, 2, TextureFormat.ARGB32, false) {
					hideFlags = HideFlags.HideAndDontSave
				};
				tex.LoadImage(bytes); // This will auto-resize the texture dimensions
			}
			return tex;
		}

		/// <summary>
		/// Load a texture from an image in persistent data path, with a given file name.
		/// </summary>
		public static Texture2D LoadFromPersistentDataPath(string filename)
		{
			if (string.IsNullOrEmpty(filename)) {
				Debug.LogErrorFormat("FileUtils:LoadPersistentDataImage - filename cannot be empty");
				return null;
			}
			return LoadTexture(Path.Combine(Application.persistentDataPath, filename));
		}

		/// <summary>
		/// Load a texture from an image in persistent data path, with a given PlayerPrefs key.
		/// </summary>
		public static Texture2D LoadTextureFromPlayerPrefs(string key)
		{
			if (string.IsNullOrEmpty(key)) {
				Debug.LogErrorFormat("FileUtils:LoadPlayerPrefsImage - key cannot be empty");
				return null;
			}
			if (!PlayerPrefs.HasKey(key)) {
				Debug.LogErrorFormat("FileUtils:LoadPlayerPrefsImage - key not found");
				return null;
			}
			return LoadFromPersistentDataPath(PlayerPrefs.GetString(key));
		}

		/// <summary>
		/// Load a texture from an image with a given path.
		/// </summary>
		public static Texture2D LoadTextureFromBytes(byte[] bytes)
		{
			Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false) {
				hideFlags = HideFlags.HideAndDontSave
			};
			tex.LoadImage(bytes);
			return tex;
		}

		/// <summary>
		/// Load an array of textures from images in persistent data path, with a given PlayerPrefs key.
		/// </summary>
		public static Texture2D[] LoadTexturesFromPlayerPrefs(string key)
		{
			if (string.IsNullOrEmpty(key)) {
				Debug.LogErrorFormat("FileUtils:LoadPlayerPrefsImage - key cannot be empty");
				return null;
			}
			if (!PlayerPrefs.HasKey(key)) {
				Debug.LogErrorFormat("FileUtils:LoadPlayerPrefsImage - key not found");
				return null;
			}
			string[] filenames = PlayerPrefsX.GetStringArray(key);
			Texture2D[] textures = new Texture2D[filenames.Length];
			for (int i = 0, imax = filenames.Length; i < imax; i++) {
				string filename = filenames[i];
				textures[i] = LoadTextureFromPlayerPrefs(filename);
			}
			return textures;
		}

		#endregion

		#region Texture as Bytes

		/// <summary>
		/// Save a texture as a byte file in a given path. (This won't use EncodeToJPG or EncodeToPNG and cannot be read outside)
		/// <param name="filePath">Example: /User/ABC/image.byte</param>
		/// </summary>
		public static void SaveAsBytes(this Texture2D texture, string filePath)
		{
			if (string.IsNullOrEmpty(filePath)) {
				Debug.LogErrorFormat("FileUtils:SaveImageAsBytes - filePath cannot be empty");
				return;
			}
			File.WriteAllBytes(filePath, texture.GetRawTextureData());
		}

		/// <summary>
		/// Save a texture into a byte file in persisten data path, with a given file name.
		/// </summary>
		/// <param name="fileName">Example: image.byte</param>
		public static void SaveAsBytesToPersistentDataPath(this Texture2D texture, string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) {
				Debug.LogErrorFormat("FileUtils:SavePersistentDataImageAsBytes - filename cannot be empty");
				return;
			}
			SaveAsBytes(texture, Path.Combine(Application.persistentDataPath, fileName));
		}

		/// <summary>
		/// Save a texture into a byte file in persisten data path, with a given file name; and save the file name in PlayerPrefs.
		/// </summary>
		/// <param name="fileName">Example: image.byte</param>
		/// <param name="key">Example: ScreenshotImage</param>
		public static void SaveAsBytesToPlayerPrefs(Texture2D texture, string fileName, string key)
		{
			if (string.IsNullOrEmpty(fileName)) {
				Debug.LogErrorFormat("FileUtils:SavePlayerPrefsImageAsBytes - filename cannot be empty");
				return;
			}
			PlayerPrefs.SetString(key, fileName);
			SaveAsBytesToPersistentDataPath(texture, PlayerPrefs.GetString(key));
		}

		/// <summary>
		/// Load a texture from a byte file in a given path, with given format, width and height.
		/// <param name="filePath">Example: /User/ABC/image.byte</param>
		/// </summary>
		public static Texture2D LoadImageFromBytes(string filePath, TextureFormat format, int width, int height)
		{
			if (string.IsNullOrEmpty(filePath)) {
				Debug.LogErrorFormat("FileUtils:LoadImageFromBytes - filePath cannot be empty");
				return null;
			}
			Texture2D tex = null;
			if (File.Exists(filePath)) {
				byte[] bytes = File.ReadAllBytes(filePath);
				tex = new Texture2D(width, height, format, false) {
					hideFlags = HideFlags.HideAndDontSave
				};
				tex.LoadRawTextureData(bytes);
			}
			return tex;
		}

		/// <summary>
		/// Load a texture from a byte file in persistent data path, with a given file name, format, width and height.
		/// <param name="fileName">Example: image.byte</param>
		/// </summary>
		public static Texture2D LoadPersistentDataImageFromBytes(string filename, TextureFormat format, int width, int height)
		{
			if (string.IsNullOrEmpty(filename)) {
				Debug.LogErrorFormat("FileUtils:LoadPersistentDataImageFromBytes - filename cannot be empty");
				return null;
			}
			return LoadImageFromBytes(Path.Combine(Application.persistentDataPath, filename), format, width, height);
		}

		/// <summary>
		/// Load a texture from a byte file in persistent data path, with a given PlayerPrefs key, format, width and height.
		/// </summary>
		public static Texture2D LoadPlayerPrefsImageFromBytes(string key, TextureFormat format, int width, int height)
		{
			if (string.IsNullOrEmpty(key)) {
				Debug.LogErrorFormat("FileUtils:LoadPlayerPrefsImageFromBytes - key cannot be empty");
				return null;
			}
			if (!PlayerPrefs.HasKey(key)) {
				Debug.LogErrorFormat("FileUtils:LoadPlayerPrefsImageFromBytes - key not found");
				return null;
			}
			return LoadPersistentDataImageFromBytes(PlayerPrefs.GetString(key), format, width, height);
		}

		#endregion

		#region Sprites

		public static Sprite LoadSprite(string filePath)
		{
			// Initialize our sprite to null
			Sprite sprite = null;

			// Load our image from our path
			Texture2D texture = LoadTexture(filePath);

			// If our image isn't null and does indeed exist, create our sprite and assign it for return
			if (texture != null)
				sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

			return sprite;
		}

		public static Sprite ToSprite(this Texture2D texture)
		{
			// Quick check to see if our texture is null
			if (texture == null)
				return null;

			return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
		}

		#endregion

		#region Conversion

		public static Texture2D ToTexture2D(this RenderTexture renderTexture, bool isTransParent = false)
		{
			Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, isTransParent ? TextureFormat.ARGB32 : TextureFormat.RGB24, false);
			RenderTexture.active = renderTexture;
			tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			tex.Apply();
			return tex;
		}

		#endregion

	}

}