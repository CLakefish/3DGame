/*
Author: Carson L
Date: 5/17
Desc: Script for objects that look at a target
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTarget : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.Translate(Vector3.right * (5f * Time.deltaTime));
    }
}
