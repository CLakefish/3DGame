using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public OpenObj[] doors;

    // Start is called before the first frame update
    void Start()
    {
        doors = FindObjectsOfType<OpenObj>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (OpenObj i in doors)
        {
            i.isActive = true;
        }
    }
}
