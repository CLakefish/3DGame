using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

//[CanEditMultipleObjects]
public class OpenObj : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] public DData[] Door;

    [Header("Variables")]
    public bool isActive;
    public Type doorType;
    public float moveTime;
    public float distance;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = Door[0].pos;
    }

    public void Update()
    {
        StartCoroutine(Open());
    }

    public IEnumerator Open()
    {
        while (isActive)
        {
            for (int i = Door.Length - 1; i > 0; i--)
            {
                Vector3 objectPos = transform.position;

                transform.position = Vector3.Lerp(objectPos, Door[i].pos, moveTime * Time.deltaTime);

                if (Vector3.Distance(transform.position, Door[i].pos) <= distance)
                {
                    switch (doorType)
                    {
                        case (Type.Loop):
                            System.Array.Reverse(Door);
                            break;

                        case (Type.Shut):
                            System.Array.Reverse(Door);
                            isActive = false;
                            break;
                    }

                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

/*
[CanEditMultipleObjects]
[CustomEditor(typeof(OpenObj))]
public class OpenObjEditor : Editor
{
    private void OnSceneGUI()
    {
        OpenObj obj = (OpenObj)target;

        if (obj.Door.Length == 0 || obj == null) return;

        for (int i = 0; i < obj.Door.Length; i++)
        {
            Handles.color = obj.Door[i].color;

            Handles.DrawWireCube(obj.Door[i].pos, obj.transform.lossyScale);

            obj.Door[i].pos = Handles.PositionHandle(obj.Door[i].pos, Quaternion.identity);
        }
    }
    public override void OnInspectorGUI()
    {

        DData[] d = FindObjectOfType<OpenObj>().GetComponent<OpenObj>().Door;

        if (d.Length == 1 && d != null)
            if (GUILayout.Button("Set Pos"))
            {
                GameObject o = FindObjectOfType<OpenObj>().gameObject;

                d[0].pos = o.transform.position + Vector3.up;
            }

        base.OnInspectorGUI();
    }
}
*/
public enum Type
{
    Shut,
    OpenClose,
    Loop
}

[Serializable]
//[CanEditMultipleObjects]
public struct DData
{
    public string Name;
    public Color color;
    public Vector3 pos;
}