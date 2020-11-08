using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A script spawn and laser prefabs.
	/// </summary>
	public class LaserSpawner : MonoBehaviour
	{

		public static void Shoot(Vector3 from, Vector3 to, float duration)
		{
			GameObject prefab = Resources.Load<GameObject>("Laser");
			if (prefab == null) {
				Debug.LogError("LaserSpawner:Shoot - Laser prefab not found.");
				return;
			}
			GameObject go = Instantiate(prefab, from, Quaternion.identity);
			LineRenderer lineRenderer = go.GetComponent<LineRenderer>();
			lineRenderer.SetPosition(0, from);
			lineRenderer.SetPosition(1, to);
			SelfDestroyer sd = go.GetOrAddComponent<SelfDestroyer>();
			sd.SetDelay(duration);
		}

	}

}