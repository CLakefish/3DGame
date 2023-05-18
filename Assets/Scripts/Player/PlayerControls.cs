using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Unity.VisualScripting;

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
    [SerializeField] Vector2 walkingSpeed;
    [SerializeField] Vector2 runningSpeed, acceleration, deceleration;
    [Header("Slopes")]
    [SerializeField] float maxSlopeAngle;

    [Tooltip("1 is default, > 1 is a positive modifier, < 1 is a negative modifier")]
    [Header("Movement Modifiers")]
    public string README;
    [Tooltip("Velocity modifier for when entering a slide")]
    public float slideModifier = 1.0f;
    [Tooltip("Velocity modifier for when jumping out of a slide")]
    public float slideJumpModifier = 1.0f;
    [Tooltip("Velocity modifier for when in the air")]
    public float airModifier = 1.0f;
    [Tooltip("Movement speed modifier for when crouching")]
    public float crouchModifier = 0.99f;
    [Tooltip("Fall speed modifier")]
    public float fallModifier = 0.5f;
    [Tooltip("Friction modifier while sliding")]
    public float frictionModifier = 0.99f;


    [Space()]
    [SerializeField] public float jumpTime;
    [SerializeField] public float jumpSpeed,
                                  jumpBufferTime;
    [SerializeField] public float jumpCoyoteTime;

    float jumpBufferTimeTemp,
          jumpCoyoteTimeT;

     float groundRay = 0.005f;

    CapsuleCollider col;

    public bool isRunning;
    RaycastHit slopeHit;

    [Header("VECTORS! OH YEAH!!")] // for use in velocity
    Vector3 currentVel;
    Vector3 moveDir;
    internal Vector2 inputs;

    [Header("Collider Variables")]
    CapsuleCollider playerCollider;
    float originalHeight;
    float crouchHeight;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Get the RB (Player tag should only be applied to the rigidbody component itself)
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerCollider = GetComponentInChildren<CapsuleCollider>();
        originalHeight = playerCollider.height;
        crouchHeight = originalHeight/2;

        cam = FindObjectOfType<PlayerCamera>();
        col = rb.GetComponent<CapsuleCollider>();

        jumpBufferTimeTemp = jumpBufferTime;
        jumpCoyoteTimeT = 0;
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

        bool onSlope()
        {
            if (Physics.Raycast(rb.transform.position, Vector3.down, out slopeHit, col.height * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            // If the angle of the object beneath the player is more than 0, less than the maxSlopeAngle, then it implies you are on a slope.

            return false;
        }

        Vector3 SlopeMoveDir()
        {
            return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
        }

        // Ground & Wall Detection
        bool isGrounded = checkCollider(rb.transform.position + ((col.height / 2) + groundRay) * Vector3.down, out RaycastHit ground);

        #endregion

        #region Inputs

        // Inputs
        inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        bool inputting = (inputs != new Vector2(0f, 0f)), isCrouching = (Input.GetKey(KeyCode.LeftControl));
        isRunning = Input.GetKey(KeyCode.LeftShift);

        #endregion

        // Handles x & z movement & camera movement
        #region Movement 

        // Speed
        float moveSpeed = isRunning ? ( state == PlayerState.Falling ? runningSpeed.x : runningSpeed.y) : ( state == PlayerState.Falling ? walkingSpeed.x : walkingSpeed.y),
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

                case (PlayerState.Sliding):

                    rb.AddForce(new Vector3(inputs.x, 0f, inputs.y) * slideModifier, ForceMode.Impulse);

                    break;
            }
        }

        stateDur += Time.deltaTime;

        jumpBufferTimeTemp = Mathf.MoveTowards(jumpBufferTimeTemp, 0, Time.deltaTime);
        jumpCoyoteTimeT = Mathf.MoveTowards(jumpCoyoteTimeT, 0, Time.deltaTime);

        switch (state)
        {
            case (PlayerState.Grounded):

                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    ChangeState(PlayerState.Jumping);
                }

                if (jumpBufferTimeTemp > 0)
                {
                    jumpBufferTimeTemp = 0f;
                    ChangeState(PlayerState.Jumping);
                }

                if (!isGrounded && !Input.GetKey(KeyCode.Space))
                {
                    jumpCoyoteTimeT = jumpCoyoteTime;
                    ChangeState(PlayerState.Falling);
                }
                
                if (isRunning)
                {
                    ChangeState(PlayerState.Running);
                }

                if (isCrouching)
                {
                    ChangeState(PlayerState.Crouching);
                }

                break;

            case (PlayerState.Running):

                if (isCrouching)
                {
                    ChangeState(PlayerState.Sliding);
                }

                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    ChangeState(PlayerState.Grounded);
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    ChangeState(PlayerState.Jumping);
                }

                break;

            case (PlayerState.Jumping):

                if ((stateDur > jumpTime) || (rb.velocity.y < 0) || (!Input.GetKey(KeyCode.Space))) ChangeState(PlayerState.Falling);

                rb.velocity = new Vector3((rb.velocity.x * airModifier), rb.velocity.y, (rb.velocity.z * airModifier));

                break;

            case (PlayerState.Falling):

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    if (prevState == PlayerState.Grounded && jumpCoyoteTimeT > 0)
                    {
                        jumpCoyoteTimeT = 0;
                        ChangeState(PlayerState.Jumping);

                        break;
                    }

                    rb.velocity = new Vector3(rb.velocity.x * airModifier, rb.velocity.y, rb.velocity.z * airModifier);

                    jumpBufferTimeTemp = jumpBufferTime;
                }

                rb.AddForce(Vector3.down * stateDur * fallModifier, ForceMode.Impulse);

                if (isGrounded) ChangeState(PlayerState.Grounded);

                break;

            case (PlayerState.Crouching):

                if (prevState != PlayerState.Sliding) {
                    playerCollider.height = crouchHeight;
                }

                rb.velocity = new Vector3(rb.velocity.x * crouchModifier, rb.velocity.y, rb.velocity.z * crouchModifier);

                if(Input.GetKey(KeyCode.Space))
                {
                    playerCollider.height = originalHeight;
                    ChangeState(PlayerState.Jumping);

                }

                if(Input.GetKeyUp(KeyCode.LeftControl))
                {
                    playerCollider.height = originalHeight;
                    ChangeState(PlayerState.Grounded);
                }

                break;

            case (PlayerState.Sliding):

                if (prevState != PlayerState.Crouching){
                    playerCollider.height = crouchHeight;
                }

                if (!onSlope()) {
                    rb.velocity = new Vector3(rb.velocity.x * Mathf.Pow(frictionModifier, stateDur), rb.velocity.y, rb.velocity.z * Mathf.Pow(frictionModifier, stateDur));
                }
                else {
                    rb.velocity = new Vector3(rb.velocity.x / Mathf.Pow(frictionModifier, stateDur), rb.velocity.y, rb.velocity.z / Mathf.Pow(frictionModifier, stateDur));
                }

                if ((rb.velocity.x < 2 && rb.velocity.x > -2) && (rb.velocity.z < 2 && rb.velocity.z > -2))
                {
                    ChangeState(PlayerState.Crouching);
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    playerCollider.height = originalHeight;
                    rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                    rb.AddForce(new Vector3(inputs.x, 0f, inputs.y) * slideJumpModifier, ForceMode.Impulse);
                    ChangeState(PlayerState.Jumping);
                }

                if (Input.GetKeyUp(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.LeftShift))
                {
                    playerCollider.height = originalHeight;
                    ChangeState(PlayerState.Grounded);
                }

                if (Input.GetKeyUp(KeyCode.LeftControl))
                {
                    playerCollider.height = originalHeight;
                    ChangeState(PlayerState.Running);
                }

                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    ChangeState(PlayerState.Crouching);
                }

                break;
        }

        #endregion

        // Smooth Damp for the win
        rb.velocity = new Vector3(Mathf.SmoothDamp(rb.velocity.x, moveDir.x, ref currentVel.x, speedIncrease), rb.velocity.y, Mathf.SmoothDamp(rb.velocity.z, moveDir.z, ref currentVel.z, speedIncrease));

        if (onSlope())
        {
            // Jumping
            if (state == PlayerState.Jumping || Input.GetKey(KeyCode.Space))
            {
                rb.useGravity = true;
                return;
            }

            // Ensure no weird sliding issues
            rb.useGravity = false;

            // Movement properly
            rb.AddForce(SlopeMoveDir() * moveSpeed * (QualitySettings.vSyncCount == 1 ? 1 : 10), ForceMode.Force);

            // Velocity Clamp
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;

            // Y vel fix
            if (rb.velocity.y > 0 && (state != PlayerState.Jumping && Input.GetKey(KeyCode.Space)))
                rb.AddForce(Vector3.down * 30f, ForceMode.Force);
        }
        else rb.useGravity = true;

        // Clamp the Velocity to the Move Speed
        MovementHelp.VelocityClamp(moveSpeed, rb.velocity);

        Debug.Log(state);
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
