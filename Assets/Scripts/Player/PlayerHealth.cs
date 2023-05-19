using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IHealth
{
    [Space()]
    [Header("Health")]
    [SerializeField] public float invulnerabilitySeconds;
    [SerializeField] public bool hasInvulnerability = false;

    public Rigidbody rb;

    // we are changing this later once i understand interfaces better
    public bool isInvulnerable { get; set; }
    [SerializeField] public int health { get; set; }
    public int maxHealth { get; set; }
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

    public void OnDeath()
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

    public void Hit(int damage)
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

        if (hasInvulnerability)
        {
            StartCoroutine(Invulnerable(invulnerabilitySeconds));
        }
        //throw new System.NotImplementedException();
    }

    public IEnumerator Invulnerable(float seconds)
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(seconds);

        isInvulnerable = false;
    }

    public IEnumerator Knockback(Vector3 pos, float knockbackForce, bool stopForce = true)
    {
        pos = (pos - rb.transform.position).normalized;

        rb.velocity = (!stopForce) ? rb.velocity : new Vector3(0f, 0f, 0f);

        yield return new WaitForSeconds(0.05f);
        rb.AddForce(new Vector3(-pos.x, pos.y, -pos.z) * knockbackForce, ForceMode.Impulse);
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
