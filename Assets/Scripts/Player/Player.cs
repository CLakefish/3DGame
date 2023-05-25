using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public bool isReloading = false;
    public bool isEmpty = false;
    public bool isShooting = false;
    public int currentBulletCount = 0;
    public float previousFireTime;
    public WeaponItem weaponItem;
}

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

