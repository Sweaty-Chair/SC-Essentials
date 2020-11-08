using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationOnTrigger : MonoBehaviour
{
    public float coolDown = 5;
    public Animation anim;
    public LayerMask collidables;

    private float nextHitTime;

    // Start is called before the first frame update
    void Start()
    {
        if(anim == null) {
            anim = GetComponentInChildren<Animation>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & collidables) != 0) {
            if(Time.time > nextHitTime) {
                anim.Play();
                nextHitTime = Time.time + coolDown;
            }
        }
    }
}
