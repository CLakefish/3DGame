using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Player;

/*[CustomEditor(typeof(WeaponItem))]
public class WeaponItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.TextArea("Very easy to use, just open the weapon data and change the attributes!");

        base.OnInspectorGUI();
    }
}*/

[CreateAssetMenu(fileName = "Weapon Attributes")]
public class WeaponItem : ScriptableObject
{
    [Header("Name")]
        public string Name;

        [Header("Weapon Type")]
        public WeaponType weaponType;
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

        [Header("Explosion")]
        public bool explodeOnDeath;

        public bool playerCanBeHit;
        public float explosionRadius;
        public float explosionStrength;
}
