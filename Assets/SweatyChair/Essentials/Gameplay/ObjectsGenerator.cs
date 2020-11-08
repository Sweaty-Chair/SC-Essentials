using UnityEngine;
#if POOLS
using PathologicalGames;
#endif

namespace SweatyChair
{

	/// <summary>
	/// Spawn objects using pools.
	/// Setup:
	/// 1. Add PoolManager into the project.
	/// 2. Add "POOLS" into scripting symbol.
	/// </summary>
	public static class ObjectsGenerator
	{

		public static bool IsSpawned(Transform objectTF, string poolName = "Object")
		{
#if POOLS
			try {
				return PoolManager.Pools[poolName].IsSpawned(objectTF);
			} catch {
				Debug.LogWarningFormat("ObjectGenerator:IsSpawned - pool '{0}' not exists", poolName);
			}
#endif
			return false;
		}

		#region Spawn

		public static Transform Spawn(Transform objectTF, Vector2 pos, string poolName = "Object")
		{
			return Spawn(objectTF, (Vector3)pos, poolName);
		}

		public static Transform Spawn(Transform objectTF, Vector3 pos, string poolName = "Object")
		{
#if POOLS
			try {
				Transform tf = PoolManager.Pools[poolName].Spawn(objectTF, pos, Quaternion.Euler(Vector2.zero));
				tf.position = pos;
				return tf;
			} catch {
				Debug.LogWarningFormat("ObjectGenerator:Spawn - pool '{0}' not exists", poolName);
			}
#endif
			return null;
		}

		/// <summary>
		/// Spawn with local position.
		/// </summary>
		public static Transform SpawnLocal(Transform objectTF, Vector3 localPos, string poolName = "Object")
		{
			Transform tf = Spawn(objectTF, Vector3.zero, poolName);
			if (tf != null) {
				tf.localPosition = localPos;
				tf.localEulerAngles = Vector3.one;
				tf.localScale = Vector3.one;
			}
			return tf;
		}

		#endregion

		#region Despawn

		public static void Despawn(Transform objectTF, string poolName = "Object")
		{
#if POOLS
			try {
				if (PoolManager.Pools[poolName].IsSpawned(objectTF))
					PoolManager.Pools[poolName].Despawn(objectTF);
			} catch {
				Debug.LogWarningFormat("ObjectGenerator:Despawn - pool '{0}' not exists", poolName);
			}
#endif
		}

		public static void DespawnAll(string poolName = "Object")
		{
#if POOLS
			try {
				PoolManager.Pools[poolName].DespawnAll();
			} catch {
				Debug.LogWarningFormat("ObjectGenerator:DespawnAll - pool '{0}' not exists", poolName);
			}
#endif
		}

		#endregion

		/// <summary>
		/// Destroy all transform in the pool, uses this with care.
		/// </summary>
		public static void DestroyAll(string poolName)
		{
#if POOLS
			try {
				if (PoolManager.Pools.ContainsKey(poolName)) {
					var pool = PoolManager.Pools[poolName];
					Transform poolParentTF = pool.transform.parent;
					PoolManager.Pools.Destroy(poolName);
					var newPool = PoolManager.Pools.Create(poolName);
					newPool.transform.SetParent(poolParentTF);
					newPool.transform.localScale = Vector3.one; // Just in case
				}
			} catch (System.Exception e) {
				Debug.LogErrorFormat("ObjectGenerator:CleanUp({0}) - error={1}", poolName, e);
			}
#endif
		}

		public static GameObject GetPoolGameObject(string poolName = "Object")
		{
#if POOLS
			try {
				return PoolManager.Pools[poolName].gameObject;
			} catch {
				Debug.LogWarningFormat("ObjectGenerator:GetPoolGameObject - pool '{0}' not exists", poolName);
			}
#endif
			return null;
		}

		#region Bullets

		private static ObjectsGeneratorSettings _settings => ObjectsGeneratorSettings.current;

		public static int totalBullets { get; private set; }

		public static Transform SpawnBullet(Vector2 posFrom, Vector2 posTo, Transform bulletPrefabTF = null, float speedScaler = 2.5f, bool incrementTotalBulets = true)
		{
			Transform bulletTF = Spawn(bulletPrefabTF ?? _settings.defaultBulletTF, posFrom);
			if (bulletTF != null) { // Just in case
				SetBulletDirectionAndSpeed(bulletTF, posFrom, posTo, speedScaler * _settings.bulletSpeed);
				if (incrementTotalBulets)
					totalBullets++;
			}
			return bulletTF;
		}

		private static void SetBulletDirectionAndSpeed(Transform bulletTF, Vector2 posFrom, Vector2 posTo, float speed)
		{
			Rigidbody2D rigidbody = bulletTF.GetComponent<Rigidbody2D>();
			if (rigidbody) {
				Vector2 dir = (posTo - posFrom).normalized;
				bulletTF.eulerAngles = Vector3.forward * (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90); // Bullet prefabs are face up, -90deg to become 0deg
				bulletTF.GetComponent<Rigidbody2D>().velocity = dir * speed;
			}
		}

		#endregion

	}

}