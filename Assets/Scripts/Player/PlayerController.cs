using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    public PlayerCamera Camera;
    public Transform viewPosition;

    [Header("Firing")]
    public LayerMask layers;

    [Header("Rigidbody")]
    public Rigidbody rb;

    [Header("Debugging")]
    internal PlayerState state;
    internal PlayerState prevState;
    internal float stateDur;

    [Header("Movement")]
    public float walkingSpeed;
    public float runningSpeed;
    public float acceleration,
                 deceleration;
    float speedIncrease;
    float moveSpeed;

    [Header("View")]
    internal float walkingFOV;
    internal float runningFOV;
    internal float viewTilt;
    float FOV;

    [Header("VECTORS! OH YEAH!!")]
    Vector3 currentVel;
    Vector3 moveDir;
    Vector2 inputs;

    bool canShoot = true;

    void Start()
    {
        rb.freezeRotation = true;

        // Field of View

        walkingFOV = Camera.FOV;
        runningFOV = Camera.FOV * 1.15f;

        FOV = (state == Player.PlayerState.Running) ? runningFOV : walkingFOV;
    }

    void Update()
    {


        void ChangeState(PlayerState changeState)
        {
            prevState = state;
            state = changeState;
            stateDur = 0f;
        }

        // Inputs
        inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool inputting = (inputs != new Vector2(0f, 0f));
        bool running = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetMouseButtonDown(0) && canShoot) StartCoroutine(ShootObj());

        // Speed
        moveSpeed = (state == Player.PlayerState.Running) ? runningSpeed : walkingSpeed;
        speedIncrease = inputting ? acceleration : deceleration;

        // Move Direction
        moveDir = (viewPosition.forward * inputs.y + viewPosition.right * inputs.x) * moveSpeed;
        ClampVel();

        CameraTilt();

        #region State Machine

        // On enter
        if (stateDur == 0)
        {
            switch (state)
            {
                case (Player.PlayerState.Walking):

                    break;

                case (Player.PlayerState.Running):

                    break;

            }
        }

        stateDur += Time.deltaTime;

        // While Running
        switch (state)
        {
            case (Player.PlayerState.Walking):

                FOV = Mathf.Lerp(FOV, walkingFOV, 10 * Time.deltaTime);

                if (running) ChangeState(PlayerState.Running);

                break;

            case (Player.PlayerState.Running):

                FOV = Mathf.Lerp(FOV, runningFOV, 10 * Time.deltaTime);

                if (!running) ChangeState(PlayerState.Walking);

                break;
        }

        #endregion

        Camera.FOV = FOV;
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector3.SmoothDamp(rb.velocity, moveDir, ref currentVel, speedIncrease);
    }

    IEnumerator ShootObj()
    {
        Debug.Log("Fired!");
        canShoot = false;

        Ray ray = Camera.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast;

        if (Physics.Raycast(ray, out raycast, Mathf.Infinity, layers))
        {
            Debug.Log("Hit!");
        }

        yield return new WaitForSeconds(1f);

        canShoot = true;
        Debug.Log("Can shoot!");
    }

    void ClampVel()
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (vel.magnitude > moveSpeed)
        {
            Vector3 newVel = vel.normalized * moveSpeed;
            rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
        }
    }

    void CameraTilt()
    {
        if (inputs.x != 0)
        {
            if (inputs.x > 0)
            {
                viewTilt = Mathf.Lerp(viewTilt, (state == Player.PlayerState.Running) ? -3 : -2, 3 * Time.deltaTime);
            }
            else if (inputs.x < 0)
            {
                viewTilt = Mathf.Lerp(viewTilt, (state == Player.PlayerState.Running) ? 3 : 2, 3 * Time.deltaTime);
            }
        }
        else
        {
            viewTilt = Mathf.Lerp(viewTilt, 0, 3 * Time.deltaTime);
        }
    }
}
