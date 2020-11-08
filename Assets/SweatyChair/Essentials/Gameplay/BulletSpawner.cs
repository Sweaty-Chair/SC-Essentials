using UnityEngine;
using System.Collections;
using SweatyChair.StateManagement;

namespace SweatyChair
{

	public class BulletSpawner : MonoBehaviour
	{

		[Tooltip("Fire starting wait time")]
		[SerializeField] protected float _fireStartTime = 0.5f;
		[Tooltip("Cooldown between each fire")]
		[SerializeField] protected float _cooldownSeconds = 2;
		[Tooltip("Fire duration")]
		[SerializeField] protected float _fireSeconds = 1;
		[Tooltip("Fire rate (number bullets per second)")]
		[SerializeField] protected float _fireRate = 10;

		[Tooltip("Randomize the direction for each bullet, ignore aim transform if set true")]
		[SerializeField] protected bool _randomRotationOnFire = false;
		[Tooltip("The bullet speed")]
		[SerializeField] protected float _bulletSpeed = 1;

		[Tooltip("The bullet prefab")]
		[SerializeField] protected Transform _bulletPrefab;
		[Tooltip("(Optional) aiming transform, leave null if firing upward (2D) / forward (3D) of the object")]
		[SerializeField] protected Transform _aimTF;

		protected float _nextFireTime = -1;

		protected virtual void OnEnable()
		{
			SetNextFireTime();
		}

		protected void FixedUpdate()
		{
			if (Time.time > _nextFireTime && StateManager.Compare(State.Game)) { // Don't spawn if not in game
				StartCoroutine(ShootCoroutine());
				_nextFireTime = Time.time + _fireSeconds + _cooldownSeconds;
			}
		}

		protected void SetNextFireTime()
		{
			_nextFireTime = Time.time + _fireStartTime;
		}

		protected IEnumerator ShootCoroutine()
		{
			float fireDuration = 0;
			float fireInterval = 1f / _fireRate;
			while (fireDuration < _fireSeconds) {
				Shoot();
				yield return new WaitForSeconds(fireInterval);
				fireDuration += fireInterval;
			}
		}

		public virtual void Shoot()
		{
			if (_randomRotationOnFire && _aimTF != null)
				_aimTF.rotation = Random.rotation;
			Vector3 aimPos = _aimTF == null ? transform.TransformPoint(Vector3.up) : _aimTF.position;
			if (_randomRotationOnFire && _aimTF != null)
				aimPos = transform.TransformDirection(Random.insideUnitCircle);
			if (_randomRotationOnFire)
				transform.rotation = Random.rotation;
			ObjectsGenerator.SpawnBullet(transform.position, aimPos, _bulletPrefab, _bulletSpeed, false);
		}

		public void SetFireRate(float fireRate)
		{
			_fireRate = fireRate;
		}

		public void SetFireRateScale(float scale)
		{
			_fireRate *= scale;
		}

		public void SetCooldownSeconds(float cooldownSeconds)
		{
			_cooldownSeconds = cooldownSeconds;
		}

		public void SetCooldownSecondsScale(float scale)
		{
			_cooldownSeconds *= scale;
		}

		public void SetBulletSpeedScale(float scale)
		{
			_bulletSpeed *= scale;
		}

	}

}