using UnityEngine;

namespace SweatyChair
{
	public static class LayerUtils
	{

		#region Set Layer Recursive

		public static void SetLayerRecursively(this GameObject obj, string layerName)
		{
			SetLayerRecursively(obj, LayerMask.NameToLayer(layerName));
		}

		public static void SetLayerRecursively(this GameObject obj, int layer)
		{
			obj.layer = layer;
			foreach (Transform child in obj.transform)
				child.gameObject.SetLayerRecursively(layer);
		}

		/// <summary>
		/// Sets the layer recursively for objects having T component only.
		/// </summary>
		public static void SetLayerRecursivelyInclude<T>(this GameObject obj, string layerName)
		{
			SetLayerRecursivelyInclude<T>(obj, LayerMask.NameToLayer(layerName));
		}

		public static void SetLayerRecursivelyInclude<T>(this GameObject obj, int layer)
		{
			T t = obj.GetComponent<T>();
			if (t != null)
				obj.layer = layer;
			foreach (Transform child in obj.transform)
				child.gameObject.SetLayerRecursivelyInclude<T>(layer);
		}

		/// <summary>
		/// Sets the layer recursively excluding the objects having T component.
		/// </summary>
		public static void SetLayerRecursivelyExclude<T>(this GameObject obj, string layerName)
		{
			SetLayerRecursivelyExclude<T>(obj, LayerMask.NameToLayer(layerName));
		}

		public static void SetLayerRecursivelyExclude<T>(this GameObject obj, int layer)
		{
			if (obj.GetComponent<T>() == null)
				obj.layer = layer;
			foreach (Transform child in obj.transform)
				child.gameObject.SetLayerRecursivelyExclude<T>(layer);
		}

		#endregion

		#region Contain Layermask?

		public static bool ContainsLayer(this LayerMask layerMask, int layerIndex)
		{
			return layerMask == (layerMask | 1 << layerIndex);
		}

		#endregion

	}
}