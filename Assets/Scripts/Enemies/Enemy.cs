using System.Collections;
using System.Collections.Generic;
using UnityEngine; using UnityEngine.AI;
using Player;

// The class can be found in the player script/namespace
// It contains 2 coroutines which handle invulnerability and knockback, both with slight delay and slight timing delay
// It also contains a bool for if you are invulerable and how much hp you have

public enum EnemyType
{
    Melee,
}

public class Enemy : Health
{
    public NavMeshAgent navigation;
    public PlayerControls player;
    public Vector3 playerPos;
    public float knockBackTime;
    public bool knockBack = false;
    public bool grounded = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(new Ray(rb.transform.position, Vector3.down), 2f, player.groundLayer);

        if (!knockBack && grounded) FindObj();
        if (!knockBack && grounded && navigation.isOnNavMesh) navigation.SetDestination(playerPos);

        if (Vector3.Distance(rb.transform.position, player.rb.transform.position) <= 2)
        {
            player.GetComponent<PlayerHealth>().Hit(Random.Range(1, 5), rb.transform.position, 10f);
        }

        Debug.Log(knockBack);
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

        if (!knockBack)
        {
            StartCoroutine(Knockback(pos, knockbackForce));
            StartCoroutine(NavKnockback());
        }

        if (hasInvulnerability) StartCoroutine(Invulnerable(invulnerabilitySeconds));

        //throw new System.NotImplementedException();
    }

    void FindObj()
    {
        playerPos = player.rb.transform.position;
    }

    IEnumerator NavKnockback()
    {
        navigation.enabled = false;
        knockBack = true;
        yield return new WaitForSeconds(1f);

        while (knockBack == true)
        {
            if (grounded || navigation.isOnNavMesh)
            {
                knockBack = false;
                navigation.enabled = true;
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
