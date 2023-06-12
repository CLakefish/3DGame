/*
Author: Carson L, Preston C
Date: 5/23/2023
Desc: Scriptable weapon object
*/

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
    public WeaponData weaponData;
    internal int currentDamage;
    internal float currentKnockback;
    internal float currentTrailSpeed;
    internal float currentExplosionStrength;
    internal int currentBounceCount;
}
