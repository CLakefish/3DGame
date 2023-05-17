using System.Collections;
using System.Collections.Generic;
using UnityEngine; using UnityEngine.AI;
using Player;

// The class can be found in the player script/namespace
// It contains 2 coroutines which handle invulnerability and knockback, both with slight delay and slight timing delay
// It also contains a bool for if you are invulerable and how much hp you have
public class Enemy : Health
{
    public NavMeshAgent navigation;
    public PlayerControls player;
    public Vector3 playerPos;
    public float knockBackTime;
    bool knockBack;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        FindObj();
        navigation.SetDestination(playerPos);
    }

    // This is the death effect, which can be altered later
    public override void OnDeath()
    {
        Destroy(gameObject);
        //throw new System.NotImplementedException();
    }

    // When the enemy is hit it will do this
    public override void Hit(int damage, Vector3 pos, float knockbackForce)
    {
        if (isInvulnerable) return;
        health = health - damage;

        if (health <= 0)
        {
            OnDeath();
            isInvulnerable = true;
            return;
        }

        if (!knockBack) StartCoroutine(NavKnockback(pos, knockbackForce));

        if (hasInvulnerability) StartCoroutine(Invulnerable(invulnerabilitySeconds));

        //throw new System.NotImplementedException();
    }

    void FindObj()
    {
        playerPos = player.rb.transform.position;
    }

    IEnumerator NavKnockback(Vector3 pos, float knockbackForce)
    {
        knockBack = true;
        float time = Time.time;

        while (Time.time <= time + knockBackTime)
        {
            navigation.velocity = -(pos - navigation.transform.position).normalized * knockbackForce;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.2f);

        knockBack = false;
        navigation.updatePosition = true;
    }
}
