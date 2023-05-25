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
    Collider coll;
    internal PlayerCamera playerCamera;

    [Header("Weapon Information")]
    public List<WeaponItem> weaponItems;
    [HideInInspector] public List<WeaponData> weapons;
    public int selectedIndex = 0;
    [Header("Debugging")]

    [Header("Shootable Layers")]
    public LayerMask layers;
    public GameObject firePos;
    float previousFireTime;
    public float heldTime;
    public bool canShoot = true;
    bool isFiring;
    [Header("Prefabs")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject explosion;

    #endregion

    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        coll = GetComponentInChildren<Collider>();
        playerCamera = FindObjectOfType<PlayerCamera>();

        foreach(WeaponItem currentWeapon in weaponItems)
        {
            AddWeapon(currentWeapon);
        }
    }

    void Update()
    {
        // Weapon Change
        int mouseWheelDelta = Input.GetAxis("Mouse ScrollWheel") != 0 ? (int)Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")) : 0;
        if (mouseWheelDelta != 0 && weaponItems.Count > 1)
        {

            selectedIndex += mouseWheelDelta;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, weapons.Count - 1);

            heldTime = 0;

            StopCoroutine(ItemSwitchPause());
            StartCoroutine(ItemSwitchPause());
        }

        if (Input.GetKeyDown(KeyCode.R) && !isFiring)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0) && (weapons[selectedIndex].weaponItem.bulletType == BulletType.Charge || weapons[selectedIndex].weaponItem.bulletType == BulletType.ChargeBounce) && canShoot)
        {
            heldTime += Time.deltaTime;
        }
        else
        {
            heldTime = 0;
        }

        // Shooting 
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        else if (weapons[selectedIndex].currentBulletCount <= 0 && !weapons[selectedIndex].isEmpty)
        {
            StartCoroutine(Reload());
        }
    }

    public void AddWeapon(WeaponItem weaponItem)
    {
        WeaponData weaponData = new();
        weaponData.weaponItem = weaponItem;
        weaponData.currentBulletCount = weaponItem.bulletCount;
        weapons.Add(weaponData);
    }


    #region Projectile Firing 
    IEnumerator Reload()
    {
        weapons[selectedIndex].isEmpty = true;
        weapons[selectedIndex].isReloading = true;

        yield return new WaitForSeconds(weapons[selectedIndex].weaponItem.reloadTime);

        weapons[selectedIndex].currentBulletCount = weapons[selectedIndex].weaponItem.bulletCount;

        weapons[selectedIndex].isReloading = false;
        weapons[selectedIndex].isEmpty = false;
    }

    public void ShootObj()
    {
        if (!canShoot) return;

        // If the time is greater than the previous time + fireRate time then fire (Technically makes fireRate different but whatever)
        if (Time.time > previousFireTime + weapons[selectedIndex].weaponItem.timeBetweenShots && weapons[selectedIndex].currentBulletCount > 0)
        {
            previousFireTime = Time.time;
            weapons[selectedIndex].isShooting = true;

            // For different weapon types
            switch (weapons[selectedIndex].weaponItem.weaponType)
            {
                case (Player.WeaponType.Single):

                    if (weapons[selectedIndex].weaponItem.bulletType == BulletType.Charge || weapons[selectedIndex].weaponItem.bulletType == BulletType.ChargeBounce)
                    {
                        if (!weapons[selectedIndex].isReloading)
                        {
                            weapons[selectedIndex].currentBulletCount--;
                            StopCoroutine(ChargedProjectileShotHandler());
                            StartCoroutine(ChargedProjectileShotHandler());
                        }
                        break;
                    }

                    weapons[selectedIndex].currentBulletCount--;
                    Shoot();
                    break;

                case (Player.WeaponType.Multi):

                    // Eventually this'll be better
                    for (int i = 0; i < (weapons[selectedIndex].weaponItem.bulletCount / 2); i++)
                    {
                        weapons[selectedIndex].currentBulletCount--;
                        Shoot();
                    }

                    break;
            }

            return;
        }
        // Return
        else
        {
            weapons[selectedIndex].isShooting = false;
            return;
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
        weapons[selectedIndex].isReloading = true;

        float bounceCount = weapons[selectedIndex].weaponItem.bounceCount;
        float tempStore = weapons[selectedIndex].weaponItem.trailSpeed;
        float attack = weapons[selectedIndex].weaponItem.bulletDamage;

        while (weapons[selectedIndex].isReloading)
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
                /*
                if (heldTime > 0.5f)
                {
                    weapons[selectedIndex].weaponItem.bounceCount = bounceCount + 1;
                    weapons[selectedIndex].weaponItem.trailSpeed = tempStore + 35f;
                }
                if (heldTime > 1f)
                {
                    weapons[selectedIndex].weaponItem.bounceCount = bounceCount + 2;
                    weapons[selectedIndex].weaponItem.trailSpeed = tempStore * 2f;
                }
                if (heldTime > 2f)
                {
                    weapons[selectedIndex].weaponItem.bounceCount = bounceCount + 3;
                    weapons[selectedIndex].weaponItem.trailSpeed = tempStore * 3f;
                    heldTime = 0f;
                    break;
                }
                */
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

/*
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

        if (trail != null)
        {
            trail.transform.position = point;
        }
        if (hitObj)
        {
            // Use Object Pooling here eventually 

            GameObject obj = Instantiate(weapons[selectedIndex].weaponItem.hitParticle, point, Quaternion.LookRotation(normal));

            if (raycast.collider == null) yield break;
            
            obj.transform.parent = raycast.collider.gameObject.transform;

            if (raycast.collider.gameObject.tag == "Enemy")
            {
                if (raycast.collider.TryGetComponent(out HealthController currentHealth))
                {
                    currentHealth.ChangeHealth(-weapons[selectedIndex].weaponItem.bulletDamage);
                }
                if (raycast.rigidbody)
                {
                    raycast.rigidbody.velocity += dir * weapons[selectedIndex].weaponItem.enemyKnockback;
                }
            }

            Destroy(obj, 2f);

            if (weapons[selectedIndex].weaponItem.explodeOnDeath && trail != null)
            {
                Explode(trail.transform.position, weapons[selectedIndex].weaponItem.enemyKnockback, weapons[selectedIndex].weaponItem.explosionRadius, weapons[selectedIndex].weaponItem.explosionStrength);
            }

            if ((weapons[selectedIndex].weaponItem.bulletType == BulletType.Bounce || weapons[selectedIndex].weaponItem.bulletType == BulletType.ChargeBounce) && bounceCount > 0)
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
                if (weapons[selectedIndex].weaponItem.explodeOnDeath)
                {
                    Explode(trail.transform.position, weapons[selectedIndex].weaponItem.enemyKnockback, weapons[selectedIndex].weaponItem.explosionRadius, weapons[selectedIndex].weaponItem.explosionStrength);
                }
            }

            Destroy(trail, 0.1f);
        }  
    }
*/

    // Visualization
    void Shoot()
    {
        Vector3 direction = playerCamera.cam.transform.forward;
        // Trail rendering
        TrailRenderer trail = Instantiate(weapons[selectedIndex].weaponItem.bulletTrail, rb.transform.position, playerCamera.transform.rotation);

        float r1 = Random.Range(-weapons[selectedIndex].weaponItem.bulletSpread, weapons[selectedIndex].weaponItem.bulletSpread);
        float r2 = Random.Range(-weapons[selectedIndex].weaponItem.bulletSpread, weapons[selectedIndex].weaponItem.bulletSpread);
        float r3 = Random.Range(-weapons[selectedIndex].weaponItem.bulletSpread, weapons[selectedIndex].weaponItem.bulletSpread);

        // Randomization to the bullet direction
        Vector3 newDirection = direction + new Vector3(r1, r2, r3);

        Bullet bullet = Instantiate(bulletPrefab, playerCamera.transform.position, Quaternion.identity).GetComponent<Bullet>();
        Physics.IgnoreCollision(coll, bullet.GetComponent<Collider>(), true);
        bullet.direction = newDirection;
        bullet.weaponItem = weapons[selectedIndex].weaponItem;
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
                    currentHealth.ChangeHealth(-weapons[selectedIndex].weaponItem.bulletDamage);
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

    public WeaponData GetCurrentWeapon()
    {
        return weapons[selectedIndex];
    }
}