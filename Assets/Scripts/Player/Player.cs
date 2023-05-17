using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public enum WeaponType
    {
        Single,
        Multi,
        Auto
    }

    public enum BulletType
    {
        Normal,
        Follow,
        Bounce,
        Charge,
        ChargeBounce,
    }

    public enum PlayerState
    {
        Grounded,
        Jumping,
        Falling,
        Crouching,
        Sliding,
        Running,
    }

    [System.Serializable]
    public struct WeaponData
    {
        [Header("Name")]
        public string Name;

        [Header("Weapon Type")]
        public WeaponType weapon;
        public BulletType bulletType;

        [Header("Bullets")]
        public int bulletDamage;
        public float timeBetweenShots;
        public int bulletCount;
        [SerializeField, Range(0, 1)]
        public float bulletSpread;
        public float reloadTime,
                     bounceCount,
                     enemyKnockback;

        [Header("Trail Rendering")]
        public TrailRenderer bulletTrail;
        public float trailSpeed;

        [Header("Bullet Hole & Debugging")]
        public GameObject hitParticle;


        public bool isReloading,
                      isEmpty,
                      isShooting;

        [Header("Explosion")]
        public bool explodeOnDeath;

        public bool playerCanBeHit;
        public float explosionRadius;

        internal float currentBulletCount,
                       previousFireTime;
    }

    public abstract class Health : MonoBehaviour
    {
        [Space()]
        [Header("Health")]
        [SerializeField] public int health;
        [SerializeField] public float invulnerabilitySeconds;
        [SerializeField] public bool isInvulnerable = false;
        [SerializeField] public bool hasInvulnerability = false;

        public Rigidbody rb;

        public abstract void Hit(int damage, Vector3 pos, float knockbackForce);

        public abstract void OnDeath();

        public IEnumerator Invulnerable(float seconds)
        {
            isInvulnerable = true;

            yield return new WaitForSeconds(seconds);

            isInvulnerable = false;
        }

        public IEnumerator Knockback(Vector3 pos, float knockbackForce)
        {
            pos = (pos - rb.transform.position).normalized;

            rb.velocity = new Vector3(0f, 0f, 0f);

            yield return new WaitForSeconds(0.05f);
            rb.AddForce(-pos * knockbackForce, ForceMode.Impulse);
        }
    }

    public static class MovementHelp
    {
        public static Vector3 ClosestPointMesh(this MeshCollider col, Vector3 point)
        {
            point = col.transform.InverseTransformPoint(point);

            var mesh = col.GetComponent<MeshFilter>().mesh;
            float dist = Mathf.Infinity;
            Vector3 closest = Vector3.positiveInfinity;

            foreach (var vertex in mesh.vertices)
            {

                float newDist = (vertex - point).magnitude;

                if (newDist < dist)
                {
                    dist = newDist;
                    closest = vertex;
                }
            }

            return col.transform.TransformPoint(closest);
        }

        public static Vector3 VelocityClamp(float moveSpeed, Vector3 reference)
        {
            Vector3 sirSplinkle = reference;

            if (sirSplinkle.magnitude > moveSpeed)
            {
                Vector3 newVel = reference.normalized * moveSpeed;
                sirSplinkle = new Vector3(newVel.x, reference.y, newVel.z);
            }

            return sirSplinkle;
        }
    }
}

