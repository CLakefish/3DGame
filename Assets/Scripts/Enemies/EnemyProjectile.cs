using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// unused
public class EnemyProjectile : MonoBehaviour
{
    public int damage;
    public int pierce;

    private void Update()
    {
        if (pierce <= 0 || damage == 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponentInParent<HealthController>().ChangeHealth(-damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag != "Enemy") Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            other.GetComponentInParent<HealthController>().ChangeHealth(-damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag != "Enemy") Destroy(gameObject);
    }
}
