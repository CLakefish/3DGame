using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Orientation")]
    public Transform orientation;
    public Transform playerObj;
    public PlayerController player;
    internal Camera cam;

    [Header("Mouse Stuff")]
    public float sensX;
    public float sensY;
    public float yOffset,
                 FOV = 75;
    internal float appliedFOV;
    public bool invertY;
    Vector2 mouseRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.fieldOfView = appliedFOV;

        // Get mouse pos
        Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X") * sensX, Input.GetAxis("Mouse Y") * sensY);

        // Mouse stuff
        mouseRotation.x -= mousePos.y;
        mouseRotation.y = (invertY) ? mouseRotation.y -= mousePos.x : mouseRotation.y += mousePos.x;

        // Clamp
        mouseRotation.x = Mathf.Clamp(mouseRotation.x, -90f, 90f);

        // Proper Rotation
        transform.rotation = Quaternion.Euler(new Vector3(mouseRotation.x, mouseRotation.y, transform.rotation.z + player.viewTilt));
        orientation.rotation = Quaternion.Euler(0f, mouseRotation.y, 0f);

        // Proper Positioning
        transform.position = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y + yOffset, playerObj.transform.position.z);
    }

    private void LateUpdate()
    {
        appliedFOV = FOV;
    }
}
