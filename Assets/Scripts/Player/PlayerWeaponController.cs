using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.AI;

public class PlayerWeaponController : MonoBehaviour
{
    #region Parameters

    [Header("Assignables")]
    internal Rigidbody rb;
    internal PlayerCamera playerCamera;

    [Header("Weapon Information")]
    public List<WeaponItem> weaponItems;
    [Header("Debugging")]
    public WeaponData weaponData;
    public int selectedIndex = 0;

    [Header("Shootable Layers")]
    public LayerMask layers;
    public GameObject firePos;
    float previousFireTime;
    public float heldTime;
    public bool canShoot = true;
    bool isFiring;
    [Header("Explosion Prefab")]
    [SerializeField] GameObject explosion;

    #endregion

    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerCamera = FindObjectOfType<PlayerCamera>();

        weaponData = weaponItems[0].weaponData;
        weaponData.currentBulletCount = weaponData.bulletCount;

        weaponData.isReloading = false;
        weaponData.isEmpty = false;
    }

    void Update()
    {

        // Weapon Change 
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (weaponItems.Count <= 1) return;

            if (Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")) == 1)
            {


                weaponItems[selectedIndex].weaponData = weaponData;

                if (selectedIndex < weaponItems.Count - 1) selectedIndex++;
                else selectedIndex = 0;

                heldTime = 0f;

                weaponData = weaponItems[selectedIndex].weaponData;
                weaponData.isEmpty = false;
                weaponData.isReloading = false;

                StopCoroutine(ItemSwitchPause());
                StartCoroutine(ItemSwitchPause());
            }
            else
            {
                weaponItems[selectedIndex].weaponData = weaponData;

                if (selectedIndex > 0) selectedIndex--;
                else selectedIndex = weaponItems.Count - 1;

                heldTime = 0f;

                weaponData = weaponItems[selectedIndex].weaponData;
                weaponData.isEmpty = false;
                weaponData.isReloading = false;

                StopCoroutine(ItemSwitchPause());
                StartCoroutine(ItemSwitchPause());
            }
        }

        isFiring = Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0);

        if (Input.GetKeyDown(KeyCode.R) && !isFiring) StartCoroutine(Reload());

        if (Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0) && (weaponData.bulletType == BulletType.Charge || weaponData.bulletType == BulletType.ChargeBounce) && canShoot)
        {
            heldTime += Time.deltaTime;
        }
        else
        {
            heldTime = 0;
        }

        // Shooting 
        if (isFiring && weaponData.currentBulletCount > 0 && !weaponData.isReloading) ShootObj();
        else if (weaponData.currentBulletCount <= 0 && !weaponData.isEmpty) StartCoroutine(Reload());
    }


    #region Projectile Firing 
    IEnumerator Reload()
    {
        weaponData.isEmpty = true;
        weaponData.isReloading = true;

        yield return new WaitForSeconds(weaponData.reloadTime);

        weaponData.currentBulletCount = weaponData.bulletCount;

        weaponData.isReloading = false;
        weaponData.isEmpty = false;
    }

    public void ShootObj()
    {
        if (!canShoot) return;

        // If the time is greater than the previous time + fireRate time then fire (Technically makes fireRate different but whatever)
        if (Time.time > previousFireTime + weaponData.timeBetweenShots && weaponData.currentBulletCount > 0)
        {
            previousFireTime = Time.time;
            weaponData.isShooting = true;

            // For different weapon types
            switch (weaponData.weapon)
            {
                case (Player.WeaponType.Single):

                    if (weaponData.bulletType == BulletType.Charge || weaponData.bulletType == BulletType.ChargeBounce)
                    {
                        if (!weaponData.isReloading)
                        {
                            weaponData.currentBulletCount--;
                            StopCoroutine(ChargedProjectileShotHandler());
                            StartCoroutine(ChargedProjectileShotHandler());
                        }
                        break;
                    }

                    weaponData.currentBulletCount--;
                    ProjectileShotHandler();
                    break;

                case (Player.WeaponType.Multi):

                    // Eventually this'll be better
                    for (int i = 0; i < (weaponData.bulletCount / 2); i++)
                    {
                        weaponData.currentBulletCount--;
                        ProjectileShotHandler();
                    }

                    break;
            }

            return;
        }
        // Return
        else
        {
            weaponData.isShooting = false;
            return;
        }
    }

    // Projectile Fire
    void ProjectileShotHandler()
    {
        if (weaponData.bulletType == BulletType.Follow)
        {
            GameObject obj = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Enemy"));

            if (obj == null) return;

            // Raycast to Mouse Input Position in world
            Ray ray = new Ray(rb.transform.position, (obj.transform.position - rb.transform.position).normalized); 

            // Trail rendering
            TrailRenderer trail = Instantiate(weaponData.bulletTrail, rb.transform.position, playerCamera.transform.rotation);

            // Raycast Hit reference
            RaycastHit raycast;

            Debug.Log(ray.direction);

            float r1 = Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread);
            float r2 = Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread);
            float r3 = Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread);

            // Randomization to the bullet direction
            ray.direction = new Vector3((r1 == 0 ? ray.direction.x : ray.direction.x + r1), (r2 == 0 ? ray.direction.y : ray.direction.y + r2), r3 == 0 ? ray.direction.z : ray.direction.z + r3);

            Debug.Log(ray.direction);

            bool hit = Physics.Raycast(ray, out raycast, 1000);

            // If it hits or does not hit based on a raycast, Mathf.Infinity can be changed soon enough.
            if (hit)
            {
                StartCoroutine(SpawnTrail(trail, raycast, raycast.point, raycast.normal, weaponData.bounceCount, 100, true, weaponData.bulletDamage, weaponData.trailSpeed, true));
            }
            else
            {
                StartCoroutine(SpawnTrail(trail, raycast, raycast.point, raycast.normal, weaponData.bounceCount, 100, false, weaponData.bulletDamage, weaponData.trailSpeed));
            }
        }
        else
        {
            // Raycast to Mouse Input Position in world
            Ray ray = new Ray(firePos.transform.position, firePos.transform.rotation * Vector3.forward);

            // Trail rendering
            TrailRenderer trail = Instantiate(weaponData.bulletTrail, firePos.transform.position, playerCamera.transform.rotation);

            // Raycast Hit reference
            RaycastHit raycast;

            // Randomization to the bullet direction
            ray.direction += new Vector3(Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread), Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread), Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread));

            // If it hits or does not hit based on a raycast, Mathf.Infinity can be changed soon enough.
            if (Physics.Raycast(ray, out raycast, 10000, layers))
            {
                StartCoroutine(SpawnTrail(trail, raycast, raycast.point, raycast.normal, weaponData.bounceCount, 100, true, weaponData.bulletDamage, weaponData.trailSpeed));
            }
            else
            {
                StartCoroutine(SpawnTrail(trail, raycast, ray.direction * 4000, new Vector3(0f, 0f, 0f), weaponData.bounceCount, 100, false, weaponData.bulletDamage, weaponData.trailSpeed));
            }
        }
    }

    IEnumerator ItemSwitchPause()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.4f);
        canShoot = true;
    }

    // Fix this
    IEnumerator ChargedProjectileShotHandler()
    {
        weaponData.isReloading = true;

        float bounceCount = weaponData.bounceCount;
        float tempStore = weaponData.trailSpeed;
        float attack = weaponData.bulletDamage;

        while (weaponData.isReloading)
        {
            if (!isFiring)
            {
                heldTime = 0;
                break;
            }
            else
            {
                // ?????????????????
                // todo: fix this
                if (heldTime > 0.5f)
                {
                    weaponData.bounceCount = bounceCount + 1;
                    weaponData.trailSpeed = tempStore + 35f;
                }
                if (heldTime > 1f)
                {
                    weaponData.bounceCount = bounceCount + 2;
                    weaponData.trailSpeed = tempStore * 2f;
                }
                if (heldTime > 2f)
                {
                    weaponData.bounceCount = bounceCount + 3;
                    weaponData.trailSpeed = tempStore * 3f;
                    heldTime = 0f;
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        weaponData.bounceCount = bounceCount;
        weaponData.trailSpeed = tempStore;

        ProjectileShotHandler();

        weaponData.isReloading = false;

        yield return null;
    }

    // Visualization
    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit raycast, Vector3 point, Vector3 normal, float bounceCount, float bounceDistance, bool hitObj, float damage, float speed, bool follow = false, GameObject positionUpdate = null)
    {
        Vector3 startPos = trail.transform.position;
        Vector3 dir = (follow) ? (rb.transform.position - point).normalized : (point - trail.transform.position).normalized;

        float d = Vector3.Distance(trail.transform.position, point);
        float startDist = d;

        while (d > 0)
        {
            if (trail == null) yield break;

            trail.transform.position = (follow) ? Vector3.Slerp(startPos, point, 1 - (d / startDist)) : Vector3.Lerp(startPos, point, 1 - (d / startDist));
            d -= Time.deltaTime * speed;

            yield return null;
        }

        if (trail != null) trail.transform.position = point;

        if (hitObj)
        {
            // Use Object Pooling here eventually 

            GameObject obj = Instantiate(weaponData.hitParticle, point, Quaternion.LookRotation(normal));

            if (raycast.collider == null) yield break;
            
            obj.transform.parent = raycast.collider.gameObject.transform;

            if (raycast.collider.gameObject.tag == "Enemy")
            {
                if (raycast.collider.TryGetComponent(out HealthController currentHealth))
                {
                    currentHealth.ChangeHealth(-weaponData.bulletDamage);
                }
                if (raycast.rigidbody)
                {
                    raycast.rigidbody.velocity += dir * weaponData.enemyKnockback;
                }
            }

            Destroy(obj, 2f);

            if (weaponData.explodeOnDeath && trail != null)
            {
                Explode(trail.transform.position, weaponData.enemyKnockback, weaponData.explosionRadius, weaponData.explosionStrength);
            }

            if ((weaponData.bulletType == BulletType.Bounce || weaponData.bulletType == BulletType.ChargeBounce) && bounceCount > 0)
            {
                Vector3 bounceDir = Vector3.Reflect(dir, normal);

                bounceCount--;

                // Recursion!
                if (Physics.Raycast(point, bounceDir, out RaycastHit h, Mathf.Infinity, layers))
                {
                    yield return new WaitForEndOfFrame();
                    yield return StartCoroutine(SpawnTrail(trail, h, h.point, h.normal, bounceCount, bounceDistance - Vector3.Distance(h.point, point), true, damage, speed));
                }
                else
                {
                    yield return new WaitForEndOfFrame();

                    yield return StartCoroutine(SpawnTrail(trail, h, bounceDir * 2000, Vector3.zero, 0, 0, false, damage, speed));
                }
            }
            else if (bounceCount <= 0 && trail != null)
            {
                if (weaponData.explodeOnDeath)
                {
                    Explode(trail.transform.position, weaponData.enemyKnockback, weaponData.explosionRadius, weaponData.explosionStrength);
                }
            }

            Destroy(trail, 0.1f);
        }
    }

    void Explode(Vector3 explosionPos, float knockbackValue, float explosionSize, float explosionStrength)
    {
        // For each possible collider, get the closest one then return if you're hitting it.
        foreach (var collider in Physics.OverlapSphere(explosionPos, explosionSize))
        {
            Vector3 difference = collider.transform.position - explosionPos;
            
            if (collider.TryGetComponent(out HealthController currentHealth))
            {
                if (currentHealth.transform != transform)
                {
                    currentHealth.ChangeHealth(-weaponData.bulletDamage);
                }
            }
            if (collider.TryGetComponent(out Rigidbody currentRigidbody))
            {
                float proximity = -((difference.magnitude / explosionSize) - 1);
                currentRigidbody.velocity += difference.normalized * proximity * explosionStrength;
                if (collider.TryGetComponent(out Enemy currentEnemy))
                {
                    currentEnemy.StartCoroutine(currentEnemy.NavKnockback());
                }
            }
        }

        Instantiate(explosion, explosionPos, Quaternion.identity);
    }

    #endregion

    GameObject GetClosestEnemy(GameObject[] enemies)
    {
        if (enemies == null) return null;

        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = rb.transform.position;

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 directionToTarget = enemies[i].gameObject.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = enemies[i];
            }
        }

        return bestTarget;
    }
}