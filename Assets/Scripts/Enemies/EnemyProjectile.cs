using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            other.gameObject.GetComponentInParent<PlayerHealth>().Hit(damage, transform.position, 1f);
        }
        else
            pierce--;
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponentInParent<PlayerHealth>().Hit(damage, transform.position, 1f);
        }
            else pierce--;
    }
}
