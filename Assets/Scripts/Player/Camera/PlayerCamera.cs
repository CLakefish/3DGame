using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Orientation")]
    public Transform playerObj;
    PlayerMovementController playerMovementController;
    internal Camera cam;

    [Header("Mouse Stuff")]
    public float sensX;
    public float sensY;
    public float yOffset,
                 FOV = 75;
    float newFOV,
          newWalkingFOV,
          newRunningFOV,
          slidingFOV;

    float shakePos;
    public bool invertY;

    public bool CanRotate = true;
    Vector2 mouseRotation;
    internal Vector2 mousePos;
    [SerializeField] public Transform lookRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        playerMovementController = FindObjectOfType<PlayerMovementController>();
        cam = GetComponent<Camera>();

        newFOV = FOV;
        newWalkingFOV = FOV;
        newRunningFOV = FOV * 1.25f;
        slidingFOV = FOV * 1.4f;
    }

    // Update is called once per frame
    void Update()
    {
        #region FOV

        if (playerMovementController.isRunning && playerMovementController.isCrouching)
        {
            newFOV = Mathf.Lerp(newFOV, slidingFOV, 8 * Time.deltaTime);
        }
        else
        {
            // Change FOV in game
            newFOV = (playerMovementController.isRunning) ?
                newFOV = Mathf.Lerp(newFOV, newRunningFOV, 8 * Time.deltaTime) :
                newFOV = Mathf.Lerp(newFOV, newWalkingFOV, 8 * Time.deltaTime);
        }

        // FOV Change based on FOV 
        cam.fieldOfView = newFOV;

        #endregion

        // Get mouse pos
        mousePos = new Vector2(Input.GetAxis("Mouse X") * sensX, Input.GetAxis("Mouse Y") * sensY);

        // Mouse stuff
        mouseRotation.x -= mousePos.y;
        mouseRotation.y = (invertY) ? mouseRotation.y -= mousePos.x : mouseRotation.y += mousePos.x;

        // Clamp
        mouseRotation.x = Mathf.Clamp(mouseRotation.x, -90f, 90f);

        // Proper Rotation
        Vector3 direction = new Vector3(mouseRotation.x - playerMovementController.viewTilt.y, mouseRotation.y, transform.rotation.z + playerMovementController.viewTilt.x + shakePos);
        
        transform.rotation = Quaternion.Euler(direction);
        lookRotation.rotation = Quaternion.Euler(new Vector3(0, direction.y, direction.z));

        // orientation.rotation = Quaternion.Euler(0f, mouseRotation.y, 0f);

        // Proper Positioning
        transform.position = new Vector3(transform.position.x, playerObj.transform.position.y + yOffset, transform.position.z);
    }

    public void UpdateFOV(float FOV)
    {
        slidingFOV = FOV * 1.4f;
        newFOV = FOV;
        newWalkingFOV = FOV;
        newRunningFOV = FOV * 1.25f;
    }

    public void UpdateSensitivity(float FOV)
    {
        sensX = FOV;
        sensY = FOV;
    }

    public IEnumerator ShakeCamera(float s, float t)
    {
        float time = Time.time;

        Debug.Log("shake");

        while (time + t > Time.time)
        {
            shakePos = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.eulerAngles.z * Random.insideUnitSphere.z * s).z;
            yield return new WaitForEndOfFrame();
        }

        shakePos = 0;
    }
}
