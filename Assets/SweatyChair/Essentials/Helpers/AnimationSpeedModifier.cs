using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeedModifier : MonoBehaviour
{

    public float speed = 2;

    // Start is called before the first frame update
    void Start()
    {
        Animation anim = GetComponent<Animation>();
        if(anim != null) {
            anim[anim.clip.name].speed = speed;
        }
    }

}
