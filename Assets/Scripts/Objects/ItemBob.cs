/*
Author: Carson L
Date: 5/26/2023
Desc: Script that animates an object by bobbing it up and down
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBob : MonoBehaviour
{
    public AnimationCurve curve;
    public GameObject attached;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, attached.transform.position.y + curve.Evaluate((Time.time % curve.length)), transform.position.z);
    }
}
