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
                    chargeCount;

    //public TMP_Text charge;

    public PlayerController player;
    public PlayerControls playerC;

    [SerializeField] private float hudLerpTime;
    [SerializeField] private Transform hudTransform;
    private PlayerCamera cam;

    private Vector3 hudVel;
    private Vector3 rotVel;
    [SerializeField] float maxDist = 0.75f;
    public Color c;

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
        ammoCount.text = player.weaponData.currentBulletCount.ToString();

        if ((player.weaponData.isReloading && player.weaponData.isEmpty) || (player.weaponData.currentBulletCount <= 0))
        {
            ammoCount.text = "Reloading!";
        }
        if ((player.weaponData.bulletType == Player.BulletType.Charge || player.weaponData.bulletType == Player.BulletType.ChargeBounce))
        {
            if (player.weaponData.isReloading && player.weaponData.isShooting)
            {
                chargeCount.text = (player.heldTime == 1.99f ? "Firing!" : "Charge : " + player.heldTime.ToString((player.heldTime < 1 ? "0.00" : "#.00")));

                chargeCount.color = Color.Lerp(Color.white, c, (player.heldTime / 2));
            }
            else
            {
                chargeCount.text = "Charge : 0.00";
            }
        }
        else 
        {
            chargeCount.color = Color.white;
            chargeCount.text = player.weaponData.Name;
        }
    }

    private void LateUpdate()
    {
        //hudTransform.position = transform.position + Vector3.ClampMagnitude(hudTransform.position - transform.position, maxDist);
        hudTransform.position = Vector3.SmoothDamp(hudTransform.position, transform.position, ref hudVel, hudLerpTime);
        hudTransform.rotation = transform.rotation;
    }

}
