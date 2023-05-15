using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

// The class can be found in the player script/namespace
// It contains 2 coroutines which handle invulnerability and knockback, both with slight delay and slight timing delay
// It also contains a bool for if you are invulerable and how much hp you have
public class Enemy : Health
{
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity += new Vector3(0f, -100f * Time.deltaTime, 0f);
    }

    // This is the death effect, which can be altered later
    public override void OnDeath()
    {
        Destroy(gameObject);
        //throw new System.NotImplementedException();
    }

    // When the enemy is hit it will do this
    public override void Hit(int damage, Vector3 pos, Vector3 knockbackForce)
    {
        if (isInvulnerable) return;
        health = health - damage;

        if (health <= 0)
        {
            OnDeath();
            isInvulnerable = true;
            return;
        }

        rb = gameObject.GetComponent<Rigidbody>();

        StartCoroutine(Knockback(pos, knockbackForce));

        if (hasInvulnerability) StartCoroutine(Invulnerable(invulnerabilitySeconds));

        //throw new System.NotImplementedException();
    }
}
