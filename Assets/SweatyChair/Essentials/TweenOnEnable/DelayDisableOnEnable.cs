using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDisableOnEnable : MonoBehaviour
{
    public float showTime = 3;

    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("DisableMe", showTime);
    }

    // Update is called once per frame
    void DisableMe()
    {
        gameObject.SetActive(false);
    }
}
