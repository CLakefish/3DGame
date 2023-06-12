/*
Author: Carson L
Date: 5/24/2023
Desc: Rotates objects toward the camera
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardGUI : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
    }
}
