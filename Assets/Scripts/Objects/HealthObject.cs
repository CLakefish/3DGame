using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// unused
public class HealthObject : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int hp;
    [SerializeField] bool canUse;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && canUse)
        {
            // Health player = other.gameObject.GetComponentInParent<Health>();

            // if (player == null || player.health >= 100) return;

            // player.health = Mathf.Min(100, player.health + hp);


            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        canUse = false;
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.gray;

        yield return new WaitForSeconds(1f);

        canUse = true;
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
}