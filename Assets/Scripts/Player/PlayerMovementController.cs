using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerMovementController : MonoBehaviour
{
    #region Variables
    [SerializeField] string uiSceneName;

    // p: you can be running and jumping at the same time, right? this state machine just doesn't make much sense because the states are not mutually exclusive.
    public enum PlayerState
    {
        Grounded,
        Jumping,
        Falling,
        Crouching,
        Sliding,
        Running,
    }

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
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [Header("Slopes")]
    [SerializeField] float maxSlopeAngle;

    [Tooltip("1 is default, > 1 is a positive modifier, < 1 is a negative modifier")]
    [Header("Movement Modifiers")]
    public float slideModifier = 1.0f;
    public float slideSpeed;
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
    public float slideDuration;

    [Space()]
    // just because variables have similar types (serializefield, float) does not mean that you should bunch them together. it makes it more confusing because sometimes variables that word in tandem with each other are far apart
    [SerializeField] float jumpTime;
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpCoyoteTime;
    [SerializeField] float jumpBufferTime;
    float jumpBufferTimer;
    // p: the original was: float jumpCoyoteTimeT;
    // T? What does the t stand for carson. you cant just be saying T
    float jumpCoyoteTimer;

    float groundRay = 0.005f;

    CapsuleCollider col;

    [HideInInspector] public bool isRunning;
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

        SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
    }

    void Update()
    {
        // Ground & Wall Detection
        bool isGrounded = CheckCollider(rb.transform.position + ((col.height / 2) + groundRay) * Vector3.down, out RaycastHit ground);

        #region Inputs

        // Inputs
        inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        bool inputting = (inputs != new Vector2(0f, 0f)), isCrouching = (Input.GetKey(KeyCode.LeftControl));
        isRunning = Input.GetKey(KeyCode.LeftShift);

        #endregion

        // Handles x & z movement & camera movement
        #region Movement 

        // Speed

        float moveSpeed = OnSlope() && state == PlayerState.Sliding ? slideSpeed :
                            isRunning ? runningSpeed : walkingSpeed;

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

                    //rb.AddForce(new Vector3(inputs.x, 0f, inputs.y) * slideModifier, ForceMode.Impulse);

                    break;
            }
        }

        stateDur += Time.deltaTime;

        jumpBufferTimer = Mathf.MoveTowards(jumpBufferTimer, 0, Time.deltaTime);
        jumpCoyoteTimer = Mathf.MoveTowards(jumpCoyoteTimer, 0, Time.deltaTime);

        switch (state)
        {
            case (PlayerState.Grounded):

                // jump
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    ChangeState(PlayerState.Jumping);
                }

                if (jumpBufferTimer > 0)
                {
                    jumpBufferTimer = 0f;
                    ChangeState(PlayerState.Jumping);
                }

                jumpCoyoteTimer = jumpCoyoteTime;
                if (!isGrounded && !Input.GetKey(KeyCode.Space))
                {
                    ChangeState(PlayerState.Falling);
                }

                if (isCrouching && isRunning)
                {
                    float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                    if (angle >= maxSlopeAngle) break;

                    ChangeState(PlayerState.Sliding);
                }
                else if (isCrouching)
                {

                    ChangeState(PlayerState.Crouching);
                }


                break;

            case (PlayerState.Jumping):

                if ((stateDur > jumpTime) || (rb.velocity.y < 0) || (!Input.GetKey(KeyCode.Space))) ChangeState(PlayerState.Falling);

                rb.velocity = new Vector3((rb.velocity.x * airModifier), rb.velocity.y, (rb.velocity.z * airModifier));

                break;

            case (PlayerState.Falling):

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    if (prevState == PlayerState.Grounded || jumpCoyoteTimer > 0)
                    {
                        jumpCoyoteTimer = 0;
                        ChangeState(PlayerState.Jumping);

                        break;
                    }

                    jumpBufferTimer = jumpBufferTime;
                }

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

                if(!Input.GetKey(KeyCode.LeftControl))
                {
                    playerCollider.height = originalHeight;
                    ChangeState(PlayerState.Grounded);
                }

                break;

            case (PlayerState.Sliding):

                if (prevState != PlayerState.Crouching){

                    playerCollider.height = crouchHeight;

                }

                Vector3 inputDir = cam.lookRotation.forward * inputs.y + viewPosition.right * inputs.x;
                inputDir = new Vector3(inputDir.x, 0f, inputDir.z);

                if (!OnSlope() || rb.velocity.y > -.1f) {

                    rb.AddForce(inputDir.normalized * (slideModifier - stateDur), ForceMode.VelocityChange);

                }
                else
                {
                    rb.AddForce(SlopeMoveDir(inputDir) * (slideModifier - stateDur), ForceMode.VelocityChange);
                }

                if (((rb.velocity.x < 2 && rb.velocity.x > -2) && (rb.velocity.z < 2 && rb.velocity.z > -2)) || !Input.GetKey(KeyCode.LeftShift) || stateDur > slideDuration)
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

                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    playerCollider.height = originalHeight;
                    ChangeState(PlayerState.Grounded);
                }

                break;
        }

        #endregion

        // Get the move direction of the viewPosition multiplied by the move speed
        moveDir = (cam.lookRotation.forward * inputs.y + viewPosition.right * inputs.x).normalized;
        Vector3 goalVelocity = moveDir * moveSpeed;
        goalVelocity.y = rb.velocity.y;

        // We are now using Vector3.MoveTowards for the accelerating
        rb.velocity = Vector3.MoveTowards(rb.velocity, goalVelocity, acceleration * Time.deltaTime);

        if (OnSlope())
        {
            // Jumping
            if (state == PlayerState.Jumping || Input.GetKey(KeyCode.Space))
            {
                rb.useGravity = true;
                return;
            }

            if (rb.velocity.y > 0 && (inputs.x == 0 || inputs.y == 0))
            {
                rb.useGravity = true;
                return;
            }

            // p: what issue does this fix?
            // Ensure no weird sliding issues
            rb.useGravity = false;

            // Velocity Clamp

            // p: can you please use a consistent formatting? it makes it harder to read if its not consistent. please do the extra line break and curly braces after all if-statements. 
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }

            // Y vel fix
            if (rb.velocity.y > 0 && (state != PlayerState.Jumping && Input.GetKey(KeyCode.Space)))
            {
                rb.AddForce(Vector3.down * (QualitySettings.vSyncCount == 1 ? 100 : 150), ForceMode.Force);
            }
        }
        else
        {
            rb.useGravity = true;
        }

        // Clamp the Velocity to the Move Speed
        Vector3.ClampMagnitude(rb.velocity, moveSpeed);
    }

    void ChangeState(PlayerState newState)
    {
        prevState = state;

        state = newState;

        stateDur = 0f;
    }

    // Handles wall and ground collisions
    #region Collision Detection

    bool CheckCollider(Vector3 pos, out RaycastHit h)
    {
        // p: why is this the equation we are using??
        float rad = col.radius * 0.7f;

        // For each possible collider, get the closest one then return if you're hitting it.
        Collider[] overlapColliders = Physics.OverlapSphere(pos, rad, groundLayer);
        foreach (var collider in overlapColliders)
        {
            if (Physics.Raycast(new(rb.transform.position, (collider is MeshCollider ? (((MeshCollider)collider).ClosestPointMesh(rb.transform.position)) : collider.ClosestPoint(rb.transform.position)) - rb.transform.position), out h, Mathf.Infinity, groundLayer))
            {
                return true;
            }
        }

        h = default;
        return false;
    }

    bool OnSlope()
    {
        if (Physics.Raycast(rb.transform.position, Vector3.down, out slopeHit, col.height * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        // If the angle of the object beneath the player is more than 0, less than the maxSlopeAngle, then it implies you are on a slope.

        return false;
    }

    Vector3 SlopeMoveDir(Vector3 dir)
    {
        return Vector3.ProjectOnPlane(dir, slopeHit.normal).normalized;
    }

    #endregion

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