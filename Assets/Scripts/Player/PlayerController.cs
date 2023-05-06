using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class PlayerController : MonoBehaviour
{

    #region Parameters

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

    [Header("Camera Variables")]
    public PlayerCamera Camera;
    public Transform viewPosition;
    internal Vector2 viewTilt;

    [Header("Weapon Information")]
    public WeaponData weaponData;
    public LayerMask layers;
    public GameObject hitParticle;

    [Header("Rigidbody")]
    public Rigidbody rb;

    [Header("Movement Variables")]
    public float walkingSpeed;
    public float runningSpeed;

    public float acceleration,
                 deceleration;

    [Header("VECTORS! OH YEAH!!")]
    Vector3 currentVel;
    Vector3 moveDir;
    Vector2 inputs;

    #endregion

    void Start()
    {
        rb.freezeRotation = true;
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

        // Shooting 
        if (Input.GetMouseButton(0)) ShootObj();

        // Speed
        float moveSpeed = (state == PlayerState.Running) ? runningSpeed : walkingSpeed;
        float speedIncrease = inputting ? acceleration : deceleration;

        // Move Direction
        moveDir = (viewPosition.forward * inputs.y + viewPosition.right * inputs.x) * moveSpeed;

        // Clamp Speed
        ClampVel(moveSpeed);

        CameraTilt();

        #region State Machine

        // On enter
        if (stateDur == 0)
        {
            switch (state)
            {
                case (PlayerState.Walking):

                    break;

                case (PlayerState.Running):

                    break;

            }
        }

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

        rb.velocity = Vector3.SmoothDamp(rb.velocity, moveDir, ref currentVel, speedIncrease);
    }

    void ClampVel(float moveSpeed)
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (vel.magnitude > moveSpeed)
        {
            Vector3 newVel = vel.normalized * moveSpeed;
            rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
        }
    }

    public void ShootObj()
    {
        if (Time.time > weaponData.previousFireTime + weaponData.fireRate)
        {
            weaponData.previousFireTime = Time.time;
            weaponData.isShooting = true;

            StartCoroutine(ProjectileShotHandler());
        }
        else
        {
            weaponData.isShooting = false;
            return;
        }
    }

    IEnumerator ProjectileShotHandler()
    {
        void ShootProjectile()
        {
            Ray ray = Camera.cam.ScreenPointToRay(Input.mousePosition);

            TrailRenderer trail = Instantiate(weaponData.bulletTrail, rb.transform.position, Quaternion.identity);

            RaycastHit raycast;

            ray.direction += new Vector3(Random.Range(-.005f, .005f), Random.Range(-.005f, .005f), Random.Range(-.005f, .005f));

            if (Physics.Raycast(ray, out raycast, Mathf.Infinity, layers))
            {
                Debug.Log("Hit!");

                StartCoroutine(SpawnTrail(trail, raycast,  raycast.point, raycast.normal, weaponData.bounceCount, true));
            }
            else
            {
                StartCoroutine(SpawnTrail(trail, raycast, ray.direction * 100, new Vector3(0f, 0f, 0f), 100, false));
            }
        }

        switch (weaponData.weapon)
        {
            case (Player.WeaponType.Single):
                ShootProjectile();
                break;

            case (Player.WeaponType.Multi):

                for (int i = 0; i < weaponData.currentBulletCount; i++)
                {
                    ShootProjectile();
                    yield return new WaitForSeconds(0.01f);
                }

                break;
        }
    }

    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit raycast, Vector3 point, Vector3 normal, float distance, bool hitObj)
    {
        Vector3 startPos = trail.transform.position;
        Vector3 dir = (point - trail.transform.position).normalized;

        float d = Vector3.Distance(trail.transform.position, point);
        float startDist = d;

        while (d > 0)
        {
            trail.transform.position = Vector3.Lerp(startPos, point, 1 - (d / startDist));
            d -= Time.deltaTime * weaponData.trailSpeed;

            yield return null;
        }

        trail.transform.position = point;

        if (hitObj)
        {
            // Use Object Pooling here eventually 

            GameObject obj = Instantiate(hitParticle, point, Quaternion.LookRotation(normal));
            obj.transform.parent = raycast.collider.gameObject.transform;
            Destroy(obj, 2f);

            if (weaponData.bulletType == BulletType.Bounce && distance > 0)
            {
                Vector3 bounceDir = Vector3.Reflect(dir, normal);

                distance--;

                // Recursion!
                if (Physics.Raycast(point, bounceDir, out RaycastHit h, Mathf.Infinity, layers))
                {
                    yield return StartCoroutine(SpawnTrail(trail, raycast, h.point, h.normal, distance, true));
                }
                else
                {
                    yield return StartCoroutine(SpawnTrail(trail, raycast, bounceDir * distance, new Vector3(0f, 0f, 0f), 0, false));
                }
            }
            else if (distance <= 0) Destroy(trail, trail.time);
        }

        Destroy(trail, .25f);
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
