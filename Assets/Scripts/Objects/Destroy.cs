/*
Author: Carson L 
Date: 5/24/2023
Desc: Script that destroys an object shortly after runtime starts
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
    }
}
