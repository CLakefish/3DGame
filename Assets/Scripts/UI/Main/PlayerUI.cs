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

    PlayerController playerController;
    PlayerControls playerControls;
    PlayerHealth playerHealth;

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
        playerController = FindObjectOfType<PlayerController>();
        playerControls = playerController.GetComponent<PlayerControls>();
        playerHealth = FindObjectOfType<PlayerHealth>();

        cam = FindObjectOfType<PlayerCamera>();

        // Bar color
        healthBarColor = Color.Lerp(Color.red, Color.green, (float)playerHealth.health / playerHealth.maxHealth);

        Camera.main.GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(overlayCamera);
    }

    // Update is called once per frame
    void Update()
    {
        weaponName.text = playerController.weaponData.Name.ToString();
        ammoCount.text = playerController.weaponData.currentBulletCount.ToString();

        if ((playerController.weaponData.isReloading && playerController.weaponData.isEmpty) || (playerController.weaponData.currentBulletCount <= 0))
        {
            ammoCount.text = "Reloading!";
        }
        if ((playerController.weaponData.bulletType == Player.BulletType.Charge || playerController.weaponData.bulletType == Player.BulletType.ChargeBounce))
        {
            if (playerController.weaponData.isReloading && playerController.weaponData.isShooting)
            {
                chargeCount.text = (playerController.weaponData.currentBulletCount.ToString()) + " | " + (playerController.heldTime == 1.99f ? "Firing!" : "Charge : " + playerController.heldTime.ToString((playerController.heldTime < 1 ? "0.00" : "#.00")));

                chargeCount.color = Color.Lerp(Color.white, healthBarColor, (playerController.heldTime / 2));
            }
            else
            {
                chargeCount.text = playerController.weaponData.currentBulletCount.ToString() + " | Charge : 0.00";
            }
        }
        else 
        {
            chargeCount.color = Color.white;
        }

        if (playerControls.GetComponent<PlayerHealth>().isInvulnerable) hitEffect.color = new Color(1f, 0f, 0f, (playerHealth.health <= playerHealth.maxHealth / 4) ? 0.25f : 0.1f);

        hitEffect.color = new Color(1f, 0f, 0f, Mathf.Lerp(hitEffect.color.a, 0, 2f * Time.deltaTime));

        // UI weapon show
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = true;
            weapons[i].color = Color.black;
            weapons[i].color = new Color(weapons[i].color.r, weapons[i].color.g, weapons[i].color.b, .75f);

            if (i == playerController.selectedIndex) weapons[i].color = Color.cyan;

            if (i > playerController.weaponItems.Count - 1) weapons[i].enabled = false;
        }

        UpdateHealthBar();
    }

    void LateUpdate()
    {
        overlayCamera.transform.position = playerControls.rb.transform.position;
        hudTransform.localPosition = Vector3.SmoothDamp(hudTransform.localPosition, playerControls.rb.transform.position + offset, ref hudVel, hudLerpTime);

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
        healthBar.fillAmount = (float)playerHealth.health / playerHealth.maxHealth;

        healthBar.color = healthBarColor;

        healthBarColor = playerHealth.isInvulnerable ? Color.cyan : Color.Lerp(Color.red, Color.green, (float)playerHealth.health / playerHealth.maxHealth);

        healthText.text = playerHealth.health.ToString();

        // Effect bar Fill
        effectBar.fillAmount = Mathf.Lerp(effectBar.fillAmount, (float)playerHealth.health / playerHealth.maxHealth, 5 * Time.deltaTime);
    }

}
