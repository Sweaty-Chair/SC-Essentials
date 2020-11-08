using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedWorldRotation : MonoBehaviour
{
    private Vector3 rot;
    private Transform tf;

    // Start is called before the first frame update
    void Start()
    {
        tf = transform;
        rot = tf.eulerAngles;   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tf.eulerAngles = rot;
    }
}
