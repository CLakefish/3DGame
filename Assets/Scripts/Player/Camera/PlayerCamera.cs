using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Orientation")]
    public Transform orientation;
    public Transform playerObj;
    public PlayerMovement player;
    internal Camera cam;

    [Header("Mouse Stuff")]
    public float sensX;
    public float sensY;
    public float yOffset,
                 FOV = 75;
    float newFOV,
          newWalkingFOV,
          newRunningFOV;

    public bool invertY;
    bool vertialRecoilPlaying;

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
        #region FOV

        // Change FOV in game
        newFOV = (player.state == PlayerMovement.PlayerState.Running) ? newFOV = Mathf.Lerp(newFOV, newRunningFOV, 8 * Time.deltaTime) : newFOV = Mathf.Lerp(newFOV, newWalkingFOV, 8 * Time.deltaTime);

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
        transform.rotation = Quaternion.Euler(new Vector3(mouseRotation.x - player.viewTilt.y, mouseRotation.y, transform.rotation.z + player.viewTilt.x));
        orientation.rotation = Quaternion.Euler(0f, mouseRotation.y, 0f);

        // Proper Positioning
        transform.position = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y + yOffset, playerObj.transform.position.z);
    }
}
