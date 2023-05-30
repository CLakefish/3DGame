using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public delegate void OnDeath();
    public OnDeath onDeath;
    
    public int health;
    public int maxHealth;
    float invulnerabilityTimer;
    public float invulnerabilitySeconds;
    public bool isInvulnerable;
    void Start()
    {
        maxHealth = health;
    }
    void Update()
    {
        invulnerabilityTimer = Mathf.MoveTowards(invulnerabilityTimer, 0, Time.deltaTime);
    }

    public void ChangeHealth(int healthChange)
    {
        if (invulnerabilityTimer > 0)
        {
            return;
        }
        
        health += healthChange;

        if (healthChange < 0)
        {
            invulnerabilityTimer = invulnerabilitySeconds;
        }

        if (health > maxHealth)
        {
            health = Mathf.CeilToInt(maxHealth);
        }
        if (health <= 0)
        {
            health = 0;
            onDeath.Invoke();
        }
    }

    public IEnumerator Invulnerable(float seconds)
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(seconds);

        isInvulnerable = false;
    }
}
