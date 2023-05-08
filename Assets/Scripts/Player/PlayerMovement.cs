using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using static PlayerController;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    // Enum
    public enum PlayerState
    {
        Walking,
        Running,
        Firing,
    }

    internal PlayerState state;
    internal PlayerState prevState;
    internal float stateDur;

    internal Rigidbody rb;

    [Header("Camera Variables")]
    public Transform viewPosition;
    internal PlayerCamera Camera;
    internal Vector2 viewTilt;

    [Header("Ground Layer")]
    public LayerMask groundLayer;

    [Header("Movement Variables")]
    public float walkingSpeed;
    public float runningSpeed;

    public float acceleration,
                 deceleration;

    [Header("VECTORS! OH YEAH!!")]
    Vector3 currentVel;
    Vector3 moveDir;
    Vector2 inputs;


    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();

        rb.freezeRotation = true;
        Camera = FindObjectOfType<PlayerCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        void ChangeState(PlayerState changeState)
        {
            prevState = state;
            state = changeState;
            stateDur = 0f;
        }

        #region Inputs

        // Inputs
        inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        bool inputting = (inputs != new Vector2(0f, 0f));
        bool running = Input.GetKey(KeyCode.LeftShift);

        #endregion

        // Grounded *REMEMBER THAT THIS EXISTS! AND TO SET GROUND LAYER!*
        bool isGrounded = Physics.Raycast(new Ray(rb.transform.position, Vector3.down), 2f, groundLayer);

        #region State Machine

        stateDur += Time.deltaTime;

        // While Running
        switch (state)
        {
            case (PlayerState.Walking):

                if (running) ChangeState(PlayerState.Running);

                break;

            case (PlayerState.Running):

                if (!running) ChangeState(PlayerState.Walking);

                break;
        }

        #endregion

        // Cool Camera
        CameraTilt();

        // Speed
        float moveSpeed = (state == PlayerState.Running) ? runningSpeed : walkingSpeed;
        float speedIncrease = inputting ? acceleration : deceleration;

        // Move Direction
        moveDir = (viewPosition.forward * inputs.y + viewPosition.right * inputs.x) * moveSpeed;

        Vector3 newVel = Vector3.SmoothDamp(rb.velocity, moveDir, ref currentVel, speedIncrease);
        if (!isGrounded) newVel.y += -0.75f;

        newVel.y = Mathf.Clamp(newVel.y, -300, Mathf.Infinity);

        // Clamp Speed
        ClampVel(moveSpeed, newVel);

        AdjustSlopeVel(newVel);
        rb.velocity = newVel;
    }

    public Vector3 AdjustSlopeVel(Vector3 vel)
    {
        Ray ray = new Ray(rb.transform.position, Vector3.down);
        RaycastHit h;

        if (Physics.Raycast(ray, out h, 10f, groundLayer))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.down, h.normal);
            Vector3 adjustedVel = slopeRotation * vel;

            return adjustedVel;
        }

        return vel;
    }

    // Movement
    public Vector3 ClampVel(float moveSpeed, Vector3 vel)
    {
        Vector3 v = vel;

        if (v.magnitude > moveSpeed)
        {
            Vector3 newVel = vel.normalized * moveSpeed;
            AdjustSlopeVel(newVel);
            v = new Vector3(newVel.x, vel.y, newVel.z);
        }

        return v;
    }

    void CameraTilt()
    {
        if (inputs.x != 0)
        {
            if (Mathf.Sign(inputs.x) == 1)
                viewTilt.x = Mathf.Lerp(viewTilt.x, (state == PlayerState.Running) ? -3 : -2, 3 * Time.deltaTime);
            else
                viewTilt.x = Mathf.Lerp(viewTilt.x, (state == PlayerState.Running) ? 3 : 2, 3 * Time.deltaTime);
        }
        else
        {
            viewTilt.x = Mathf.Lerp(viewTilt.x, 0, 3 * Time.deltaTime);
        }

        if (inputs.y != 0)
        {
            if (Mathf.Sign(inputs.y) == 1)
                viewTilt.y = Mathf.Lerp(viewTilt.y, (state == PlayerState.Running) ? -2 : -1, 3 * Time.deltaTime);
            else
                viewTilt.y = Mathf.Lerp(viewTilt.y, (state == PlayerState.Running) ? 2 : 1, 3 * Time.deltaTime);
        }
        else
        {
            viewTilt.y = Mathf.Lerp(viewTilt.y, 0, 3 * Time.deltaTime);
        }
    }
}
