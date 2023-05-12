using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text weaponName,
                    ammoCount,
                    storedWeapons;

    //public TMP_Text charge;

    public PlayerController player;
    public PlayerControls playerC;

    [SerializeField] private float hudLerpTime;
    [SerializeField] private Transform hudTransform;
    private PlayerCamera cam;

    private Vector3 hudVel;
    private Vector3 rotVel;
    [SerializeField] float maxDist = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
        playerC = player.GetComponent<PlayerControls>();

        cam = FindObjectOfType<PlayerCamera>().GetComponent<PlayerCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        weaponName.text = player.weaponData.Name.ToString();
        ammoCount.text = player.weaponData.currentBulletCount.ToString() + " / " + player.weaponData.bulletCount.ToString();
        //storedWeapons.text = "Stored Weapons: \n";

        //charge.text = player.heldTime.ToString();

        /*// Weapon Storage UI
        if (player.weaponItems.Count > 0)
        {
            for (int i = 0; i < player.weaponItems.Count; i++)
            {
                storedWeapons.text += player.weaponItems[i].weaponData.Name + "\n";
            }
        }*/

        if ((player.weaponData.isReloading && player.weaponData.isEmpty) || (player.weaponData.currentBulletCount <= 0 && player.weaponData.isReloading))
        {
            ammoCount.text = "Reloading!";
        }

        if (Vector3.Max(hudTransform.position, transform.position + Vector3.ClampMagnitude(hudTransform.position - transform.position, maxDist)) != hudTransform.position) hudTransform.position = Vector3.SmoothDamp(hudTransform.position, transform.position, ref hudVel, hudLerpTime / 10f);
        else hudTransform.position = Vector3.SmoothDamp(hudTransform.position, transform.position, ref hudVel, hudLerpTime);

        hudTransform.position = transform.position + Vector3.ClampMagnitude(hudTransform.position - transform.position, maxDist);
        hudTransform.rotation = transform.rotation;
    }

    }
