using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Camera overlayCamera;
    [Header("Death Screen")]
    [SerializeField] GameObject deathScreen;
    [SerializeField] TMP_Text weaponName,
                    ammoCount,
                    chargeCount;

    Vector3 offset;

    [Header("Health Bar")]
    [SerializeField] TMP_Text healthText;
    [SerializeField] Image effectBar;
    [SerializeField] Image healthBar;
    [SerializeField] Image hitEffect;
    [Header("Weapon Index")]
    [SerializeField] Image[] weapons;

    PlayerWeaponController playerWeaponController;
    PlayerMovementController playerMovementController;
    HealthController healthController;

    [SerializeField] private float hudLerpTime;
    [SerializeField] private Transform hudTransform;
    private PlayerCamera cam;

    private Vector3 hudVel;
    private Vector3 hudRotVel;
    private Vector3 rotVel;
    Color healthBarColor;

    // Start is called before the first frame update
    void Start()
    {
        offset = hudTransform.localPosition;
        playerWeaponController = FindObjectOfType<PlayerWeaponController>();
        playerMovementController = playerWeaponController.GetComponent<PlayerMovementController>();
        healthController = playerWeaponController.GetComponent<HealthController>();

        cam = FindObjectOfType<PlayerCamera>();

        // Bar color
        healthBarColor = Color.Lerp(Color.red, Color.green, (float)healthController.health / healthController.maxHealth);

        Camera.main.GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(overlayCamera);
    }

    // Update is called once per frame
    void Update()
    {
        WeaponData currentWeapon = playerWeaponController.GetCurrentWeapon();
        weaponName.text = currentWeapon.weaponItem.Name.ToString();
        ammoCount.text = currentWeapon.currentBulletCount.ToString();

        // TODO: fix this if-statement hell
        if ((currentWeapon.isReloading && currentWeapon.isEmpty) || (currentWeapon.currentBulletCount <= 0))
        {
            ammoCount.text = "Reloading!";
        }
        if ((currentWeapon.weaponItem.bulletType == Player.BulletType.Charge || currentWeapon.weaponItem.bulletType == Player.BulletType.ChargeBounce))
        {
            if (currentWeapon.isReloading && currentWeapon.isShooting)
            {
                chargeCount.text = (currentWeapon.currentBulletCount.ToString()) + " | " + (playerWeaponController.heldTime == 1.99f ? "Firing!" : "Charge : " + playerWeaponController.heldTime.ToString((playerWeaponController.heldTime < 1 ? "0.00" : "#.00")));

                chargeCount.color = Color.Lerp(Color.white, healthBarColor, (playerWeaponController.heldTime / 2));
            }
            else
            {
                chargeCount.text = currentWeapon.currentBulletCount.ToString() + " | Charge : 0.00";
            }
        }
        else 
        {
            chargeCount.color = Color.white;
        }

        if (healthController.isInvulnerable)
        {
            hitEffect.color = new Color(1f, 0f, 0f, (healthController.health <= healthController.maxHealth / 4) ? 0.25f : 0.1f);
        }
        hitEffect.color = new Color(1f, 0f, 0f, Mathf.Lerp(hitEffect.color.a, 0, 2f * Time.deltaTime));

        // UI weapon show
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = true;
            weapons[i].color = Color.black;
            weapons[i].color = new Color(weapons[i].color.r, weapons[i].color.g, weapons[i].color.b, .75f);

            if (i == playerWeaponController.selectedIndex)
            {
                weapons[i].color = Color.cyan;
            }

            if (i > playerWeaponController.weaponItems.Count - 1)
            {
                weapons[i].enabled = false;
            }
        }

        UpdateHealthBar();
    }

    void LateUpdate()
    {
        overlayCamera.transform.position = playerMovementController.rb.transform.position;
        hudTransform.localPosition = Vector3.SmoothDamp(hudTransform.localPosition, playerMovementController.rb.transform.position + offset, ref hudVel, hudLerpTime);

        overlayCamera.transform.rotation = Camera.main.transform.rotation;
        /*
        if we want the rotation smoothing, change some code. the problem is that i dont know how to recreate smooth damp for rotation
        */
        transform.rotation = Camera.main.transform.rotation;
    }
    void OnDeath()
    {
        deathScreen.SetActive(true);
    }

    void UpdateHealthBar()
    {
        // Bar fill
        healthBar.fillAmount = (float)healthController.health / healthController.maxHealth;

        healthBar.color = healthBarColor;

        healthBarColor = healthController.isInvulnerable ? Color.cyan : Color.Lerp(Color.red, Color.green, (float)healthController.health / healthController.maxHealth);

        healthText.text = healthController.health.ToString();

        // Effect bar Fill
        effectBar.fillAmount = Mathf.Lerp(effectBar.fillAmount, (float)healthController.health / healthController.maxHealth, 5 * Time.deltaTime);
    }

}
