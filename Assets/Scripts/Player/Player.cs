using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    // p: i really do not understand this namespace thing
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
        public int bulletDamageChargeBonus;

        public float timeBetweenShots;
        public int bulletCount;
        [SerializeField, Range(0, 1)]
        public float bulletSpread;
        public float reloadTime,
                     enemyKnockback;
        
        public int bounceCount;
        public int bounceCountChargeBonus;

        [Header("Trail Rendering")]
        public TrailRenderer bulletTrail;
        public float trailSpeed;
        public float trailSpeedChargeBonus;

        [Header("Bullet Hole & Debugging")]
        public GameObject hitParticle;


        public bool isReloading,
                      isEmpty,
                      isShooting;

        [Header("Explosion")]
        public bool explodeOnDeath;

        public bool playerCanBeHit;
        public float explosionRadius;
        public float explosionStrength;

        internal float currentBulletCount,
                       previousFireTime;
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
    }
}

