using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCam : MonoBehaviour
{
    [Header("Follow Type")]
    public bool rotateToCam;

    [Header("Sprites and Angles")]
    [SerializeField] Transform Sprite;
    [SerializeField] Animator spriteAnim;
    [SerializeField]
    public float backAngle,
                                  sideAngle;
    Vector2 moveDir;

    // Update is called once per frame
    private void Update()
    {
        Vector3 camForward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
        Debug.DrawRay(Camera.main.transform.position, camForward * 5f, Color.green);

        float signedAngle = Vector3.SignedAngle(Sprite.transform.forward, camForward, Vector3.up);
        float angle = Mathf.Abs(signedAngle);

        if (angle < backAngle) moveDir = new Vector2(0f, 1f);
        else if (angle < sideAngle)
        {
            if (signedAngle < 0) moveDir = new Vector2(-1f, 0f);
            else moveDir = new Vector2(1f, 0f);
        }
        else moveDir = new Vector2(0f, -1f);

        //if (spriteAnim == null) return;

        return;
        spriteAnim.SetFloat("Position X", moveDir.x);
        spriteAnim.SetFloat("Position Y", moveDir.y);
    }
}