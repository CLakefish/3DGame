using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public delegate void OnDeath();
    public OnDeath onDeath;
    
    public int health;
    public int maxHealth;
    public bool isInvulnerable;
    public void Start()
    {
        maxHealth = health;
    }

    public void ChangeHealth(int healthChange)
    {
        health += healthChange;

        if (health > maxHealth)
        {
            health = Mathf.CeilToInt(maxHealth);
        }
    }

    public IEnumerator Invulnerable(float seconds)
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(seconds);

        isInvulnerable = false;
    }
}
