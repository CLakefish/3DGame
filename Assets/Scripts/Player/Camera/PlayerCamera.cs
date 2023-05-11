using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class PlayerCamera : MonoBehaviour
{
    [Header("Orientation")]
    public Transform orientation;
    public Transform playerObj;
    public PlayerControls player;
    internal Camera cam;

    [Header("Mouse Stuff")]
    public float sensX;
    public float sensY;
    public float yOffset,
                 FOV = 75;
    float newFOV,
          newWalkingFOV,
          newRunningFOV;

    float shakePos;
    public bool invertY;

    public bool CanRotate = true;
    Vector2 mouseRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<Camera>();

        newFOV = FOV;
        newWalkingFOV = FOV;
        newRunningFOV = FOV * 1.25f;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanRotate)
        {
            #region FOV

            // Change FOV in game
            newFOV = (player.isRunning) ? newFOV = Mathf.Lerp(newFOV, newRunningFOV, 8 * Time.deltaTime) : newFOV = Mathf.Lerp(newFOV, newWalkingFOV, 8 * Time.deltaTime);

            // FOV Change based on FOV 
            cam.fieldOfView = newFOV;

            #endregion

            // Get mouse pos
            Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X") * sensX, Input.GetAxis("Mouse Y") * sensY);

            // Mouse stuff
            mouseRotation.x -= mousePos.y;
            mouseRotation.y = (invertY) ? mouseRotation.y -= mousePos.x : mouseRotation.y += mousePos.x;

            // Clamp
            mouseRotation.x = Mathf.Clamp(mouseRotation.x, -90f, 90f);

            // Proper Rotation
            transform.rotation = Quaternion.Euler(new Vector3(mouseRotation.x - player.viewTilt.y, mouseRotation.y, transform.rotation.z + player.viewTilt.x + shakePos));
            orientation.rotation = Quaternion.Euler(0f, mouseRotation.y, 0f);

            // Proper Positioning
            transform.position = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y + yOffset, playerObj.transform.position.z);
        }
    }

    public void UpdateFOV(float FOV)
    {
        newFOV = FOV;
        newWalkingFOV = FOV;
        newRunningFOV = FOV * 1.25f;
    }

    public void UpdateSensitivity(float FOV)
    {
        sensX = FOV;
        sensY = FOV;
    }

    public void ShakeCamera(float s, float t)
    {
        float time = Time.time;

        Debug.Log("shake");

        while (time + t > Time.time)
        {
            shakePos = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.eulerAngles.z * Random.insideUnitSphere.z * s).z;
        }

        shakePos = Mathf.Lerp(shakePos, 0, Time.deltaTime);
    }
}
