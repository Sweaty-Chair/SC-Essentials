using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A function extend class to operate GameObject and components.
	/// </summary>
	public static class GameObjectUtils
	{

		/// <summary>
		/// Gets the object by path, '/' is the default path separator in Unity.
		/// </summary>
		public static GameObject GetObjectByPath(string nameOrPath, char separator = '/')
		{
			if (string.IsNullOrEmpty(nameOrPath))
				return null;

			// Just use the GameObject.Find
			GameObject go = GameObject.Find(nameOrPath);
			if (go != null)
				return go;

			string[] names = nameOrPath.Split(separator);
			if (names != null && names.Length > 0) {
				GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
				foreach (GameObject rootGO in rootGOs) {
					if (rootGO.name == names[0]) {
						go = rootGO;
						break;
					}
				}
				if (go == null)
					return null;
			}
			if (names.Length > 1) {
				string path = string.Join(separator.ToString(), names, 1, names.Length - 1);
				path = path.TrimEnd(' '); // Remove bank space at end
				Transform tf = go.transform.Find(path);
				return tf == null ? null : tf.gameObject;
			}

			return go;
		}

		/// <summary>
		/// Instantiate prefab in Resources and parent it to a GameObject.
		/// </summary>
		public static GameObject AddChild(this GameObject parent, GameObject prefab, bool instantiateInWorldSpace = false)
		{
			return GameObject.Instantiate(prefab, parent == null ? null : parent.transform, instantiateInWorldSpace);
		}

		/// <summary>
		/// Instantiate prefab in Resources and parent it to a GameObject.
		/// </summary>
		public static GameObject AddChild(this GameObject parent, string resourcesPrefabPath, bool instantiateInWorldSpace = false)
		{
			return GameObject.Instantiate(GetResourcePrefab(resourcesPrefabPath), parent.transform, instantiateInWorldSpace);
		}

		/// <summary>
		/// Get the prefab in Resource by path.
		/// </summary>
		private static GameObject GetResourcePrefab(string resourcesPrefabPath)
		{
			GameObject go = Resources.Load<GameObject>(resourcesPrefabPath);
			if (go == null) {
				Debug.LogErrorFormat("GameObjectHelper:GetResourcePrefab - Invalid path={0}", resourcesPrefabPath);
				return null;
			}
			return go;
		}

		/// <summary>
		/// Instantiate prefab in Resources by path and parent it to a GameObject by path.
		/// </summary>
		public static GameObject AddChildToPath(string parentPath, string resourcesPrefabPath)
		{
			return AddChildToPath(parentPath, GetResourcePrefab(resourcesPrefabPath));
		}

		/// <summary>
		/// Instantiate prefab and parent it to a GameObject by path.
		/// </summary>
		public static GameObject AddChildToPath(string parentPath, GameObject prefab)
		{
			if (prefab == null)
				return null;

			Transform parentTF = null;

			if (!string.IsNullOrEmpty(parentPath)) {
				parentTF = TransformUtils.GetTransformByPath(parentPath);
				if (parentTF == null)
					Debug.LogWarningFormat("GameObjectHelper:AddChildToPath - Invalid parent path={0}", parentPath);
			}

			return GameObject.Instantiate(prefab, parentTF == null ? null : parentTF);
		}

		public static void Destroy(GameObject go)
		{
			if (Application.isPlaying)
				GameObject.Destroy(go);
			else
				GameObject.DestroyImmediate(go);
		}

		public static T GetComponentInOldestParent<T>(this GameObject gameObject) where T : Component
		{
			return gameObject.transform.root.GetComponentInChildren<T>();
		}

		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			T component = gameObject.GetComponent<T>(); //Get component of type T
			if (component == null)
				component = gameObject.AddComponent<T>();   //If our object is null, add it
			return component;   //Return our new component
		}

		public static void SetActive(this GameObject[] gameObjects, bool value)
		{
			foreach (GameObject gameObject in gameObjects)
				gameObject.SetActive(value);
		}

		#region Copy Component

		#region Copy Add Component

		// Copy and paste component
		public static T CopyComponent<T>(T original, GameObject destination) where T : Component
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			return CopyComponent(original, destination, flags);
		}

		// Copy and paste component
		public static T CopyComponent<T>(T original, GameObject destination, BindingFlags flags) where T : Component
		{
			Type type = original.GetType();
			Component copy = destination.AddComponent(type);
			FieldInfo[] fields = type.GetFields(flags);
			foreach (FieldInfo field in fields)
				field.SetValue(copy, field.GetValue(original));
			return copy as T;
		}

		#endregion

		#region Copy Component

		// Copy and paste component
		public static T CopyComponent<T>(T original, T destination) where T : Component
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			return CopyComponent(original, destination, flags);
		}

		// Copy and paste component
		public static T CopyComponent<T>(T original, T destination, BindingFlags flags) where T : Component
		{
			Type type = original.GetType();
			FieldInfo[] fields = type.GetFields(flags);
			foreach (FieldInfo field in fields)
				field.SetValue(destination, field.GetValue(original));
			return destination as T;
		}

		#endregion

		#endregion

		#region Get If Requires

		private static bool Requires(Type obj, Type requirement)
		{
			// Also check for m_Type1 and m_Type2 if required
			return Attribute.IsDefined(obj, typeof(RequireComponent)) &&
				   Attribute.GetCustomAttributes(obj, typeof(RequireComponent)).OfType<RequireComponent>()
				   .Any(rc => rc.m_Type0.IsAssignableFrom(requirement));
		}

		internal static bool CanDestroy(this GameObject go, Type t)
		{
			return !go.GetComponents<Component>().Any(c => Requires(c.GetType(), t));
		}

		#endregion

		#region Create Gameobject

		public static GameObject CreateGameObjectChild(string childName, Transform parent, bool resetTransform = true)
		{
			// Create a new gameobject to hold our transforms
			GameObject childObject = new GameObject(childName);
			childObject.transform.SetParent(parent);

			if (resetTransform)
				childObject.transform.ResetTransform();

			return childObject;
		}

		#endregion

		#region Destroy all mono

		public static void TurnOffAllMono(this GameObject prefabInstance)
		{
			MonoBehaviour[] allBehaviours = prefabInstance.GetComponentsInChildren<MonoBehaviour>(true);
			for (int i = 0; i < allBehaviours.Length; i++)
				allBehaviours[i].enabled = false;
		}

		public static void DestroyAllMono(this GameObject prefabInstance, bool immediate = false)
		{
			MonoBehaviour[] allBehaviours = prefabInstance.GetComponentsInChildren<MonoBehaviour>(true);
			for (int i = allBehaviours.Length - 1; i >= 0; i--) {
				if (immediate)
					UnityEngine.Object.DestroyImmediate(allBehaviours[i]);
				else
					UnityEngine.Object.Destroy(allBehaviours[i]);
			}
		}

		#endregion

	}

}