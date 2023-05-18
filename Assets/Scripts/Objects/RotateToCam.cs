using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCam : MonoBehaviour
{
    [Header("Follow Type")]
    public bool rotateToCam;

    [Header("Camera")]
    [SerializeField] Transform mainCamera;
    Quaternion originalRotation;
    public float currentIndex;
    float angle;

    [Header("Object Refernces")]
    Vector3 targetPos;
    Vector3 targetDir;

    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.rotation;

        mainCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateToCam) transform.rotation = mainCamera.transform.rotation * originalRotation;
        else
        {
            targetPos = new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z);
            targetDir = targetPos - transform.position;

            angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

            currentIndex = GetPos(angle);
        }
    }

    float GetPos(float angle)
    {
        if (angle > -22.5f && angle < 22.6f)
            return 0;
        if (angle >= 22.5f && angle < 67.5f)
            return 7;
        if (angle >= 67.5f && angle < 112.5f)
            return 6;
        if (angle >= 112.5f && angle < 157.5f)
            return 5;

        if (angle <= -157.5 || angle >= 157.5f)
            return 4;
        if (angle >= -157.4f && angle < -112.5f)
            return 3;
        if (angle >= -112.5f && angle < -67.5f)
            return 2;
        if (angle >= -67.5f && angle <= -22.5f)
            return 1;

        return angle;
    }
}
