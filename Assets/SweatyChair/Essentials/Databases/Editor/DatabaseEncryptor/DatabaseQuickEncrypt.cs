using UnityEngine;
using UnityEditor;
using System.IO;

public class DatabaseQuickEncrypt
{

	[MenuItem("Tools/Databases Encryptor/Encrypt CardDatabase", false, 200)]
	public static void EncryptCardDatabase()
	{
		EncryptDatabase("CardDatabase");
	}

	[MenuItem("Tools/Databases Encryptor/Encrypt ConsumableDatabase", false, 200)]
	public static void EncryptConsumableDatabase()
	{
		EncryptDatabase("ConsumableDatabase");
	}

	[MenuItem("Tools/Databases Encryptor/Encrypt EnemyDatabase", false, 200)]
	public static void EncryptEnemyDatabase()
	{
		EncryptDatabase("EnemyDatabase");
	}

	[MenuItem("Tools/Databases Encryptor/Encrypt GadgetDatabase", false, 200)]
	public static void EncryptGadgetDatabase()
	{
		EncryptDatabase("GadgetDatabase");
	}

	[MenuItem("Tools/Databases Encryptor/Encrypt DungeonDatabase", false, 200)]
	public static void EncryptDungeonDatabase()
	{
		EncryptDatabase("DungeonDatabase");
	}

	[MenuItem("Tools/Databases Encryptor/Encrypt ResourceDatabase", false, 200)]
	public static void EncryptResourceDatabase()
	{
		EncryptDatabase("ResourceDatabase");
	}

	[MenuItem("Tools/Databases Encryptor/Encrypt RewardDatabase", false, 200)]
	public static void EncryptRewardDatabase()
	{
		EncryptDatabase("RewardDatabase");
	}

	[MenuItem("Tools/Databases Encryptor/Encrypt ShopDatabase", false, 200)]
	public static void EncryptShopDatabase()
	{
		EncryptDatabase("ShopDatabase");
	}

	[MenuItem("Tools/Databases Encryptor/Encrypt WorldDatabase", false, 200)]
	public static void EncryptWorldDatabase()
	{
		EncryptDatabase("WorldDatabase");
	}

	public static void EncryptDatabase(string filename)
	{
		if (DatabaseEncryptor.EncryptFile(GetDatatbaseRawProjectPath(filename + ".csv"), GetDatatbaseEncryptedProjectPath(filename + ".bytes")))
			Debug.LogFormat("{0} has successfully encrypted.", filename);
	}

	private static string GetDatatbaseRawProjectPath(string filename)
	{
		string filePath = Path.Combine(DatabaseEncryptor.dbRawProjectPath, filename);
		Debug.Log(filePath);
		Debug.Log(File.Exists(filePath));
		if (File.Exists(filePath))
			return filePath;
		// Try getting from Resources/DatabaseRaw
		var csvTextAsset = Resources.Load<TextAsset>("DatabaseRaw/" + filename.Replace(".csv", ""));
		if (csvTextAsset != null) {
			Debug.Log(AssetDatabase.GetAssetPath(csvTextAsset));
			return AssetDatabase.GetAssetPath(csvTextAsset);
		}
		// Try getting from Resources
		csvTextAsset = Resources.Load<TextAsset>(filename.Replace(".csv", ""));
		if (csvTextAsset != null) {
			Debug.Log(AssetDatabase.GetAssetPath(csvTextAsset));
			return AssetDatabase.GetAssetPath(csvTextAsset);
		}
		return string.Empty;
	}

	private static string GetDatatbaseEncryptedProjectPath(string filename)
	{
		string path = Path.Combine(DatabaseEncryptor.dbEncryptedProjectPath, filename);
		Debug.Log(path);
		#if UNITY_ANDROID
		path = path.Remove(0,7);
		Debug.Log(path);
		#endif
		return path;
	}
	
}