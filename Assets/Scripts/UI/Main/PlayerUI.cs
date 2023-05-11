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

    public TMP_Text charge;

    public PlayerController player;
    public PlayerControls playerC;
    public Transform[] points;
    public AnimationCurve c;
    public float moveTime;
    public int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
        playerC = player.GetComponent<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        weaponName.text = "Current Weapon: " + player.weaponData.Name.ToString();
        ammoCount.text = player.weaponData.currentBulletCount.ToString() + " / " + player.weaponData.bulletCount.ToString();
        storedWeapons.text = "Stored Weapons: \n";

        charge.text = player.heldTime.ToString();

        // Weapon Storage UI
        if (player.weaponItems.Count > 0)
        {
            for (int i = 0; i < player.weaponItems.Count; i++)
            {
                storedWeapons.text += player.weaponItems[i].weaponData.Name + "\n";
            }
        }

        if ((player.weaponData.isReloading && player.weaponData.isEmpty) || (player.weaponData.currentBulletCount <= 0 && player.weaponData.isReloading))
        {
            ammoCount.text = "Reloading!";
        }

        StartCoroutine(moveTowards(points[index], moveTime));
    }

    private void FixedUpdate()
    {
        index = playerC.isRunning ? 1 : 0;
    }

    IEnumerator moveTowards(Transform obj, float speed)
    {
        float fraction = 0;
        float time = 0;

        do
        {
            fraction = time / speed;

            time += Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, obj.position, fraction);
            transform.rotation = Quaternion.Lerp(transform.rotation, obj.rotation, fraction);

            yield return new WaitForEndOfFrame();
        }
        while (Vector3.Distance(transform.position, obj.position) <= 0);


    }
}
