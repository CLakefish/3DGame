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
    public float chargeTime;
    public bool canShoot = true;
    [Header("Explosion Prefab")]
    [SerializeField] GameObject explosion;
    bool mouseButtonDown;

    #endregion

    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerCamera = FindObjectOfType<PlayerCamera>();

        if (weaponItems == null)
        {
            return;
        }
        else
        {
            weaponData = weaponItems[0].weaponData;
            weaponData.currentBulletCount = weaponData.bulletCount;

            weaponData.isReloading = false;
            weaponData.isEmpty = false;
        }
    }

    void Update()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        int scrollValueDirection = scrollValue != 0 ? (int)Mathf.Sign(scrollValue) : 0;
        ScrollWeapon(scrollValueDirection);

        mouseButtonDown = Input.GetMouseButton(0);

        // Shooting 
        if (mouseButtonDown)
        {
            ShootObj();
        }

        if(Input.GetKeyDown(KeyCode.R) || weaponData.currentBulletCount <= 0)
        { 
            StartCoroutine(Reload());
        }
    }

    void ScrollWeapon(int direction)
    {
        if (direction == 0)
        {
            return;
        }
        if (weaponItems.Count <= 1)
        {
            return;
        }
        // Weapon Change
        
        selectedIndex += direction;
        selectedIndex = Mathf.Clamp(selectedIndex, 0, weaponItems.Count - 1); 

        weaponData = weaponItems[selectedIndex].weaponData;
        weaponData.isEmpty = false;
        weaponData.isReloading = false;

        chargeTime = 0;
        weaponItems[selectedIndex].weaponData = weaponData;

        StopCoroutine(ItemSwitchPause());
        StartCoroutine(ItemSwitchPause());
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
        if (weaponData.currentBulletCount <= 0)
        {
            return;
        }
        if (weaponData.isReloading)
        {
            return;
        }
        if (!canShoot)
        {
            return;
        }
        // If the time is greater than the previous time + fireRate time then fire (Technically makes fireRate different but whatever)
        if (Time.time <= previousFireTime + weaponData.timeBetweenShots || weaponData.currentBulletCount <= 0)
        {
            weaponData.isShooting = false;
            return;
        }
        previousFireTime = Time.time;
        weaponData.isShooting = true;

        // For different weapon types
        switch (weaponData.weapon)
        {
            case (Player.WeaponType.Single):

                if (weaponData.bulletType == BulletType.Charge || weaponData.bulletType == BulletType.ChargeBounce)
                {
                    // the bullet count goes down in the coroutine
                    StopCoroutine(ChargedProjectileShotHandler());
                    StartCoroutine(ChargedProjectileShotHandler());
                }
                else
                {
                    weaponData.currentBulletCount--;
                    ProjectileShotHandler();
                }
                break;

            case (Player.WeaponType.Multi):

                // Eventually this'll be better
                // update: nah
                for (int i = 0; i < (weaponData.bulletCount / 2); i++)
                {
                    if (weaponData.currentBulletCount <= 0)
                    {
                        break;
                    }
                    weaponData.currentBulletCount--;
                    ProjectileShotHandler();
                }

                break;
        }
    }

    // Projectile Fire
    void ProjectileShotHandler()
    {
        // Raycast to Mouse Input Position in world
        Ray ray = new Ray(firePos.transform.position, firePos.transform.rotation * Vector3.forward);

        // Trail rendering
        TrailRenderer trail = Instantiate(weaponData.bulletTrail, firePos.transform.position, playerCamera.transform.rotation);

        // Raycast Hit reference
        RaycastHit raycast;

        // Raycast to Mouse Input Position in world

        float r1 = Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread);
        float r2 = Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread);
        float r3 = Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread);

        // Randomization to the bullet direction
        ray.direction += new Vector3(r1, r2, r3);
        
        bool hit = Physics.Raycast(ray, out raycast, 1000);

        Vector3 point;
        Vector3 normal;

        if (hit)
        {
            point = raycast.point;
            normal = raycast.normal;
        }
        else
        {
            point = ray.direction * 4000;
            normal = Vector3.zero;
        }

        if (weaponData.bulletType == BulletType.Follow)
        {
            GameObject obj = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Enemy"));

            if (obj == null) return;

            // If it hits or does not hit based on a raycast, Mathf.Infinity can be changed soon enough.
        }
        StartCoroutine(SpawnTrail(trail, raycast, point, normal, weaponItems[selectedIndex].currentBounceCount, 100, hit, weaponItems[selectedIndex].currentDamage, weaponItems[selectedIndex].currentTrailSpeed, weaponData.bulletType == BulletType.Follow));
    }

    IEnumerator ItemSwitchPause()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.4f);
        canShoot = true;
    }

    // Fix this
    float maxChargeTime = 2;
    IEnumerator ChargedProjectileShotHandler()
    {
        weaponData.isReloading = true;

        int bounceCount = weaponData.bounceCount;
        float tempStore = weaponData.trailSpeed;
        float attack = weaponData.bulletDamage;
        chargeTime = 0;

        while (mouseButtonDown && chargeTime < maxChargeTime)
        {
            chargeTime = Mathf.MoveTowards(chargeTime, maxChargeTime, Time.deltaTime);

            float charge = chargeTime / maxChargeTime;

            weaponItems[selectedIndex].currentTrailSpeed = Mathf.Lerp(weaponData.trailSpeed, weaponData.trailSpeed + weaponData.trailSpeedChargeBonus, charge);
            
            weaponItems[selectedIndex].currentDamage = (int)Mathf.Lerp(weaponData.bulletDamage, weaponData.bulletDamage + weaponData.bulletDamageChargeBonus, charge);

            weaponItems[selectedIndex].currentBounceCount = (int)Mathf.Lerp(weaponData.bounceCount, weaponData.bounceCount + weaponData.bounceCountChargeBonus, charge);

            weaponItems[selectedIndex].currentKnockback = Mathf.Lerp(weaponData.enemyKnockback, weaponData.enemyKnockback + weaponData.enemyKnockbackChargeBonus, charge);

            weaponItems[selectedIndex].currentExplosionStrength = Mathf.Lerp(weaponData.explosionStrength, weaponData.explosionStrength + weaponData.explosionStrengthChargeBonus, charge);

            yield return null;
        }
        chargeTime = 0;

        weaponData.bounceCount = bounceCount;
        weaponData.trailSpeed = tempStore;

        weaponData.currentBulletCount--;

        ProjectileShotHandler();

        weaponData.isReloading = false;
    }

    // Visualization
    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit raycast, Vector3 point, Vector3 normal, float bounceCount, float bounceDistance, bool hitObj, float damage, float speed, bool follow = false, GameObject positionUpdate = null)
    {
        Vector3 startPos = trail.transform.position;
        Vector3 dir = (follow) ? (rb.transform.position - point).normalized : (point - trail.transform.position).normalized;

        float d = Vector3.Distance(trail.transform.position, point);
        float startDist = d;

        // the shotgun breaks in this while loop
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
                    currentHealth.ChangeHealth(-weaponItems[selectedIndex].currentDamage);
                }
                if (raycast.rigidbody)
                {
                    raycast.rigidbody.velocity += dir * weaponItems[selectedIndex].currentKnockback;
                }
            }

            Destroy(obj, 2f);

            if (weaponData.explodeOnDeath && trail != null)
            {
                Explode(trail.transform.position, weaponItems[selectedIndex].currentKnockback, weaponData.explosionRadius, weaponItems[selectedIndex].currentExplosionStrength);
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
                    Explode(trail.transform.position, weaponItems[selectedIndex].currentKnockback, weaponData.explosionRadius, weaponItems[selectedIndex].currentExplosionStrength);
                }
            }

            Destroy(trail, 0.1f);
        }
    }

    void Explode(Vector3 explosionPos, float knockbackValue, float explosionSize, float explosionStrength)
    {
        // For each possible collider, get the closest one then return if you're hitting it.
        foreach (Collider collider in Physics.OverlapSphere(explosionPos, explosionSize))
        {
            var type = collider.GetType();
            Vector3 difference;
            if (type == typeof(CapsuleCollider) || type == typeof(BoxCollider) || type == typeof(SphereCollider))
            {
                difference = collider.ClosestPoint(explosionPos) - explosionPos;
            }
            else
            {
                difference = collider.transform.position - explosionPos;
            }
            
            if (collider.TryGetComponent(out HealthController currentHealth))
            {
                if (currentHealth.transform != transform)
                {
                    currentHealth.ChangeHealth(-weaponData.bulletDamage);
                }
            }
            if (collider.TryGetComponent(out Rigidbody currentRigidbody))
            {
                float explosionDistance = difference.magnitude / explosionSize;
                float proximity = -(explosionDistance - 1);
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
