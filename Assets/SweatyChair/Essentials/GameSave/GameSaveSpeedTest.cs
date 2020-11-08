using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SweatyChair;

public class GameSaveSpeedTest : MonoBehaviour
{

	[SerializeField] private Text _resultText;
	[SerializeField] private int _repeat = 10000;
	
	public void TestSaveInt()
	{
		float time = Time.realtimeSinceStartup;
		int j = Random.Range(1, 999);
		for (int i = 1; i < _repeat; i++)
			GameSave.SetInt("TestInt", j);
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("GameSave.SetInt x {0} = {1}s, average {2}s per save", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestLoadInt()
	{
		GameSave.SetInt("TestInt", Random.Range(1, 999));
		float time = Time.realtimeSinceStartup;
		for (int i = 1; i < _repeat; i++)
			GameSave.GetInt("TestInt");
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("GameSave.GetInt x {0} = {1}s, average {2}s per load", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}
	
	public void TestSaveString()
	{
		float time = Time.realtimeSinceStartup;
		string s = RandomUtils.GetRandomName(12);
		for (int i = 1; i < _repeat; i++)
			GameSave.SetString("TestString", s);
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("GameSave.SetString x {0} = {1}s, average {2}s per save", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestLoadString()
	{
		GameSave.SetString("TestString", RandomUtils.GetRandomName(12));
		float time = Time.realtimeSinceStartup;
		for (int i = 1; i < _repeat; i++)
			GameSave.GetString("TestString");
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("GameSave.GetString x {0} = {1}s, average {2}s per load", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestSaveIntList()
	{
		float time = Time.realtimeSinceStartup;
		List<int> list = RandomUtils.GetRandomIntListRandomLength(10, 99);
		for (int i = 1; i < _repeat; i++)
			GameSave.SetIntList("TestIntList", list);
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("GameSave.SetIntList x {0} = {1}s, average {2}s per save", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestLoadIntList()
	{
		GameSave.SetIntList("TestIntList", RandomUtils.GetRandomIntListRandomLength(10, 99));
		float time = Time.realtimeSinceStartup;
		for (int i = 1; i < _repeat; i++)
			GameSave.GetIntList("TestIntList");
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("GameSave.GetIntList x {0} = {1}s, average {2}s per load", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestSaveIntDictionary()
	{
		float time = Time.realtimeSinceStartup;
		Dictionary<int, int> list = RandomUtils.GetRandomIntDictionaryRandomLength(10, 99);
		for (int i = 1; i < _repeat; i++)
			GameSave.SetIntDictionary("TestIntDictionary", list);
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("GameSave.SetIntDictionary x {0} = {1}s, average {2}s per save", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestLoadIntDictionary()
	{
		GameSave.SetIntDictionary("TestIntDictionary", RandomUtils.GetRandomIntDictionaryRandomLength(10, 99));
		float time = Time.realtimeSinceStartup;
		for (int i = 1; i < _repeat; i++)
			GameSave.GetIntDictionary("TestIntDictionary");
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("GameSave.GetIntDictionary x {0} = {1}s, average {2}s per load", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestSetInt()
	{
		float time = Time.realtimeSinceStartup;
		int j = Random.Range(1, 999);
		for (int i = 1; i < _repeat; i++)
			PlayerPrefs.SetInt("TestInt", j);
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("PlayerPrefs.SetInt x {0} = {1}s, average {2}s per save", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestGetInt()
	{
		PlayerPrefs.SetInt("TestInt", Random.Range(1, 999));
		float time = Time.realtimeSinceStartup;
		for (int i = 1; i < _repeat; i++)
			PlayerPrefs.GetInt("TestInt");
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("PlayerPrefs.GetInt x {0} = {1}s, average {2}s per load", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestSetString()
	{
		float time = Time.realtimeSinceStartup;
		string s = RandomUtils.GetRandomName(12);
		for (int i = 1; i < _repeat; i++)
			PlayerPrefs.SetString("TestString", s);
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("PlayerPrefs.SetString x {0} = {1}s, average {2}s per save", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}

	public void TestGetString()
	{
		PlayerPrefs.SetString("TestString", RandomUtils.GetRandomName(12));
		float time = Time.realtimeSinceStartup;
		for (int i = 1; i < _repeat; i++)
			PlayerPrefs.GetString("TestString");
		float spendTime = Time.realtimeSinceStartup - time;
		string result = string.Format("PlayerPrefs.GetString x {0} = {1}s, average {2}s per load", _repeat, spendTime, spendTime / _repeat);
		_resultText.text = result;
		Debug.Log(result);
	}


}