using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Test : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public Transform orientation;

    float horizantalInput;
    float verticalInput;

    Vector3 moveDir;
    
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        P_Input();
    }

    private void P_Input()
    {
        horizantalInput = Input.GetAxisRaw("Horizantal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void Move()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizantalInput;
        rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
    }

}
