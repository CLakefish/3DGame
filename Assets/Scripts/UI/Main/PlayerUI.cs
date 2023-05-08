using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text weaponName,
                    ammoCount,
                    storedWeapons;

    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        weaponName.text = "Current Weapon: " + player.weaponData.Name.ToString();
        ammoCount.text = player.weaponData.currentBulletCount.ToString() + " / " + player.weaponData.bulletCount.ToString();
        storedWeapons.text = "Stored Weapons: \n";

        // Weapon Storage UI
        if (player.weaponItems.Count > 0)
        {
            for (int i = 0; i < player.weaponItems.Count; i++)
            {
                storedWeapons.text += player.weaponItems[i].weaponData.Name + "\n";
            }
        }

        if (player.weaponData.isReloading && player.weaponData.isEmpty)
        {
            ammoCount.text = "Reloading!";
        }
    }
}
