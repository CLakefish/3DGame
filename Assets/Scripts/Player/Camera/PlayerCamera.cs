using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Orientation")]
    public Transform orientation;
    public Transform playerObj;

    [Header("Mouse Stuff")]
    public float sensX;
    public float sensY;
    public float yOffset;
    Vector2 mouseRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerObj = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse pos
        Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X") * sensX, Input.GetAxis("Mouse Y") * sensY);

        // Mouse stuff
        mouseRotation.x -= mousePos.y;
        mouseRotation.y += mousePos.x;

        // Clamp
        mouseRotation.x = Mathf.Clamp(mouseRotation.x, -90f, 90f);

        // Proper Rotation
        transform.rotation = Quaternion.Euler(mouseRotation);
        orientation.rotation = Quaternion.Euler(0f, mouseRotation.y, 0f);

        // Proper Positioning
        transform.position = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y + yOffset, playerObj.transform.position.z);
    }
}
