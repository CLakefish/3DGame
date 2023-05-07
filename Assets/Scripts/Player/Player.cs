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

        internal bool isReloading,
                      isEmpty,
                      isShooting;

        internal float currentBulletCount,
                       previousFireTime;

        public void ApplyChanges(WeaponData newData)
        {
            weapon = newData.weapon;
            bulletType = newData.bulletType;

            bulletDamage = newData.bulletDamage;
            bulletCount = newData.bulletCount;
            timeBetweenShots = newData.timeBetweenShots;
            reloadTime = newData.reloadTime;
            enemyKnockback = newData.enemyKnockback;
            currentBulletCount = newData.currentBulletCount;

            isEmpty = newData.isEmpty;
            isReloading = false;
        }
    }
}

