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

            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();

            if (player == null) return;

            player.weaponItems.Add(weapon);
            player.selectedIndex++;
            weapon.weaponData.isEmpty = false;
            player.weaponData = player.weaponItems[player.selectedIndex].weaponData;

            Destroy(gameObject);
        }
    }
}
