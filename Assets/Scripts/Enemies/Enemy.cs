using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class Enemy : Health
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "PlayerProjectile")
        {
            Hit(1);
        }
    }

    public override void OnDeath()
    {

        Destroy(gameObject);

        throw new System.NotImplementedException();
    }
}
