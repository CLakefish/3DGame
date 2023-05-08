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
        public float bulletDamage;
        public float timeBetweenShots;
        public float bulletCount;
        [SerializeField, Range(0, 1)]
        public float bulletSpread;
        public float reloadTime,
                     bounceCount,
                     enemyKnockback;

        [Header("Trail Rendering")]
        public TrailRenderer bulletTrail;
        public float trailSpeed;

        [Header("Bullet Hole")]
        public GameObject hitParticle;

        public bool isReloading,
                      isEmpty,
                      isShooting;

        internal float currentBulletCount,
                       previousFireTime;
    }
}

