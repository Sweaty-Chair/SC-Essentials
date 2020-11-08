using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair
{

	public static class TransformUtils
	{

		/// <summary>
		/// Gets the transform by path, '/' is the default path separator in Unity.
		/// </summary>
		public static Transform GetTransformByPath(string path, char separator = '/')
		{
			if (string.IsNullOrEmpty(path))
				return null;

			// Just use the GameObject.Find
			GameObject go = GameObject.Find(path);
			if (go != null)
				return go.transform;

			// Find the first root Transform first
			string[] names = path.Split(separator);
			Transform rootTF = null;
			if (names != null && names.Length > 0) {
				GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
				foreach (GameObject rootGO in rootGOs) {
					if (rootGO.name == names[0]) {
						rootTF = rootGO.transform;
						break;
					}
				}
				if (rootTF == null)
					return null;
			}

			// Find the Transform with the path from root Transform
			if (names.Length > 1) {
				path = string.Join(separator.ToString(), names, 1, names.Length - 1);
				path = path.TrimEnd(' '); // Remove bank space at end
				return rootTF.Find(path);
			}

			return rootTF;
		}

		/// <summary>
		/// Gets the transform by path, '/' is the default path separator in Unity.
		/// </summary>
		public static Transform GetTransformByPath(Transform parentTF, string path)
		{
			if (string.IsNullOrEmpty(path) || parentTF == null)
				return null;

			return parentTF.Find(path);
		}

		public static Transform GetOrAddChildByPath(Transform parent, string path, GameObject prefab = null, char separator = '/')
		{
			//Check if our path is null
			if (string.IsNullOrWhiteSpace(path)) {
				return null;
			}

			//Split our path
			string[] splitPath = path.Split(separator);

			int startIndex = 0;

			//If our parent is null, create a new one
			if (parent == null) {

				//Find our object
				GameObject obj = GameObject.Find(splitPath[0]);
				if (obj == null) {
					//Create a new object if null
					parent = new GameObject().transform;
					parent.name = splitPath[0];
					parent.ResetTransform();

				} else {
					//Otherwise set our parent to be the transform
					parent = obj.transform;
				}
				startIndex = 1;
			}

			for (int i = startIndex; i < splitPath.Length; i++) {

				//See if we can find our first child
				Transform child = parent.Find(splitPath[i]);

				if (child == null) {
					//Create a new transform
					if (prefab == null) {
						child = new GameObject().transform;
					} else {
						child = Object.Instantiate(prefab).transform;
					}

					child.SetParent(parent);

					child.name = splitPath[i];
					child.ResetTransform();
				}

				parent = child;
			}

			return parent;
		}

		public static T GetComponentByPath<T>(string path) where T : Object
		{
			Transform targetTF = GetTransformByPath(path);
			return targetTF == null ? null : targetTF.GetComponent<T>();
		}

		public static Transform[] GetAllChildrenTF(this Transform tf)
		{
			//Create a new array with the length of our children array
			Transform[] children = new Transform[tf.childCount];

			//Then iterate through all children and add them to the array
			int count = 0;
			foreach (Transform child in tf) {
				children[count++] = child;
			}

			//Return our children
			return children;
		}

		public static GameObject[] GetGameObjectsInChildren(this Transform tf, bool includeInactive = false)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Transform child in tf) {
				if (includeInactive || child.gameObject.activeSelf) // Ignore if excluding inactive
					list.Add(child.gameObject);
			}
			return list.ToArray();
		}

		public static void DestroyAllChildren(this Transform tf)
		{
			foreach (Transform childTF in tf) {
				Object.Destroy(childTF.gameObject);
			}
		}

		public static void DestroyAllChildrenImmediate(this Transform tf)
		{
			int childs = tf.childCount;
			for (int i = childs - 1; i >= 0; i--) {
				Object.DestroyImmediate(tf.GetChild(i).gameObject);
			}
		}

		public static void DestroyChildrenExclude(this Transform tf, Transform excludeTF)
		{
			Transform[] childrenTFs = new Transform[tf.childCount];
			for (int i = 0, imax = tf.childCount; i < imax; i++)
				childrenTFs[i] = tf.GetChild(i);

			foreach (Transform childTF in childrenTFs) {
				if (childTF == excludeTF)
					continue;
				if (Application.isPlaying) {
					childTF.parent = null;
					Object.Destroy(childTF.gameObject);
				} else {
					Object.DestroyImmediate(childTF.gameObject);
				}
			}
		}

		public static void DestroyChildren(this Transform tf, string childrenName)
		{
			Transform[] childrenTFs = new Transform[tf.childCount];
			for (int i = 0, imax = tf.childCount; i < imax; i++)
				childrenTFs[i] = tf.GetChild(i);

			foreach (Transform childTF in childrenTFs) {
				if (childTF.name.Contains(childrenName)) {
					if (Application.isPlaying) {
						childTF.parent = null;
						Object.Destroy(childTF.gameObject);
					} else {
						Object.DestroyImmediate(childTF.gameObject);
					}
				}
			}
		}

		public static void DestroyChildrenExclude(this Transform tf, string excludeName)
		{
			Transform[] childrenTFs = new Transform[tf.childCount];
			for (int i = 0, imax = tf.childCount; i < imax; i++)
				childrenTFs[i] = tf.GetChild(i);

			foreach (Transform childTF in childrenTFs) {
				if (childTF.name.Contains(excludeName))
					continue;
				if (Application.isPlaying) {
					childTF.parent = null;
					Object.Destroy(childTF.gameObject);
				} else {
					Object.DestroyImmediate(childTF.gameObject);
				}
			}
		}

		/// <summary>
		/// Rename all children
		/// </summary>
		public static void RenameChildren(this Transform tf, string newName)
		{
			foreach (Transform t in tf)
				t.name = newName;
		}

		/// <summary>
		/// Disable all children
		/// </summary>
		public static void DisableChildren(this Transform tf)
		{
			tf.ToggleChildren(false);
		}

		/// <summary>
		/// Toggle the GameObject of all children
		/// </summary>
		public static void ToggleChildren(this Transform tf, bool isShown = true)
		{
			foreach (Transform t in tf)
				t.gameObject.SetActive(isShown);
		}

		/// <summary>
		/// Destroy all specfic components in next lower level children, exclude tf inself.
		/// </summary>
		public static void DestroyChildrenComponents<T>(this Transform tf) where T : Component
		{
			foreach (Transform t in tf) {
				T obj = t.GetComponent<T>();
				if (obj != null)
					Object.Destroy(obj);
			}
		}

		/// <summary>
		/// Destroy all specfic components in all lower level children, include tf inself.
		/// </summary>
		public static void DestroyAllChildrenComponents<T>(this Transform tf, bool includeInactive) where T : Component
		{
			T[] tArray = tf.GetComponentsInChildren<T>(includeInactive);
			foreach (T obj in tArray) {
				Object.Destroy(obj);
			}
		}

		/// <summary>
		/// Get the path of a transform, '/' is the default path separator in Unity.
		/// </summary>
		public static string ToPath(this Transform tf, char separator = '/')
		{
			if (tf == null)
				return string.Empty;
			if (tf.parent == null)
				return tf.name;
			return tf.parent.ToPath() + separator + tf.name;
		}

		/// <summary>
		/// Get the path of a component, '/' is the default path separator in Unity.
		/// </summary>
		public static string ToPath<T>(this T t, char separator = '/') where T : Component
		{
			if (t == null)
				return string.Empty;
			return t.transform.ToPath();
		}

		/// <summary>
		/// Get the path of a transform from a root transform, '/' is the default path separator in Unity.
		/// </summary>
		public static string ToPath(this Transform tf, Transform rootTF, char separator = '/')
		{
			if (tf == null || tf == rootTF)
				return string.Empty;
			if (tf.parent == null || tf.parent == rootTF)
				return tf.name;
			return tf.parent.ToPath(rootTF, separator) + separator + tf.name;
		}

		public static T GetOrAddComponent<T>(this Transform transform) where T : Component
		{
			if (transform == null)
				return null; // Just in case
			T component = transform.GetComponent<T>(); // Get component of type T
			if (component == null)
				component = transform.gameObject.AddComponent<T>(); // If our object is null, add it
			return component; // Return our new component
		}

		#region Have Children Look At parent

		public static void LookAtParentRecursively(Transform rootTransform)
		{
			// Go through each child in our parent, and make the children look at our parent
			foreach (var child in rootTransform)
				RecursiveLookAtParent(rootTransform);
		}
		private static void RecursiveLookAtParent(Transform thisTransform)
		{
			List<Transform> transforms = new List<Transform>();
			// Get all children and call this method recursively until we get an object which has no children
			foreach (Transform child in thisTransform)
				transforms.Add(child);

			// Then go through our children in the list to avoid issues with reordering children and foreach loops
			foreach (Transform child in transforms)
				RecursiveLookAtParent(child); // Call our look at recursively

			// If our object has a parent
			if (thisTransform.parent != null) {
				List<Transform> children = new List<Transform>();

				// Get all children as a list
				foreach (Transform child in thisTransform)
					children.Add(child);

				// Then unparent our children
				for (int i = 0; i < children.Count; i++)
					children[i].SetParent(null, true);

				// Cache our parent
				Transform cachedParent = thisTransform.parent;

				thisTransform.SetParent(null, true);

				// Now we set our look at to our parent
				thisTransform.LookAt(cachedParent, Vector3.forward);

				thisTransform.SetParent(cachedParent, true);

				// Now we can safely reparent all our children
				for (int i = 0; i < children.Count; i++)
					children[i].SetParent(thisTransform, true);
			}
		}

		#endregion

		#region Set Transforms Rotation Without Affecting Children

		/// <summary>
		/// Set the rotations of all child transforms in a heirarchy without affecting position or scale of the children
		/// </summary>
		/// <param name="rootTransform"></param>
		public static void SetRotationRecursively(Transform rootTransform, Quaternion rotation)
		{
			RecursiveSetRotation(rootTransform, rotation);
		}

		private static void RecursiveSetRotation(Transform thisTransform, Quaternion rotation)
		{
			List<Transform> transforms = new List<Transform>();
			// Get all children and call this method recursively until we get an object which has no children
			foreach (Transform child in thisTransform)
				transforms.Add(child);

			// Then go through our children in the list to avoid issues with reordering children and foreach loops
			foreach (Transform child in transforms)
				RecursiveSetRotation(child, rotation); // Call our look at recursively

			// If our object has a parent
			if (thisTransform.parent != null) {
				List<Transform> children = new List<Transform>();

				// Get all children as a list
				foreach (Transform child in thisTransform)
					children.Add(child);

				// Then unparent our children
				for (int i = 0; i < children.Count; i++)
					children[i].SetParent(null, true);

				// Now we set our look at to our parent
				thisTransform.rotation = rotation;

				// Now we can safely reparent all our children
				for (int i = 0; i < children.Count; i++)
					children[i].SetParent(thisTransform, true);
			}
		}

		/// <summary>
		/// Set the rotations of all child transforms in a heirarchy without affecting position or scale of the children
		/// </summary>
		/// <param name="rootTransform"></param>
		public static void SetRotationRecursively(Transform rootTransform, Vector3 rotation)
		{
			RecursiveSetRotation(rootTransform, rotation);
		}
		private static void RecursiveSetRotation(Transform thisTransform, Vector3 rotation)
		{
			List<Transform> transforms = new List<Transform>();
			// Get all children and call this method recursively until we get an object which has no children
			foreach (Transform child in thisTransform)
				transforms.Add(child);

			// Then go through our children in the list to avoid issues with reordering children and foreach loops
			foreach (Transform child in transforms)
				RecursiveSetRotation(child, rotation); // Call our look at recursively

			// If our object has a parent
			if (thisTransform.parent != null) {
				List<Transform> children = new List<Transform>();

				// Get all children as a list
				foreach (Transform child in thisTransform)
					children.Add(child);

				// Then unparent our children
				for (int i = 0; i < children.Count; i++)
					children[i].SetParent(null, true);

				// Now we set our look at to our parent
				thisTransform.eulerAngles = rotation;

				// Now we can safely reparent all our children
				for (int i = 0; i < children.Count; i++)
					children[i].SetParent(thisTransform, true);
			}
		}

		#endregion

		#region Set Transform Without affecting children

		public static void SetTransformWithoutAffectingChildren(this Transform rootTransform, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			// If our object has a parent
			List<Transform> children = new List<Transform>();

			// Get all children as a list
			foreach (Transform child in rootTransform)
				children.Add(child);

			// Then unparent our children
			for (int i = 0; i < children.Count; i++)
				children[i].SetParent(null, true);

			// Cache our parent
			Transform cachedParent = rootTransform.parent;
			rootTransform.SetParent(null, true);

			// Now we set our look at to our parent
			rootTransform.localPosition = position;
			rootTransform.localRotation = rotation;
			rootTransform.localScale = scale;

			rootTransform.SetParent(cachedParent, true);

			// Now we can safely reparent all our children
			for (int i = 0; i < children.Count; i++)
				children[i].SetParent(rootTransform, true);
		}

		#endregion

		#region Destroy Transform Without Affecting children

		public static void DestroyTransformWithoutAffectingChildren(Transform transform, bool destroyImmediate = false)
		{
			// Cache our parent
			Transform cachedParent = transform.parent;

			// If our object has a parent
			List<Transform> children = new List<Transform>();

			// Get all children as a list
			foreach (Transform child in transform)
				children.Add(child);

			// Then re-parent our children
			for (int i = 0; i < children.Count; i++)
				children[i].SetParent(cachedParent, true);

			// Finally destroy our object
			if (destroyImmediate)
				Object.DestroyImmediate(transform.gameObject);
			else
				Object.Destroy(transform.gameObject);
		}

		#endregion

		#region Get youngest shared parent

		/// <summary>
		/// Finds the first parent up the hierarchy that two transforms share.
		/// </summary>
		/// <param name="child1"></param>
		/// <param name="child2"></param>
		/// <returns>The first parent our objects share. Or returns null if they share no parent</returns>
		public static Transform GetYoungestSharedParent(Transform child1, Transform child2)
		{
			// If either are null, return false
			if (child1 == null || child2 == null) { return null; }

			// Split both our arrays
			string[] firstChildFullPathSplit = child1.ToPath().Split('/');
			string[] secondChildFullPathSplit = child2.ToPath().Split('/');

			// Inititalize our null
			Transform firstSharedParent = null;

			// Get our lowest child count to iterate over
			int lowestChildCount = Mathf.Min(firstChildFullPathSplit.Length, secondChildFullPathSplit.Length);

			// Then go through both of our lists attempting to find the first difference in our paths for our object
			for (int i = 0; i < lowestChildCount; i++) {
				if (firstChildFullPathSplit[i] != secondChildFullPathSplit[i]) {
					string transformPath = string.Join("/", firstChildFullPathSplit, 0, i);
					firstSharedParent = GetTransformByPath(transformPath);
					break;
				}
			}

			// Do a final check if our paths are the same at this index
			if (firstChildFullPathSplit[lowestChildCount - 1] == secondChildFullPathSplit[lowestChildCount - 1]) {
				string[] longestChildPath = (firstChildFullPathSplit.Length >= secondChildFullPathSplit.Length) ? firstChildFullPathSplit : secondChildFullPathSplit;
				string transformPath = string.Join("/", longestChildPath, 0, lowestChildCount);
				firstSharedParent = GetTransformByPath(transformPath);
			}

			return firstSharedParent;
		}

		#endregion

		#region Get if Child of Transform

		/// <summary>
		/// Returns whether two transforms occur as a child of either one in hierarchy
		/// </summary>
		/// <param name="transform1"></param>
		/// <param name="transform2"></param>
		/// <returns></returns>
		public static bool IsEitherChildOfTransform(Transform transform1, Transform transform2)
		{
			// If either are null, return false
			if (transform1 == null || transform2 == null) { return false; }

			// Optimization, we first check the depth of our children, and iterate based on which is smaller
			int tf1Depth = transform1.GetDepth();
			int tf2Depth = transform2.GetDepth();

			if (tf1Depth == tf2Depth) { return false; } // Cant be child of other if both are same level children

			// Then we determine which would be the parent, and which would be the child
			if (tf1Depth < tf2Depth)
				return IsChildOfTransform(transform1, transform2);
			else
				return IsChildOfTransform(transform2, transform1);
		}

		/// <summary>
		/// Returns whether a transform is a child of the specified parent transform
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		public static bool IsChildOfTransform(Transform parent, Transform child)
		{
			// If either are null, return false
			if (parent == null || child == null) { return false; }

			// Go through and check if our parent is null	
			if (child.parent == null)
				return false;

			// Then check if our parent is our transform
			if (child.parent == parent)
				return true;

			return IsChildOfTransform(parent, child.parent);
		}

		#endregion

		#region GetComponent

		/// <summary>
		/// Attempts to get the requested component on all sibling transforms in the hierarchy
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="transform"></param>
		/// <returns></returns>
		public static T GetComponentInSiblings<T>(this Transform transform) where T : Component
		{
			// Get if we have a non null transform
			if (transform == null)
				return null;

			// Or if we dont have a parent, get our own component
			if (transform.parent == null) {
				return transform.GetComponent<T>();
			}

			// Otherwise go through all of our child transforms in our parent and get component. Once we find one that matches, return it
			T component = null;
			foreach (Transform children in transform.parent) {
				component = children.GetComponent<T>();
				if (component != null)
					break;
			}

			return component;
		}

		/// <summary>
		/// Attempts to get the requested component on all sibling transforms in the hierarchy
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="transform"></param>
		/// <returns></returns>
		public static T GetComponentInTopLevelChildren<T>(this Transform transform) where T : Component
		{
			// Get if we have a non null transform
			if (transform == null)
				return null;

			T component = null;
			foreach (Transform child in transform) {
				component = child.GetComponent<T>();
				if (component != null)
					break;
			}
			return component;

		}

		#endregion

		#region Set LossyScale

		public static void SetLossyScale(this Transform trans, Vector3 scale, bool onGlobalAxis)
		{
			trans.localScale = Vector3.one;
			var m = trans.worldToLocalMatrix;
			if (onGlobalAxis) {
				m.SetColumn(0, new Vector4(m.GetColumn(0).magnitude, 0f));
				m.SetColumn(1, new Vector4(0f, m.GetColumn(1).magnitude));
				m.SetColumn(2, new Vector4(0f, 0f, m.GetColumn(2).magnitude));
			}
			m.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
			trans.localScale = m.MultiplyPoint(scale);
		}

		#endregion

		#region Get Transform Depth

		public static int GetDepth(this Transform tf)
		{
			int depth = 0;
			Transform currentTransform = tf;
			while (currentTransform.parent != null) {
				depth++;
				currentTransform = currentTransform.parent;
			}
			return depth;
		}

		#endregion

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Tools/Transforms/Print Selected Transforms Path")]
		public static void PrintSelectedTransformsPath()
		{
			GameObject[] selectedGameObjects = UnityEditor.Selection.gameObjects;
			foreach (GameObject selectedGo in selectedGameObjects)
				Debug.Log(selectedGo.transform.ToPath());
		}

#endif

		public static void ResetTransform(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;
			transform.localScale = Vector3.one;
		}

		public static void ResetGlobalTransform(this Transform transform)
		{
			transform.transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.SetLossyScale(Vector3.one, true);
		}

	}

}