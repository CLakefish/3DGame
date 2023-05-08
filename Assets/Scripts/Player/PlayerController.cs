using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    #region Parameters

    [Header("Assignables")]
    internal Rigidbody rb;
    internal PlayerCamera Camera;

    [Header("Weapon Information")]
    public List<WeaponItem> weaponItems;
    [Header("Debugging")]
    public WeaponData weaponData;
    public int selectedIndex = 0;

    [Header("Shootable Layers")]
    public LayerMask layers;
    float previousFireTime;

    #endregion

    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        Camera = FindObjectOfType<PlayerCamera>();

        weaponData = weaponItems[0].weaponData;
        weaponData.currentBulletCount = weaponData.bulletCount;
    }

    void Update()
    {

        // Weapon Change 
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")) == 1)
            {
                weaponItems[selectedIndex].weaponData = weaponData;

                if (selectedIndex < weaponItems.Count - 1) selectedIndex++;
                else selectedIndex = 0;

                weaponData = weaponItems[selectedIndex].weaponData;
                weaponData.isEmpty = false;
            }
            else
            {
                weaponItems[selectedIndex].weaponData = weaponData;

                if (selectedIndex > 0) selectedIndex--;
                else selectedIndex = weaponItems.Count - 1;


                weaponData = weaponItems[selectedIndex].weaponData;
                weaponData.isEmpty = false;
            }
        }

        // Shooting 
        if (Input.GetMouseButton(0) && weaponData.currentBulletCount > 0 && !weaponData.isReloading) ShootObj();
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
        // If the time is greater than the previous time + fireRate time then fire (Technically makes fireRate different but whatever)
        if (Time.time > previousFireTime + weaponData.timeBetweenShots && weaponData.currentBulletCount > 0)
        {
            Debug.Log("gone");

            previousFireTime = Time.time;
            weaponData.isShooting = true;

            // For different weapon types
            switch (weaponData.weapon)
            {
                case (Player.WeaponType.Single):
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
        // Raycast to Mouse Input Position in world
        Ray ray = Camera.cam.ScreenPointToRay(Input.mousePosition);

        // Trail rendering
        TrailRenderer trail = Instantiate(weaponData.bulletTrail, rb.transform.position, Quaternion.identity);

        // Raycast Hit reference
        RaycastHit raycast;

        // Randomization to the bullet direction
        ray.direction += new Vector3(Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread), Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread), Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread));

        // If it hits or does not hit based on a raycast, Mathf.Infinity can be changed soon enough.
        if (Physics.Raycast(ray, out raycast, Mathf.Infinity, layers))
        {
            Debug.Log("Hit!");

            StartCoroutine(SpawnTrail(trail, raycast,  raycast.point, raycast.normal, weaponData.bounceCount, 100, true));
        }
        else
        {
            StartCoroutine(SpawnTrail(trail, raycast, ray.direction * 100, new Vector3(0f, 0f, 0f), weaponData.bounceCount, 100, false));
        }
    }

    // Visualization
    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit raycast, Vector3 point, Vector3 normal, float bounceCount, float bounceDistance, bool hitObj)
    {
        if (weaponData.bulletType == BulletType.Follow)
        {
            Vector3 startPos = trail.transform.position;
            Vector3 EnemyPos = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Enemy")).transform.position + new Vector3(Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread), Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread), Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread));

            float d = Vector3.Distance(trail.transform.position, EnemyPos);
            float startDist = d;

            while (d > 0)
            {
                trail.transform.position = Vector3.Slerp(startPos, EnemyPos, 1 - (d / startDist));
                d -= Time.deltaTime * weaponData.trailSpeed;

                yield return null;
            }

            trail.transform.position = EnemyPos;

            Destroy(trail, .25f);
        }
        else
        {

            Vector3 startPos = trail.transform.position;
            Vector3 dir = (point - trail.transform.position).normalized;

            float d = Vector3.Distance(trail.transform.position, point);
            float startDist = d;

            while (d > 0)
            {
                trail.transform.position = Vector3.Lerp(startPos, point, 1 - (d / startDist));
                d -= Time.deltaTime * weaponData.trailSpeed;

                yield return null;
            }

            trail.transform.position = point;

            if (hitObj)
            {
                // Use Object Pooling here eventually 

                GameObject obj = Instantiate(weaponData.hitParticle, point, Quaternion.LookRotation(normal));
                obj.transform.parent = raycast.collider.gameObject.transform;
                Destroy(obj, 2f);

                if (weaponData.bulletType == BulletType.Bounce && bounceCount > 0)
                {
                    Vector3 bounceDir = Vector3.Reflect(dir, normal);

                    bounceCount--;

                    // Recursion!
                    if (Physics.Raycast(point, bounceDir, out RaycastHit h, Mathf.Infinity, layers))
                    {
                        yield return StartCoroutine(SpawnTrail(trail, raycast, h.point, h.normal, bounceCount, bounceDistance - Vector3.Distance(h.point, point), true));
                    }
                    else
                    {
                        yield return StartCoroutine(SpawnTrail(trail, raycast, bounceDir * bounceDistance, Vector3.zero, 0, 0, false));
                    }
                }
                else if (bounceCount <= 0) Destroy(trail, trail.time * 2f);
            }

            Destroy(trail, .25f);
        }
    }

    #endregion

    GameObject GetClosestEnemy(GameObject[] enemies)
    {
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
