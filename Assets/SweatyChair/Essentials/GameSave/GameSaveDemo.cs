using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SweatyChair;

public class GameSaveDemo : MonoBehaviour
{

	public Text coinText, gemText;

	private void Awake()
	{
		GameSaveSync.gameSaveUpdated += Start;
	}

	private void Start()
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		coinText.text = GameSave.GetInt("Coins").ToString();
		gemText.text = GameSave.GetInt("Gems").ToString();
	}

	[ContextMenu("Change Coin Gem")]
	public void ChangeCoinGem()
	{
		GameSave.SetInt("Coins", Random.Range(1, 99999), SyncPolicy.NoSync);
		GameSave.SetInt("Gems", Random.Range(1, 99999));
		UpdateUI();
	}

	[ContextMenu("Save Random List")]
	public void SaveRandomList()
	{
		GameSave.Set<List<int>>("RandomList", RandomUtils.GetRandomIntListRandomLength(1, 10));
		DebugUtils.Log(GameSave.Get<List<int>>("RandomList"));
	}

	[ContextMenu("Save Random Dictionary")]
	public void SaveRandomDictionary()
	{
		GameSave.Set<Dictionary<int, int>>("RandomDictionary", RandomUtils.GetRandomIntDictionaryRandomLength(1, 10));
		DebugUtils.Log(GameSave.Get<Dictionary<int, int>>("RandomDictionary"));
	}

	[ContextMenu("Print Game Save")]
	public void PrintGameSave()
	{
		GameSave.Log();
	}

	[ContextMenu("Print Bytes")]
	public void PrintBytes()
	{
		GameSave.LogBytes();
	}

	[ContextMenu("Delete Game Save")]
	public void DeleteGameSave()
	{
		GameSave.ResetSave();
		UpdateUI();
	}

	public void GoToPlayGameCenterDemo()
	{
#if UNITY_IOS
		UnityEngine.SceneManagement.SceneManager.LoadScene("GameCenterTestScene");
#else
		UnityEngine.SceneManagement.SceneManager.LoadScene("PlayGameServicesDemoScene");
#endif
	}

}