using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class Enemy : Health
{
    Vector3 vel;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity += new Vector3(0f, -100f * Time.deltaTime, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit");

        if (collision.gameObject.tag == "PlayerProjectile")
        {
            Hit(1, collision.gameObject.transform.position, new Vector3(10f, 10f, 10f));
        }
    }

    public override void OnDeath()
    {
        Destroy(gameObject);
        //throw new System.NotImplementedException();
    }
}
