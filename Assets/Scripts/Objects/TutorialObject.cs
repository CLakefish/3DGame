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
