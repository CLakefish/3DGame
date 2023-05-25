using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class WeaponObject : MonoBehaviour
{
    public WeaponItem weapon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Picked up!");

            PlayerWeaponController playerWeaponController = other.gameObject.GetComponentInParent<PlayerWeaponController>();
            if (playerWeaponController == null)
            {
                return;
            }
            playerWeaponController.AddWeapon(weapon);

            Destroy(gameObject);
        }
    }
}
