using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class PlayerControls : MonoBehaviour
{
    #region Variables

    [Header("State Handlers")] // See Player for details
    public PlayerState state = PlayerState.Grounded;
    internal PlayerState prevState;
    internal float stateDur;

    [Header("Rigidbody")]
    internal Rigidbody rb;

    [Header("Camera Variables")] // Used for viewPosition & viewTilt
    public Transform viewPosition;
    internal PlayerCamera cam;
    internal Vector2 viewTilt, currentTiltSpeed;
    const float smoothDampSpeed = 0.1f, // Const since you don't need to re-assign these at any time
                maxSmoothDampSpeed = 10f;

    [Header("Layer Detection")] // For ground detection
    public LayerMask groundLayer;

    [Header("Movement Variables")] // Eventually Add Gravity
    public float walkingSpeed;
    public float runningSpeed;
    public Vector2 acceleration,
                   deceleration;

    public float jumpTime,
                 jumpSpeed,
                 jumpBufferTime;
    float jumpBufferTimeTemp;

     float groundRay = 0.005f;

    CapsuleCollider col;

    public bool isRunning;

    [Header("VECTORS! OH YEAH!!")] // for use in velocity
    Vector3 currentVel;
    Vector3 moveDir;
    Vector2 inputs;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Get the RB (Player tag should only be applied to the rigidbody component itself)
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cam = FindObjectOfType<PlayerCamera>();
        col = rb.GetComponent<CapsuleCollider>();

        jumpBufferTimeTemp = jumpBufferTime;
    }

    // Update is called once per frame
    void Update()
    {
        void ChangeState(PlayerState newState)
        {
            prevState = state;

            state = newState;

            stateDur = 0f;
        }

        // Handles wall and ground collisions
        #region Collision Detection

        bool checkCollider(Vector3 pos, out RaycastHit h)
        {
            float rad = col.radius * .7f;

            // For each possible collider, get the closest one then return if you're hitting it.
            foreach (var collider in Physics.OverlapSphere(pos, rad, groundLayer))
            {
                if (Physics.Raycast(new(rb.transform.position, (collider is MeshCollider ? (((MeshCollider)collider).ClosestPointMesh(rb.transform.position)) : collider.ClosestPoint(rb.transform.position)) - rb.transform.position), out h, Mathf.Infinity, groundLayer))
                {
                    return true;
                }
            }

            h = default;
            return false;
        }

        // Ground & Wall Detection
        bool isGrounded = checkCollider(rb.transform.position + ((col.height / 2) + groundRay) * Vector3.down, out RaycastHit ground);

        #endregion

        #region Inputs

        // Inputs
        inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        bool inputting = (inputs != new Vector2(0f, 0f));
        isRunning = Input.GetKey(KeyCode.LeftShift);

        #endregion

        // Handles x & z movement & camera movement
        #region Movement 

        // Speed
        float moveSpeed = isRunning ? runningSpeed : walkingSpeed,
              speedIncrease = inputting ? (isGrounded ? acceleration.x : acceleration.y) : (isGrounded ? deceleration.x : deceleration.y);

        // Get the move direction of the viewPosition multiplied by the move speed
        moveDir = (viewPosition.forward * inputs.y + viewPosition.right * inputs.x) * moveSpeed;

        // Camera Tilting
        CameraTilt(Input.GetKey(KeyCode.LeftShift));

        #endregion

        #region states

        if (stateDur == 0)
        {
            switch (state)
            {
                case (PlayerState.Jumping):

                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                    rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);

                    break;
            }
        }

        stateDur += Time.deltaTime;

        jumpBufferTimeTemp = Mathf.Lerp(jumpBufferTimeTemp, 0, Time.deltaTime);

        switch (state)
        {
            case (PlayerState.Grounded):
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded) ChangeState(PlayerState.Jumping);

                if (jumpBufferTimeTemp > 0)
                {
                    jumpBufferTimeTemp = 0f;
                    ChangeState(PlayerState.Jumping);
                }

                if (!isGrounded && !Input.GetKey(KeyCode.Space)) ChangeState(PlayerState.Falling);
                break;

            case (PlayerState.Jumping):

                if ((stateDur > jumpTime) || (rb.velocity.y < 0) || (!Input.GetKey(KeyCode.Space))) ChangeState(PlayerState.Falling);

                break;

            case (PlayerState.Falling):

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    jumpBufferTimeTemp = jumpBufferTime;
                }

                rb.AddForce(Vector3.down * 0.05f, ForceMode.Impulse);

                if (isGrounded) ChangeState(PlayerState.Grounded);

                break;
        }

        #endregion

        // Smooth Damp for the win
        rb.velocity = new Vector3(Mathf.SmoothDamp(rb.velocity.x, moveDir.x, ref currentVel.x, speedIncrease), rb.velocity.y, Mathf.SmoothDamp(rb.velocity.z, moveDir.z, ref currentVel.z, speedIncrease));

        // Clamp the Velocity to the Move Speed
        MovementHelp.VelocityClamp(moveSpeed, rb.velocity);

        Debug.Log(isGrounded);
    }

    void CameraTilt(bool running)
    {
        // Tilt Values
        Vector2 xTiltValues = new(3, 2),
                yTiltValues = new(2, 1);

        // Tilting
        Vector2 tiltStrength = -inputs * (running ? new Vector2(xTiltValues.x, yTiltValues.x) : new Vector2(xTiltValues.y, yTiltValues.y));

        // Smoothdamp? I hardly know her!
        viewTilt = new(
          Mathf.SmoothDamp(viewTilt.x, tiltStrength.x, ref currentTiltSpeed.x, smoothDampSpeed, maxSmoothDampSpeed, Time.deltaTime),
          Mathf.SmoothDamp(viewTilt.y, tiltStrength.y, ref currentTiltSpeed.y, smoothDampSpeed, maxSmoothDampSpeed, Time.deltaTime));
    }
}
