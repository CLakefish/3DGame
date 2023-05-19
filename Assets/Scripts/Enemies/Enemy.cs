using System.Collections;
using System.Collections.Generic;
using UnityEngine; using UnityEngine.AI;

// The class can be found in the player script/namespace
// It contains 2 coroutines which handle invulnerability and knockback, both with slight delay and slight timing delay
// It also contains a bool for if you are invulerable and how much hp you have

public enum EnemyType
{
    Melee,
}

public class Enemy : MonoBehaviour, IHealth
{
    public bool isInvulnerable { get; set; }
    public int health { get; set; }
    public int maxHealth { get; set; }
    public NavMeshAgent navigation;
    public PlayerControls playerControls;
    public Vector3 playerPos;
    public float knockBackTime;
    public bool knockBack = false;
    public bool grounded = false;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        playerControls = FindObjectOfType<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(new Ray(rb.transform.position, Vector3.down), 2f, playerControls.groundLayer);

        if (!knockBack && grounded) FindObj();
        if (!knockBack && grounded && navigation.isOnNavMesh) navigation.SetDestination(playerPos);

        if (!navigation.pathPending && navigation.remainingDistance <= navigation.stoppingDistance && (!navigation.hasPath || navigation.velocity.sqrMagnitude == 0f) && Vector3.Distance(rb.transform.position, playerControls.rb.transform.position) <= 3.5f)
        {
            playerControls.GetComponent<PlayerHealth>().Hit(Random.Range(1, 5));
            // todo: add knockback here using rb.transform.position, 10f
        }

        Debug.Log(knockBack);
    }

    // This is the death effect, which can be altered later
    public void OnDeath()
    {
        Destroy(gameObject);
        //throw new System.NotImplementedException();
    }

    // When the enemy is hit it will do this
    public void Hit(int damage)
    {
        if (isInvulnerable)
        {
            return;
        }
        health -= damage;

        if (health <= 0)
        {
            OnDeath();
            isInvulnerable = true;
            return;
        }

        // if (hasInvulnerability) StartCoroutine(Invulnerable(invulnerabilitySeconds));

        //throw new System.NotImplementedException();
    }

    void Knockback(float knockbackForce, Vector3 hitPoint)
    {
        if (!knockBack)
        {
            return;
        }
        // StartCoroutine(Knockback(pos, knockbackForce));
        StartCoroutine(NavKnockback());
    }

    void FindObj()
    {
        playerPos = playerControls.rb.transform.position;
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
