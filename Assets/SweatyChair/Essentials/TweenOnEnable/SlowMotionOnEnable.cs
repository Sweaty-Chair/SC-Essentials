using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SweatyChair;

public class SlowMotionOnEnable : MonoBehaviour
{

    public float timeSpeed = 0.1f;
    public float duration = 0.75f;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(TimeDilate());
    }

    IEnumerator TimeDilate()
    {

        TimeScaleManager.SetTimescale(timeSpeed);

        yield return new WaitForSecondsRealtime(duration);

        // Then retrn everything back to normal
        TimeScaleManager.SetTimescale(1);
    }

}
