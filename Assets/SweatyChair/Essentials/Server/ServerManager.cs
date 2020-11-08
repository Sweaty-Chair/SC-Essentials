using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SweatyChair
{

	public static class ServerManager
	{

		private static ServerSettings _settings = ServerSettings.current;

		private static int _gameId => _settings.gameId;

		private static string _domain => _settings.domain;

		#region Auth Token

		private static string _token;

		public static void SetToken(string token)
		{
			_token = token;
		}

		#endregion

		#region Get

		public static IEnumerator Get(string requestString)
		{
			return Get(requestString, (Hashtable ht) => { }, (Hashtable err) => { });
		}

		public static IEnumerator Get(string requestString, UnityAction<Hashtable> onSucceed)
		{
			return Get(requestString, onSucceed, (Hashtable err) => { });
		}

		// Get and return a json on success, or a json error on fail.
		public static IEnumerator Get(string requestString, UnityAction<Hashtable> onSucceed, UnityAction<Hashtable> onFailed)
		{
			return Get(requestString, new Dictionary<string, string>(), onSucceed, onFailed);
		}

		// Get with token auth header, return a json on success, or a json error on fail.
		public static IEnumerator GetWithToken(string requestString, UnityAction<Hashtable> onSucceed, UnityAction<Hashtable> onFailed)
		{
			Dictionary<string, string> header = new Dictionary<string, string> {
				{ "Accept", "application/json" },
				{ "Authorization", string.Format("Bearer {0}", _token) },
			};
			return Get(requestString, header, onSucceed, onFailed);
		}

		// Get with a header, and return a json on success, or a json error on fail.
		public static IEnumerator Get(string requestString, Dictionary<string, string> header, UnityAction<Hashtable> onSucceed, UnityAction<Hashtable> onFailed)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Get - url={0}", _domain + requestString);

			UnityWebRequest request = UnityWebRequest.Get(_domain + requestString);
			if (header != null) {
				foreach (var kvp in header)
					request.SetRequestHeader(kvp.Key, kvp.Value);
			}

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Get and return a json on success, or a string error on fail.
		public static IEnumerator Get(string requestString, UnityAction<Hashtable> onSucceed, UnityAction<string> onFailed)
		{
			return Get(requestString, new Dictionary<string, string>(), onSucceed, onFailed);
		}

		// Get with token auth header, return a json on success, or a json error on fail.
		public static IEnumerator GetWithToken(string requestString, UnityAction<Hashtable> onSucceed, UnityAction<string> onFailed)
		{
			Dictionary<string, string> header = new Dictionary<string, string> {
				{ "Accept", "application/json" },
				{ "Authorization", string.Format("Bearer {0}", _token) },
			};
			return Get(requestString, header, onSucceed, onFailed);
		}

		// Get with a header, and return a json on success, or a json error on fail.
		public static IEnumerator Get(string requestString, Dictionary<string, string> header, UnityAction<Hashtable> onSucceed, UnityAction<string> onFailed)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Get - url={0}", _domain + requestString);

			UnityWebRequest request = UnityWebRequest.Get(_domain + requestString);
			if (header != null) {
				foreach (var kvp in header)
					request.SetRequestHeader(kvp.Key, kvp.Value);
			}

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		public static IEnumerator Get(string requestString, UnityAction<Hashtable[]> onSucceed)
		{
			return Get(requestString, onSucceed, (string s) => { });
		}

		// Get and return a json array on success, or a json error on fail.
		public static IEnumerator Get(string requestString, UnityAction<Hashtable[]> onSucceed, UnityAction<Hashtable> onFailed)
		{
			return Get(requestString, null, onSucceed, onFailed);
		}

		// Get with token auth header, return a json on success, or a json error on fail.
		public static IEnumerator GetWithToken(string requestString, UnityAction<Hashtable[]> onSucceed, UnityAction<Hashtable> onFailed)
		{
			Dictionary<string, string> header = new Dictionary<string, string> {
				{ "Accept", "application/json" },
				{ "Authorization", string.Format("Bearer {0}", _token) },
			};
			return Get(requestString, header, onSucceed, onFailed);
		}

		// Get with a header, and return a json on success, or a json error on fail.
		public static IEnumerator Get(string requestString, Dictionary<string, string> header, UnityAction<Hashtable[]> onSucceed, UnityAction<Hashtable> onFailed)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable[0]);
				return null;
			}

			CheckDynamicParameters(ref requestString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Get - url={0}", _domain + requestString);

			UnityWebRequest request = UnityWebRequest.Get(_domain + requestString);
			if (header != null) {
				foreach (var kvp in header)
					request.SetRequestHeader(kvp.Key, kvp.Value);
			}

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Get and return a json array on success, or a json error on fail.
		public static IEnumerator Get(string requestString, UnityAction<Hashtable[]> onSucceed, UnityAction<string> onFailed)
		{
			return Get(requestString, null, onSucceed, onFailed);
		}

		// Get with token auth header, return a json on success, or a json error on fail.
		public static IEnumerator GetWithToken(string requestString, UnityAction<Hashtable[]> onSucceed, UnityAction<string> onFailed)
		{
			Dictionary<string, string> header = new Dictionary<string, string> {
				{ "Accept", "application/json" },
				{ "Authorization", string.Format("Bearer {0}", _token) },
			};
			return Get(requestString, header, onSucceed, onFailed);
		}

		// Get and return a json array on success, or a string error on fail.
		public static IEnumerator Get(string requestString, Dictionary<string, string> header, UnityAction<Hashtable[]> onSucceed, UnityAction<string> onFailed)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable[0]);
				return null;
			}

			CheckDynamicParameters(ref requestString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Get - url={0}", _domain + requestString);

			UnityWebRequest request = UnityWebRequest.Get(_domain + requestString);
			if (header != null) {
				foreach (var kvp in header)
					request.SetRequestHeader(kvp.Key, kvp.Value);
			}

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		public static IEnumerator Get(string requestString, UnityAction<Hashtable> onSucceed, UnityAction<string[]> onFailed = null)
		{
			return Get(requestString, null, onSucceed, onFailed);
		}

		public static IEnumerator Get(string requestString, Dictionary<string, string> header, UnityAction<Hashtable> onSucceed = null, UnityAction<string[]> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Get - url={0}", _domain + requestString);

			UnityWebRequest request = UnityWebRequest.Get(_domain + requestString);
			if (header != null) {
				foreach (var kvp in header)
					request.SetRequestHeader(kvp.Key, kvp.Value);
			}

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		public static IEnumerator GetTexture(string requestString, UnityAction<Texture2D> onSucceed, UnityAction<string> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(null);
				yield break;
			}

			CheckDynamicParameters(ref requestString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:GetTexture - url={0}", _domain + requestString);

			using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_domain + requestString)) {
				yield return uwr.SendWebRequest();
				if (uwr.isNetworkError || uwr.isHttpError)
					onFailed?.Invoke(uwr.error);
				else
					onSucceed?.Invoke(DownloadHandlerTexture.GetContent(uwr));
			}
		}

		#endregion

		#region Post

		// Post without data, return json (if any) on success, or a json errors on fail.
		public static IEnumerator Post(string requestString, UnityAction<Hashtable> onSucceed = null, UnityAction<Hashtable> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Post - url={0}", _domain + requestString);

			UnityWebRequest request = UnityWebRequest.Post(_domain + requestString, new Dictionary<string, string>());

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Post without data, return json (if any) on success, or a string array errors on fail.
		public static IEnumerator Post(string requestString, UnityAction<Hashtable> onSucceed = null, UnityAction<string[]> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Post - url={0}", _domain + requestString);

			UnityWebRequest request = UnityWebRequest.Post(_domain + requestString, new Dictionary<string, string>());

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Post without data, return json (if any) on success, or a string error on fail.
		public static IEnumerator Post(string requestString, UnityAction<Hashtable> onSucceed = null, UnityAction<string> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Post - url={0}", _domain + requestString);

			UnityWebRequest request = UnityWebRequest.Post(_domain + requestString, new Dictionary<string, string>());

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Post with dictionary form, return json (if any) on success, or a json error on fail.
		public static IEnumerator Post(string requestString, Dictionary<string, string> dict, UnityAction<Hashtable> onSucceed, UnityAction<Hashtable> onFailed)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);
			CheckDynamicParameters(ref dict);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Post - url={0}, request={1}", _domain + requestString, dict.toJson());

			UnityWebRequest request = UnityWebRequest.Post(_domain + requestString, dict);

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Post with dictionary form, return json (if any) on success, or a string array error on fail.
		public static IEnumerator Post(string requestString, Dictionary<string, string> dict, UnityAction<Hashtable> onSucceed, UnityAction<string[]> onFailed)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);
			CheckDynamicParameters(ref dict);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Post - url={0}, request={1}", _domain + requestString, dict.toJson());

			UnityWebRequest request = UnityWebRequest.Post(_domain + requestString, dict);

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Post with dictionary form, return json (if any) on success, or a string error on fail.
		public static IEnumerator Post(string requestString, Dictionary<string, string> dict, UnityAction<Hashtable> onSucceed = null, UnityAction<string> onFailed = null)
		{
			Dictionary<string, string> header = new Dictionary<string, string> {
				{ "Accept", "application/json" },
			};
			return Post(requestString, dict, header, onSucceed, onFailed);
		}

		// Post with dictionary form and token auth header, return json (if any) on success, or a json error on fail.
		public static IEnumerator PostWithToken(string requestString, Dictionary<string, string> dict, UnityAction<Hashtable> onSucceed = null, UnityAction<Hashtable> onFailed = null)
		{
			Dictionary<string, string> header = new Dictionary<string, string> {
				{ "Accept", "application/json" },
				{ "Authorization", string.Format("Bearer {0}", _token) },
			};
			return Post(requestString, dict, header, onSucceed, onFailed);
		}

		// Post with dictionary form and header, return json (if any) on success, or a json error on fail.
		public static IEnumerator Post(string requestString, Dictionary<string, string> dict, Dictionary<string, string> header, UnityAction<Hashtable> onSucceed = null, UnityAction<Hashtable> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);
			CheckDynamicParameters(ref dict);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Post - url={0}, request={1}", _domain + requestString, dict.toJson());

			UnityWebRequest request = UnityWebRequest.Post(_domain + requestString, dict);
			foreach (var kvp in header)
				request.SetRequestHeader(kvp.Key, kvp.Value);

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Post with dictionary form and token auth header, return json (if any) on success, or a json error on fail.
		public static IEnumerator PostWithToken(string requestString, Dictionary<string, string> dict, UnityAction<Hashtable> onSucceed, UnityAction<string> onFailed)
		{
			Dictionary<string, string> header = new Dictionary<string, string> {
				{ "Accept", "application/json" },
				{ "Authorization", string.Format("Bearer {0}", _token) },
			};
			return Post(requestString, dict, header, onSucceed, onFailed);
		}

		// Post with dictionary form and header, return json (if any) on success, or a string error on fail.
		public static IEnumerator Post(string requestString, Dictionary<string, string> dict, Dictionary<string, string> header, UnityAction<Hashtable> onSucceed, UnityAction<string> onFailed)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);
			CheckDynamicParameters(ref dict);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Post - url={0}, request={1}", _domain + requestString, dict.toJson());

			UnityWebRequest request = UnityWebRequest.Post(_domain + requestString, dict);
			foreach (var kvp in header)
				request.SetRequestHeader(kvp.Key, kvp.Value);

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Post with json, return json (if any) on success, or a json error on fail.
		public static IEnumerator Post(string requestString, Hashtable postJson, UnityAction<Hashtable> onSucceed = null, UnityAction<Hashtable> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);

			string jsonString = postJson.ToString();
			CheckDynamicParameters(ref jsonString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Post - url={0}, json={1}", _domain + requestString, jsonString);

			UnityWebRequest request = UnityWebRequest.Post(_domain + requestString, jsonString);
			request.SetRequestHeader("Content-Type", "text/json");
			request.SetRequestHeader("Content-Length", jsonString.Length.ToString());

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		#endregion

		#region Put

		// Put with dictionary form, return json (if any) on success, or a json error on fail.
		public static IEnumerator Put(string requestString, Dictionary<string, string> dict, UnityAction<Hashtable> onSucceed = null, UnityAction<Hashtable> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);
			CheckDynamicParameters(ref dict);

			var jsonHT = new Hashtable(dict);
			string jsonString = jsonHT.toJson();

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Put - url={0}, json={1}", _domain + requestString, jsonString);

			UnityWebRequest request = UnityWebRequest.Put(_domain + requestString, jsonString);
			request.SetRequestHeader("Content-Type", "text/json");

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Put with dictionary form, return json (if any) on success, or a string error on fail.
		public static IEnumerator Put(string requestString, Dictionary<string, string> dict, UnityAction<Hashtable> onSucceed = null, UnityAction<string> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);
			CheckDynamicParameters(ref dict);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Put - url={0}, json={1}", _domain + requestString, dict.toJson());

			var jsonHT = new Hashtable(dict);
			string jsonString = jsonHT.toJson();

			UnityWebRequest request = UnityWebRequest.Put(_domain + requestString, jsonString);
			request.SetRequestHeader("Content-Type", "text/json");

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Put with dictionary form and auth token header, return json (if any) on success, or a json error on fail.
		public static IEnumerator PutWithToken(string requestString, Dictionary<string, string> dict, UnityAction<Hashtable> onSucceed = null, UnityAction<Hashtable> onFailed = null)
		{
			Dictionary<string, string> header = new Dictionary<string, string> {
				{ "Accept", "application/json" },
				{ "Content-Type", "application/json" },
				{ "Authorization", string.Format("Bearer {0}", _token) },
			};
			return Put(requestString, dict, header, onSucceed, onFailed);
		}

		// Put with dictionary form and header, return json (if any) on success, or a json error on fail.
		public static IEnumerator Put(string requestString, Dictionary<string, string> dict, Dictionary<string, string> header, UnityAction<Hashtable> onSucceed = null, UnityAction<Hashtable> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);
			CheckDynamicParameters(ref dict);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Put - url={0}, json={1}", _domain + requestString, dict.toJson());

			var postJson = new Hashtable(dict);
			string jsonString = postJson.toJson();

			UnityWebRequest request = UnityWebRequest.Put(_domain + requestString, jsonString);
			request.SetRequestHeader("Content-Type", "text/json");
			foreach (var kvp in header)
				request.SetRequestHeader(kvp.Key, kvp.Value);

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Put with dictionary form and token auth header, return json (if any) on success, or a json error on fail.
		public static IEnumerator PutWithToken(string requestString, Dictionary<string, string> dict, UnityAction<Hashtable> onSucceed = null, UnityAction<string> onFailed = null)
		{
			Dictionary<string, string> header = new Dictionary<string, string> {
				{ "Accept", "application/json" },
				{ "Content-Type", "application/json" },
				{ "Authorization", string.Format("Bearer {0}", _token) },
			};
			return Put(requestString, dict, header, onSucceed, onFailed);
		}

		// Put with dictionary form and header, return json (if any) on success, or a json error on fail.
		public static IEnumerator Put(string requestString, Dictionary<string, string> dict, Dictionary<string, string> header, UnityAction<Hashtable> onSucceed = null, UnityAction<string> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);
			CheckDynamicParameters(ref dict);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Put - url={0}, json={1}", _domain + requestString, dict.toJson());

			var postJson = new Hashtable(dict);
			string jsonString = postJson.toJson();

			UnityWebRequest request = UnityWebRequest.Put(_domain + requestString, jsonString);
			request.SetRequestHeader("Content-Type", "text/json");
			foreach (var kvp in header)
				request.SetRequestHeader(kvp.Key, kvp.Value);

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		// Put with json, return json (if any) on success, or json error on fail.
		public static IEnumerator Put(string requestString, Hashtable dataHt, UnityAction<Hashtable> onSucceed = null, UnityAction<Hashtable> onFailed = null)
		{
			if (ServerSettings.current.offlineMode) {
				onSucceed?.Invoke(new Hashtable());
				return null;
			}

			CheckDynamicParameters(ref requestString);

			string jsonString = dataHt.ToString();
			CheckDynamicParameters(ref jsonString);

			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:Put - url={0}, json={1}", _domain + requestString, jsonString);

			UnityWebRequest request = UnityWebRequest.Put(_domain + requestString, jsonString);
			request.SetRequestHeader("Content-Type", "text/json");

			IEnumerator routine = WaitForRequest(request, onSucceed, onFailed);
			TimeManager.Start(routine);
			return routine;
		}

		public static IEnumerator CheckValid(string url, UnityAction<bool> onSucceed = null)
		{
			UnityWebRequest request = UnityWebRequest.Head(url);
			IEnumerator routine = WaitForHeadRequest(request, onSucceed);
			TimeManager.Start(routine);
			return routine;
		}

		#endregion

		#region Wait For Request

		// Wait for a response as hashtable, or errors as hashtable.
		private static IEnumerator WaitForRequest(UnityWebRequest webRequest, UnityAction<Hashtable> onSucceed, UnityAction<Hashtable> onFailed)
		{
			yield return webRequest.SendWebRequest();
			if (!webRequest.isNetworkError && !webRequest.isHttpError)
				InvokeSucceedCallback(webRequest, onSucceed);
			else
				InvokeFailedCallback(webRequest, onFailed);
		}

		// Wait for a resonpse as hashtable, or errors as string array.
		private static IEnumerator WaitForRequest(UnityWebRequest webRequest, UnityAction<Hashtable> onSucceed, UnityAction<string[]> onFailed)
		{
			yield return webRequest.SendWebRequest();
			if (!webRequest.isNetworkError && !webRequest.isHttpError)
				InvokeSucceedCallback(webRequest, onSucceed);
			else
				InvokeFailedCallback(webRequest, onFailed);
		}

		// Wait for a resonpse as hashtable, or error as string.
		private static IEnumerator WaitForRequest(UnityWebRequest webRequest, UnityAction<Hashtable> onSucceed, UnityAction<string> onFailed)
		{
			yield return webRequest.SendWebRequest();
			if (!webRequest.isNetworkError && !webRequest.isHttpError)
				InvokeSucceedCallback(webRequest, onSucceed);
			else
				InvokeFailedCallback(webRequest, onFailed);
		}

		// Wait for a resonpse as hashtable array, or errors as hashtable.
		private static IEnumerator WaitForRequest(UnityWebRequest webRequest, UnityAction<Hashtable[]> onSucceed, UnityAction<Hashtable> onFailed)
		{
			yield return webRequest.SendWebRequest();
			if (!webRequest.isNetworkError && !webRequest.isHttpError)
				InvokeSucceedCallback(webRequest, onSucceed);
			else
				InvokeFailedCallback(webRequest, onFailed);
		}

		// Wait for a resonpse as hashtable array, or errors as string array.
		private static IEnumerator WaitForRequest(UnityWebRequest webRequest, UnityAction<Hashtable[]> onSucceed, UnityAction<string[]> onFailed)
		{
			yield return webRequest.SendWebRequest();
			if (!webRequest.isNetworkError && !webRequest.isHttpError)
				InvokeSucceedCallback(webRequest, onSucceed);
			else
				InvokeFailedCallback(webRequest, onFailed);
		}

		// Wait for a resonpse as hashtable array, or error as string.
		private static IEnumerator WaitForRequest(UnityWebRequest webRequest, UnityAction<Hashtable[]> onSucceed, UnityAction<string> onFailed)
		{
			yield return webRequest.SendWebRequest();
			if (!webRequest.isNetworkError && !webRequest.isHttpError)
				InvokeSucceedCallback(webRequest, onSucceed);
			else
				InvokeFailedCallback(webRequest, onFailed);
		}

		// Wait for a head resonpse, true if valid
		private static IEnumerator WaitForHeadRequest(UnityWebRequest webRequest, UnityAction<bool> onSucceed)
		{
			yield return webRequest.SendWebRequest();
			onSucceed?.Invoke(!webRequest.isNetworkError && !webRequest.isHttpError && webRequest.responseCode >= 200);
		}

		#endregion

		#region Helpers

		// Add dynamic parameters into the request json string.
		private static void CheckDynamicParameters(ref string requestString)
		{
			if (requestString.Contains("{gameId}"))
				requestString = requestString.Replace("{gameId}", _gameId.ToString());
			if (requestString.Contains("{playerId}"))
				requestString = requestString.Replace("{playerId}", ServerPlayerManager.playerId.ToString());
		}

		// Add dynamic parameters into the request dictionary.
		private static void CheckDynamicParameters(ref Dictionary<string, string> dict)
		{
			if (dict != null) {
				foreach (KeyValuePair<string, string> kvp in dict) {
					if (kvp.Value.Contains("{gameId}"))
						dict[kvp.Key] = kvp.Value.Replace("{gameId}", _gameId.ToString());
					if (kvp.Value.Contains("{playerId}"))
						dict[kvp.Key] = kvp.Value.Replace("{playerId}", ServerPlayerManager.playerId.ToString());
				}
			}
		}

		private static void InvokeSucceedCallback(UnityWebRequest webRequest, UnityAction<Hashtable> onSucceed)
		{
			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:InvokeSucceedCallback - Succeed: url={0}, text={1}", webRequest.url, webRequest.downloadHandler?.text);
			if (onSucceed != null) {
				Hashtable result = null;
				try {
					if (webRequest.downloadHandler == null)
						Debug.LogError("ServerManager:InvokeSucceedCallback - Failed: webRequest.downloadHandler=null");
					else
						result = webRequest.downloadHandler.text.hashtableFromJson();
				} catch (NullReferenceException e) {
					Debug.LogErrorFormat("ServerManager:InvokeSucceedCallback - Failed: url={0}, text={1}, exception={2}", webRequest.url, webRequest.downloadHandler.text, e);
					//throw e;
				}
				onSucceed(result);
			}
		}

		private static void InvokeSucceedCallback(UnityWebRequest webRequest, UnityAction<Hashtable[]> onSucceed)
		{
			if (_settings.debugMode)
				Debug.LogFormat("ServerManager:InvokeSucceedCallback - Succeed: url={0}, text={1}", webRequest.url, webRequest.downloadHandler?.text);
			if (onSucceed != null) {
				Hashtable[] result = null;
				try {
					if (webRequest.downloadHandler == null) {
						Debug.LogError("ServerManager:InvokeSucceedCallback - Failed: webRequest.downloadHandler=null");
					} else {
						object[] tmp = webRequest.downloadHandler.text.arrayListFromJson().ToArray();
						result = new Hashtable[tmp.Length];
						for (int i = 0, imax = tmp.Length; i < imax; i++)
							result[i] = (Hashtable)tmp[i];
					}
				} catch (Exception e) {
					Debug.LogErrorFormat("ServerManager:InvokeSucceedCallback - Failed: url={0}, text={1}, exception={2}", webRequest.url, webRequest.downloadHandler?.text, e);
					//throw e;
				}
				onSucceed(result);
			}
		}

		private static void InvokeFailedCallback(UnityWebRequest webRequest, UnityAction<Hashtable> onFailed)
		{
			if (_settings.debugMode)
				Debug.LogErrorFormat("ServerManager:WaitForRequest - Failed: url={0}, text={1}, error={2}", webRequest.url, webRequest.downloadHandler?.text, webRequest.error);
			Hashtable resultHt = string.IsNullOrEmpty(webRequest.downloadHandler?.text) ? webRequest.error?.hashtableFromJson() : webRequest.downloadHandler?.text?.hashtableFromJson();
			if (resultHt != null && resultHt.ContainsKey("errors"))
				onFailed?.Invoke(resultHt["errors"] as Hashtable);
			else
				onFailed?.Invoke(new Hashtable() { { "error", webRequest.error } });
		}

		private static void InvokeFailedCallback(UnityWebRequest webRequest, UnityAction<string[]> onFailed)
		{
			if (_settings.debugMode)
				Debug.LogErrorFormat("ServerManager:WaitForRequest - Failed: url={0}, text={1}, error={2}", webRequest.url, webRequest.downloadHandler?.text, webRequest.error);
			Hashtable resultHt = string.IsNullOrEmpty(webRequest.downloadHandler?.text) ? webRequest.error?.hashtableFromJson() : webRequest.downloadHandler?.text?.hashtableFromJson();
			if (resultHt != null && resultHt.ContainsKey("errors")) {
				var errorsHt = resultHt["errors"] as Hashtable;
				var enumerator = errorsHt.Keys.GetEnumerator();
				enumerator.MoveNext();
				var errorsArray = errorsHt[enumerator.Current.ToString()] as ArrayList;
				var errorObjects = errorsArray.ToArray();
				onFailed?.Invoke(Array.ConvertAll(errorObjects, x => x.ToString()));
			} else {
				onFailed?.Invoke(new string[] { webRequest.error });
			}
		}

		private static void InvokeFailedCallback(UnityWebRequest webRequest, UnityAction<string> onFailed)
		{
			if (_settings.debugMode)
				Debug.LogErrorFormat("ServerManager:WaitForRequest - Failed: url={0}, text={1}, error={2}", webRequest.url, webRequest.downloadHandler?.text, webRequest.error);
			string error = "Please make sure you have Internet connection and try again later.";
			Hashtable resultHt = string.IsNullOrEmpty(webRequest.downloadHandler?.text) ? webRequest.error?.hashtableFromJson() : webRequest.downloadHandler?.text?.hashtableFromJson();
			if (resultHt != null) {
				if (resultHt.ContainsKey("errors")) { // Get the first error if errors is a list
					var errorsHt = resultHt["errors"] as Hashtable;
					var enumerator = errorsHt.Keys.GetEnumerator();
					enumerator.MoveNext();
					var errorsArray = errorsHt[enumerator.Current.ToString()] as ArrayList;
					error = (errorsArray.ToArray())[0].ToString();
				} else if (resultHt.ContainsKey("error")) { // Try get the single error
					error = resultHt["error"] as string;
				} else if (resultHt.ContainsKey("message")) { // Try get the message
					error = resultHt["message"] as string;
				}
			}
			onFailed?.Invoke(error);
		}

		#endregion

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Users/Print Auth Token", false, 400)]
		private static void PrintAuthToken()
		{
			Debug.Log("token=" + _token);
		}

#endif

	}

}