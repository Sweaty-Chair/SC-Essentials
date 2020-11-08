using UnityEngine;
using System.Collections;


public class PlayAnimationByDistance : MonoBehaviour
{
    [SerializeField] private float _showDistance = 60;
    [SerializeField] private AnimationClip _clip;

    private Transform playerTF;
    private Animation anim;

    private void OnEnable()
    {
        playerTF = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponentInChildren<Animation>();
    }


    void FixedUpdate()
    {
        if (Mathf.Abs(playerTF.position.x - transform.position.x) < _showDistance) {
            anim.Play(_clip.name);
            Destroy(this);
        }
    }


}
