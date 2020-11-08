using UnityEngine;
using UnityEngine.Events;
using System;

namespace SweatyChair.Events {
    public class MonoBehaviourCollision2DHandler : MonoBehaviour {
        public LayerMask collisionMask;

        [EnumFlag("Events")]
        public MonoCollisionType selectedCollisionEvents;

        //[Header ("Collision")]
        public UnityEvent onCollisionEnter2D;
        public UnityEvent onCollisionStay2D;
        public UnityEvent onCollisionExit2D;

        //[Header ("Trigger")]
        public UnityEvent onTriggerEnter2D;
        public UnityEvent onTriggerStay2D;
        public UnityEvent onTriggerExit2D;

        #region Collision

        void OnCollisionEnter2D(Collision2D collision) {
            if (onCollisionEnter2D != null && collisionMask == (collisionMask | (1 << collision.gameObject.layer)))
                onCollisionEnter2D.Invoke();
        }

        void OnCollisionStay2D(Collision2D collision) {
            if (onCollisionStay2D != null && collisionMask == (collisionMask | (1 << collision.gameObject.layer)))
                onCollisionStay2D.Invoke();
        }

        void OnCollisionExit2D(Collision2D collision) {
            if (onCollisionExit2D != null && collisionMask == (collisionMask | (1 << collision.gameObject.layer)))
                onCollisionExit2D.Invoke();
        }

        #endregion

        #region Trigger

        void OnTriggerEnter2D(Collider2D other) {
            if (onTriggerEnter2D != null && collisionMask == (collisionMask | (1 << other.gameObject.layer)))
                onTriggerEnter2D.Invoke();
        }

        void OnTriggerStay2D(Collider2D other) {
            if (onTriggerStay2D != null && collisionMask == (collisionMask | (1 << other.gameObject.layer)))
                onTriggerStay2D.Invoke();
        }

        void OnTriggerExit2D(Collider2D other) {
            if (onTriggerExit2D != null && collisionMask == (collisionMask | (1 << other.gameObject.layer)))
                onTriggerExit2D.Invoke();
        }

        #endregion
    }
}