using System.Linq;
using UnityEngine;

namespace SweatyChair
{

	public static class BoundsUtils
	{

		#region Get Bounds

		/// <summary>
		/// Gets the total world-space - Renderer bounds for a specific GameObject, and its children Objects.
		/// </summary>
		/// <param name="obj">The Gameobject which we are going to get the entire bounds</param>
		/// <returns></returns>
		public static Bounds GetBounds(GameObject obj, bool includeInactive = false)
		{
			if (obj == null)
				return new Bounds();

			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(includeInactive);
			return GetBounds(renderers, includeInactive);
		}
		public static Bounds GetBounds(Renderer[] objectRenderers, bool includeInactive)
		{
			// If we are ignoring inactive objects, only select our renderers which are enabled
			if (!includeInactive)
				objectRenderers = objectRenderers.Where(renderer => renderer.enabled).ToArray();

			// If we have any renderers, we add them to the list
			if (objectRenderers != null && objectRenderers.Length > 0) {

				// Intialize our bounds to our first bounds
				Bounds bounds = objectRenderers[0].bounds;

				// Then We add all the other ones
				for (int i = 1; i < objectRenderers.Length; i++) {
					// If our Bounds are default, we do not include. This solves issues with particle system null bounds
					if (objectRenderers[i].bounds != default)
						bounds.Encapsulate(objectRenderers[i].bounds);
				}

				return bounds;

			}
			else {
				return new Bounds();
			}
		}

		public static Bounds GetColliderBounds(GameObject obj)
		{
			if (obj == null)
				return new Bounds();

			Collider[] collider3Ds = obj.GetComponentsInChildren<Collider>();
			Collider2D[] collider2Ds = obj.GetComponentsInChildren<Collider2D>();

			return GetColliderBounds(collider3Ds, collider2Ds);
		}
		public static Bounds GetColliderBounds(Collider[] collider3Ds, Collider2D[] collider2Ds)
		{
			Bounds outBounds = new Bounds();

			// If we have any Colliders, we add them to the list
			if ((collider3Ds != null && collider3Ds.Length > 0) ||
				(collider2Ds != null && collider2Ds.Length > 0)) {

				// Intialize our bounds to our first valid bounds
				if (collider3Ds != null && collider3Ds.Length > 0)
					outBounds = collider3Ds[0].bounds;
				else
					outBounds = collider2Ds[0].bounds;

				// Then We add all the other ones
				if (collider3Ds != null) {
					for (int i = 0; i < collider3Ds.Length; i++)
						outBounds.Encapsulate(collider3Ds[i].bounds);
				}

				if (collider2Ds != null) {
					for (int i = 0; i < collider2Ds.Length; i++)
						outBounds.Encapsulate(collider2Ds[i].bounds);
				}

				return outBounds;

			}
			else {
				return outBounds;
			}
		}

		#endregion

		#region Bounds Encapsulates bounds?

		/// <summary>
		/// Checks whether an objects bounds is fully enclosed by another containing bounds.
		/// </summary>
		/// <param name="containingBounds"></param>
		/// <param name="objBounds"></param>
		/// <returns></returns>
		public static bool BoundsIsEncapsulated(Bounds containingBounds, Bounds objBounds)
		{
			return containingBounds.Contains(objBounds.min) && containingBounds.Contains(objBounds.max);
		}

		#endregion

		#region Transform Bounds

		public static Bounds TransformBounds(this Bounds bounds, Matrix4x4 transformationMatrix)
		{
			Vector3 center = transformationMatrix.MultiplyPoint(bounds.center);
			Vector3 size = transformationMatrix.MultiplyVector(bounds.size);

			return new Bounds(center, size);
		}

		#endregion

	}
}
