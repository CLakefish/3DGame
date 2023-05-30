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

public class Enemy : MonoBehaviour
{
    public NavMeshAgent navigation;
    PlayerMovementController playerMovementController;
    PlayerHealth playerHealth;
    HealthController healthController;
    public SpriteRenderer sp;
    public Vector3 playerPos;
    public float knockBackTime;
    public bool knockBack = false;
    public bool grounded = false;
    public bool openOnDeath = false;
    public int index;
    Rigidbody rb;

    void OnEnable()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        playerMovementController = FindObjectOfType<PlayerMovementController>();
        playerHealth = playerMovementController.GetComponent<PlayerHealth>();
        healthController = GetComponent<HealthController>();

        healthController.onDeath += OnDeath;
    }
    void OnDisable()
    {
        healthController.onDeath -= OnDeath;
    }
    void Update()
    {
        grounded = Physics.Raycast(new Ray(rb.transform.position, Vector3.down), 2f, playerMovementController.groundLayer);

        if (!knockBack) sp.color = Color.Lerp(sp.color, Color.white, 5f * Time.deltaTime);

        if (!knockBack && grounded)
        {
            FindObj();
        }
        if (!knockBack && grounded && navigation.isOnNavMesh)
        {
            navigation.SetDestination(playerPos);
        }

        // maybe turn this into some sort of collision event?
        if (navigation.enabled && !navigation.pathPending && navigation.remainingDistance <= navigation.stoppingDistance && (!navigation.hasPath || navigation.velocity.sqrMagnitude == 0f) && Vector3.Distance(rb.transform.position, playerMovementController.rb.transform.position) <= 3.5f)
        {
            // player is hit
            playerHealth.healthController.ChangeHealth(-Random.Range(2, 5));
            Knockback(10, rb.transform.position);
        }
    }

    // This is the death effect, which can be altered later
    public void OnDeath()
    {
        if (openOnDeath) FindObjectOfType<DoorHandler>().Open(index);
        Destroy(gameObject);
    }

    void Knockback(float knockbackForce, Vector3 hitPoint)
    {
        if (!knockBack)
        {
            return;
        }
        StartCoroutine(KnockbackEnemy(knockbackForce, hitPoint));
        StartCoroutine(NavKnockback());
    }

    void FindObj()
    {
        playerPos = playerMovementController.rb.transform.position;
    }

    public IEnumerator KnockbackEnemy(float knockbackForce, Vector3 position)
    {
        Vector3 direction = (position - rb.transform.position).normalized;

        rb.velocity = new Vector3(0f, 0f, 0f);

        yield return new WaitForSeconds(0.0001f);

        rb.AddForce(new Vector3(-direction.x, direction.y, -direction.z) * knockbackForce, ForceMode.Impulse);
    }

    public IEnumerator NavKnockback()
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