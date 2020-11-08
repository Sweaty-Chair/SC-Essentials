using UnityEngine;
using System.Collections;

public class TouchChildrenOneByOneByDistance : MonoBehaviour
{

    [SerializeField] private float _showDistance = 60;
    [SerializeField] private float _interval = 0.15f;
    private Transform playerTF;
    private bool toggled = false;

    private void OnEnable()
    {
        playerTF = GameObject.FindGameObjectWithTag("Player").transform;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (playerTF == null) return;
        if (!toggled && Mathf.Abs(playerTF.position.x - transform.position.x) < _showDistance) {
            toggled = true;
            StartCoroutine(TouchingChildren());
        }
    }


    private IEnumerator TouchingChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            yield return new WaitForSeconds(_interval);
        }
    }

}
