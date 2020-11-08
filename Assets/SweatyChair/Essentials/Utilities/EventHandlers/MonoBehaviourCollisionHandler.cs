using UnityEngine;
using UnityEngine.Events;
using System;

namespace SweatyChair.Events
{
	[Flags]
	public enum MonoCollisionType
	{
		onCollisionEnter = 1<<0,
		onCollisionStay = 1<<1,
		onCollisionExit = 1<<2,
		onTriggerEnter = 1<<3,
		onTriggerStay = 1<<4,
		onTriggerExit = 1<<5
	}

	public class MonoBehaviourCollisionHandler : MonoBehaviour
	{
		public LayerMask collisionMask;

		[EnumFlag("Events")]
		public MonoCollisionType selectedCollisionEvents;

		//[Header ("Collision")]
		public UnityEvent onCollisionEnter;
		public UnityEvent onCollisionStay;
		public UnityEvent onCollisionExit;

		//[Header ("Trigger")]
		public UnityEvent onTriggerEnter;
		public UnityEvent onTriggerStay;
		public UnityEvent onTriggerExit;

		#region Collision

		void OnCollisionEnter (Collision collision)
		{
			if (onCollisionEnter != null && collisionMask == (collisionMask | (1 << collision.gameObject.layer)))
                onCollisionEnter.Invoke ();
		}

		void OnCollisionStay (Collision collision)
		{
			if (onCollisionStay != null && collisionMask == (collisionMask | (1 << collision.gameObject.layer)))
                onCollisionStay.Invoke ();
		}

		void OnCollisionExit (Collision collision)
		{
			if (onCollisionExit != null && collisionMask == (collisionMask | (1 << collision.gameObject.layer)))
                onCollisionExit.Invoke ();
		}

		#endregion

		#region Trigger

		void OnTriggerEnter (Collider other)
		{
            if (onTriggerEnter != null && collisionMask == (collisionMask | (1 << other.gameObject.layer)))
				onTriggerEnter.Invoke ();
		}

		void OnTriggerStay (Collider other)
		{
			if (onTriggerStay != null && collisionMask == (collisionMask | (1 << other.gameObject.layer)))
                onTriggerStay.Invoke ();
		}

		void OnTriggerExit (Collider other)
		{
			if (onTriggerExit != null && collisionMask == (collisionMask | (1 << other.gameObject.layer)))
                onTriggerExit.Invoke ();
		}

		#endregion
	}
}