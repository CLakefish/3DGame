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

    const float groundRay = 0.001f;
    CapsuleCollider col;

    [Header("VECTORS! OH YEAH!!")] // for use in velocity
    Vector3 newVel;
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
            float rad = col.radius;

            // For each possible collider, get the closest one then return if you're hitting it.
            foreach (var collider in Physics.OverlapSphere(pos, rad, groundLayer))
            {
                if (Physics.Raycast(new(rb.transform.position, (collider is MeshCollider ? (((MeshCollider)collider).ClosestPointMesh(rb.transform.position)) : collider.ClosestPoint(rb.transform.position)) - transform.position), out h, Mathf.Infinity, groundLayer))
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
        #endregion

        // Speed
        float moveSpeed = (Input.GetKey(KeyCode.LeftShift)) ? runningSpeed : walkingSpeed,
              speedIncrease = ((inputting) ? ((isGrounded) ? acceleration.x : acceleration.y) : ((isGrounded) ? deceleration.x : deceleration.y));

        // Get the move direction of the viewPosition multiplied by the move speed
        moveDir = (viewPosition.forward * inputs.y + viewPosition.right * inputs.x) * moveSpeed;

        // Camera Tilting
        CameraTilt(Input.GetKey(KeyCode.LeftShift));

        // Smooth Damp for the win
        newVel = new Vector3(Mathf.SmoothDamp(rb.velocity.x, moveDir.x, ref currentVel.x, speedIncrease), (state == PlayerState.Jumping && stateDur == 0) ? 0f : rb.velocity.y, Mathf.SmoothDamp(rb.velocity.z, moveDir.z, ref currentVel.z, speedIncrease));

        #region State Handler

        if (stateDur == 0)
        {
            switch (state)
            {
                case (PlayerState.Jumping):
                    newVel.y = 0f;
                    break;
            }
        }

        stateDur += Time.deltaTime;

        switch (state)
        {
            case (PlayerState.Grounded):

                if (jumpBufferTimeTemp > 0f || Input.GetKeyDown(KeyCode.Space))
                {
                    jumpBufferTimeTemp = 0f;
                    ChangeState(PlayerState.Jumping);
                }

                if (!isGrounded && !Input.GetKey(KeyCode.Space)) ChangeState(PlayerState.Falling);

                break;

            case (PlayerState.Jumping):

                if (stateDur > jumpTime) ChangeState(PlayerState.Falling);

                if (Input.GetKeyUp(KeyCode.Space) && prevState == PlayerState.Grounded)
                {
                    StartCoroutine(JumpCancel());
                    ChangeState(PlayerState.Falling);
                }

                rb.AddForce(Vector3.up * (jumpSpeed - stateDur), ForceMode.Impulse);

                break;

            case (PlayerState.Falling):

                if (isGrounded) ChangeState(PlayerState.Grounded);

                if (Input.GetKeyDown(KeyCode.Space)) jumpBufferTimeTemp = jumpBufferTime;

                jumpBufferTimeTemp -= Time.deltaTime;

                newVel.y -= .15f * stateDur;

                break;
        }

        #endregion

        newVel.y = Mathf.Clamp(newVel.y, -150f, Mathf.Infinity);

        // Clamp the Velocity to the Move Speed
        MovementHelp.VelocityClamp(moveSpeed, newVel);

        rb.velocity = newVel;

        Debug.Log(isGrounded);
    }

    IEnumerator JumpCancel()
    {
        while (state == PlayerState.Jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Lerp(rb.velocity.y, 0, 10f * Time.deltaTime), rb.velocity.z);
            yield return new WaitForEndOfFrame();
        }
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
