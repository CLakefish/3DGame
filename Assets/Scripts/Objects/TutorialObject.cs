/*
Author: Carson L
Date: 6/2/2023
Desc: opens a door when the player hits a trigger
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponentInParent<PlayerWeaponController>().enabled = true;
            FindObjectOfType<DoorHandler>().Open(0);
            Destroy(gameObject);
        }
    }
}
