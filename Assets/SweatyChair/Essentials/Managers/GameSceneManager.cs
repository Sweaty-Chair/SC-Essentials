using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SweatyChair
{

	public static class GameSceneManager
	{

		#region Reload Scene

		public static void ReloadActiveScene(float waitSeconds)
		{
			TimeManager.Invoke(ReloadActiveScene, waitSeconds);
		}

		public static void ReloadActiveScene()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		#endregion;

		#region LoadScene

		public static void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, bool useTransition = false, UnityAction onCompleted = null)
		{
			if (useTransition)
				TransitionManager.LoadSceneTransition(sceneName, loadMode, 0.5f, TransitionVisual.FullFill, Color.black, EasingFunction.Ease.Linear, null, onCompleted, true); // Load a black scene transition between scenes
			else
				LoadSceneAsync(sceneName, loadMode, null, onCompleted); // Otherwise just load scene async
		}

		public static IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, UnityAction<float> onProgressChanged = null, UnityAction onCompleted = null)
		{
			IEnumerator loadRoutine = LoadSceneAsyncCoroutine(sceneName, loadMode, onProgressChanged, onCompleted); // Get a reference to our enumerator so we can yield on it
			return TimeManager.Start(loadRoutine);
		}

		public static IEnumerator LoadSceneAsyncCoroutine(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, UnityAction<float> onProgressChanged = null, UnityAction onCompleted = null)
		{
			// Load and wait for scene loaded
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadMode);

			while (!asyncLoad.isDone) { // Wait until our scene has loaded
				onProgressChanged?.Invoke(asyncLoad.progress);
				yield return null; // Check progress next frame
			}

			yield return null;

			// Call our other on complete action
			onCompleted?.Invoke();
		}

		#endregion

		#region UnloadScene

		public static IEnumerator UnloadSceneAsync(string sceneName, UnloadSceneOptions unloadOptions = UnloadSceneOptions.None, UnityAction<float> onProgressChanged = null, UnityAction onCompleted = null)
		{
			IEnumerator unloadRoutine = UnloadSceneAsyncCoroutine(sceneName, unloadOptions, onProgressChanged, onCompleted); // Get a reference to our enumerator so we can yield on it
			return TimeManager.Start(unloadRoutine);
		}

		public static IEnumerator UnloadSceneAsyncCoroutine(string sceneName, UnloadSceneOptions unloadOptions = UnloadSceneOptions.None, UnityAction<float> onProgressChanged = null, UnityAction onCompleted = null)
		{
			// Load and wait for scene loaded
			AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName, unloadOptions);

			while (!asyncUnload.isDone) { // Wait until our scene has loaded
				onProgressChanged?.Invoke(asyncUnload.progress);
				yield return null; // Check progress next frame
			}

			yield return null;

			// Call our other on complete action
			onCompleted?.Invoke();
		}

		#endregion

		#region Reload Scene

		public static IEnumerator ReloadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, UnloadSceneOptions unloadOptions = UnloadSceneOptions.None, UnityAction<float> onProgressChanged = null, UnityAction onCompleted = null)
		{
			IEnumerator loadRoutine = ReloadSceneAsyncCoroutine(sceneName, loadMode, unloadOptions, onProgressChanged, onCompleted); // Get a reference to our enumerator so we can yield on it
			return TimeManager.Start(loadRoutine);
		}

		public static IEnumerator ReloadSceneAsyncCoroutine(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, UnloadSceneOptions unloadOptions = UnloadSceneOptions.None, UnityAction<float> onProgressChanged = null, UnityAction onCompleted = null)
		{
			// Unload our scene, while modifying our return value because since we are unloading and unloading, we half our progress, and add half plus our load progress
			yield return UnloadSceneAsync(sceneName, unloadOptions, null, null);
			yield return null;
			yield return LoadSceneAsync(sceneName, loadMode, null, onCompleted);
		}

		#endregion

		#region SetActiveScenes

		//private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		//{
		//	EvaluateActiveScene();
		//}

		//private static void OnSceneUnloaded(Scene scene)
		//{
		//	EvaluateActiveScene();
		//}

		//private static void EvaluateActiveScene()
		//{
		//	// Get the name of our currently active scene
		//	string currentActiveScene = SceneManager.GetActiveScene().name;

		//	// If our active scene list already contains that scene. Then we dont need to select a new scene to be set as active
		//	if (activeSceneNames.Contains(currentActiveScene)) { return; }

		//	for (int i = 0; i < SceneManager.sceneCount; i++) {
		//		Scene currentScene = SceneManager.GetSceneAt(i);

		//		//If we get a scene that is within our active scene name array.
		//		//Set that scene as our active scene
		//		if (currentScene.isLoaded && activeSceneNames.Contains(currentScene.name)) {
		//			SceneManager.SetActiveScene(currentScene);
		//			return;
		//		}
		//	}
		//}

		// Check if a scene is loaded
		public static bool IsSceneLoaded(string sceneName)
		{
			// Check if the Game scene that's gonna do the loading of world
			for (int i = 0, iMax = SceneManager.sceneCount; i < iMax; i++) {
				if (SceneManager.GetSceneAt(i).name.Contains(sceneName))
					return true;
			}
			return false;
		}

		// Simply check if the scene is loaded before unload it
		public static AsyncOperation UnloadSceneAsync(string sceneName)
		{
			if (IsSceneLoaded(sceneName))
				return SceneManager.UnloadSceneAsync(sceneName);
			return null;
		}

		#endregion

	}

}