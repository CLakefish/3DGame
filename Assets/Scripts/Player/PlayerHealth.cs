using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    [SerializeField] public int maxHealth;

    PlayerCamera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<PlayerCamera>();

        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R) || (health <= 0 && Input.GetMouseButtonDown(0)))
        {
            ResetLevel();
        }
    }

    public override void OnDeath()
    {
        StartCoroutine(cam.ShakeCamera(4f, .25f));

        health = 0;
        Time.timeScale = 0f;
        //throw new System.NotImplementedException();
    }

    public void ResetLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public override void Hit(int damage, Vector3 pos, float knockbackForce)
    {
        if (isInvulnerable || damage == 0)
        {
            return;
        } 
        health = health - damage;

        if (health <= 0)
        {
            OnDeath();
            isInvulnerable = true;
            return;
        }

        StartCoroutine(cam.ShakeCamera(damage <= 0 ? knockbackForce : Mathf.Clamp(damage, 0, 3), .25f));

        StartCoroutine(Knockback(pos, knockbackForce));

        if (hasInvulnerability)
        {
            StartCoroutine(Invulnerable(invulnerabilitySeconds));
        }
        //throw new System.NotImplementedException();
    }

    public void GainHP(int hp)
    {
        health = health + hp;

        if (health > maxHealth)
        {
            health = Mathf.CeilToInt(maxHealth);
        }
    }
}
