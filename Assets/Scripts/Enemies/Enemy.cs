using System.Collections;
using System.Collections.Generic;
using UnityEngine; using UnityEngine.AI;
using Player;

// The class can be found in the player script/namespace
// It contains 2 coroutines which handle invulnerability and knockback, both with slight delay and slight timing delay
// It also contains a bool for if you are invulerable and how much hp you have

public enum EnemyType
{
    Punch,
    Shoot,
}

public class Enemy : Health
{
    [Header("Get Components")]
    public NavMeshAgent navigation;
    public PlayerControls player;

    [Header("Finding")]
    public Vector3 playerPos;
    public float knockBackTime;
    public bool knockBack = false;
    public bool grounded = false;

    [Header("Attack Type")]
    [SerializeField] EnemyType attackType;
    [SerializeField] int damage;
    [SerializeField] float shootInbetweenTime;
    float prevShootTime;

    [Header("Trails")]
    [SerializeField] TrailRenderer TrailRenderer;
    [SerializeField] float trailSpeed;
    [SerializeField] float bulletSpread;
    [SerializeField] int pierce;

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
            
        float distance = Vector3.Distance(rb.transform.position, player.rb.transform.position);
        bool run = false;

        if (distance <= navigation.stoppingDistance || attackType == EnemyType.Shoot)
        {
            switch (attackType)
            {
                case (EnemyType.Punch):
                    player.GetComponent<PlayerHealth>().Hit(damage, rb.transform.position, 10f);

                    break;

                case (EnemyType.Shoot):
                    if (distance >= navigation.stoppingDistance) ShootProjectile();
                    else run = true;
                    break;
            }
        }

        if (!knockBack && grounded) FindObj();
        if (!knockBack && grounded) navigation.SetDestination((run) ? -playerPos : playerPos);

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
        if (Vector3.Distance(rb.transform.position, player.rb.transform.position) >= 10 || Vector3.Distance(rb.transform.position, player.rb.transform.position) == navigation.stoppingDistance) playerPos = rb.transform.position;
        playerPos = player.rb.transform.position;
    }
    void ShootProjectile()
    {
        if (Time.time > shootInbetweenTime + prevShootTime + Random.Range(-0.1f, 0.1f))
        {
            prevShootTime = Time.time;

            ProjectileCast();
        }
        else return;
    }

    void ProjectileCast()
    {
        // Trail rendering
        Rigidbody trail = Instantiate(TrailRenderer, rb.transform.position, player.cam.transform.rotation).GetComponent<Rigidbody>();
        EnemyProjectile p = trail.GetComponent<EnemyProjectile>();

        p.pierce = pierce;
        p.damage = damage;

        while (trail != null)
        {
            trail.AddForce((player.rb.transform.position - rb.transform.position).normalized * trailSpeed, ForceMode.VelocityChange);
            return;
        }
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
