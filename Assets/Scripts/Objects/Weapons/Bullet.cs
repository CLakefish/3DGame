using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public WeaponItem weaponItem;
    float bounceCount;
    Rigidbody rb;

    Vector3 startPos;
    public Vector3 direction;
    void Start()
    {
        bounceCount = weaponItem.bounceCount;
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * weaponItem.trailSpeed;
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody)
        {
            collision.rigidbody.velocity += rb.velocity.normalized * weaponItem.enemyKnockback;
        }

        if (collision.transform.TryGetComponent(out HealthController currentHealth))
        {
            currentHealth.ChangeHealth(-weaponItem.bulletDamage);
        }
        if (bounceCount > 0)
        {
            bounceCount--;
            return;
        }
        Destroy(gameObject);
    }
}
