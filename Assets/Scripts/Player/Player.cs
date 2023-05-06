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

        [Header("Weapon Attributes")]
        public float bulletDamage;
        public float bulletCount,
                     fireRate,
                     reloadTime,
                     bounceCount,
                     enemyKnockback;

        [Header("Trail Rendering")]
        public TrailRenderer bulletTrail;
        public float trailSpeed;

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
            fireRate = newData.fireRate;
            reloadTime = newData.reloadTime;
            enemyKnockback = newData.enemyKnockback;
            currentBulletCount = newData.currentBulletCount;

            isEmpty = newData.isEmpty;
            isReloading = false;
        }
    }
}

