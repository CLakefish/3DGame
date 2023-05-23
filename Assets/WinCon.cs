using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCon : MonoBehaviour
{
    private static SphereCollider sc;
    private static Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        sc = gameObject.GetComponent<SphereCollider>();
        canvas = gameObject.GetComponentInChildren<Canvas>();
        canvas.enabled = false;
    }

    private void OnTriggerEnter()
    {
        canvas.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canvas.enabled = false;
    }
}
