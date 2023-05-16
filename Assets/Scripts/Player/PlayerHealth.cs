using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    [Header("Health Bar")]
    [SerializeField] TMP_Text healthText;
    [SerializeField] Image effectBar;
    [SerializeField] Image healthBar;
    [SerializeField] Image hitEffect;

    [Header("Death Screen")]
    [SerializeField] GameObject deathScreen;

    [Header("Weapon Index")]
    [SerializeField] Image[] weapons;

    [Header("Debugging")]
    [SerializeField] float maxHealth;
    Color c;

    PlayerCamera cam;
    PlayerController p;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<PlayerCamera>().GetComponent<PlayerCamera>();
        p = FindObjectOfType<PlayerController>().GetComponent<PlayerController>();

        maxHealth = health;

        // Bar color
        c = Color.Lerp(Color.red, Color.green, (health / maxHealth));

        StartCoroutine(UpdateHealthBar());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R) || (deathScreen.activeSelf && Input.GetMouseButtonDown(0))) ResetLevel();

        healthText.text = health.ToString();

        hitEffect.color = new Color(1f, 0f, 0f, Mathf.Lerp(hitEffect.color.a, 0, 2f * Time.deltaTime));

        // Effect bar Fill
        effectBar.fillAmount = Mathf.Lerp(effectBar.fillAmount, (health / maxHealth), 5f * Time.deltaTime);

        // Bar color
        c = (isInvulnerable) ? Color.cyan : Color.Lerp(Color.red, Color.green, (health / maxHealth));
        
        // UI weapon show
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = true;
            weapons[i].color = Color.black;
            weapons[i].color = new Color(weapons[i].color.r, weapons[i].color.g, weapons[i].color.b, .75f);

            if (i == p.selectedIndex) weapons[i].color = Color.cyan;

            if (i > p.weaponItems.Count - 1) weapons[i].enabled = false;
        }
    }

    public override void OnDeath()
    {
        StartCoroutine(cam.ShakeCamera(4f, .25f));

        deathScreen.SetActive(true);

        health = 0;
        Time.timeScale = 0f;
        //throw new System.NotImplementedException();
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public override void Hit(int damage, Vector3 pos, Vector3 knockbackForce)
    {
        if (isInvulnerable) return;
        health = health - damage;

        StartCoroutine(UpdateHealthBar());

        if (damage > 0) hitEffect.color = new Color(1f, 0f, 0f, (health <= maxHealth / 4) ? 0.25f : 0.1f);

        if (health <= 0)
        {
            OnDeath();
            isInvulnerable = true;
            return;
        }

        StartCoroutine(cam.ShakeCamera(damage <= 0 ? knockbackForce.magnitude : Mathf.Clamp(damage, 0, 3), .25f));

        StartCoroutine(Knockback(pos, knockbackForce));

        if (hasInvulnerability) StartCoroutine(Invulnerable(invulnerabilitySeconds));

        //throw new System.NotImplementedException();
    }

    public void GainHP(int hp)
    {
        health = health + hp;

        if (health > maxHealth)
            health = Mathf.CeilToInt(maxHealth);

        StartCoroutine(UpdateHealthBar());
    }

    IEnumerator UpdateHealthBar()
    {
        // Bar fill
        healthBar.fillAmount = health / maxHealth;

        healthBar.color = c;

        yield return null;
    }
}
