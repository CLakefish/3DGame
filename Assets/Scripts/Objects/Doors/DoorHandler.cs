using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public OpenObj[] doors;
    bool hasOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        //doors = FindObjectsOfType<OpenObj>();
    }

    private void Update()
    {
        if (FindObjectOfType<Enemy>() == null && !hasOpened)
        {
            Open();
            hasOpened = true;
        }
    }

    public void Open(int index = -1)
    {
        if (index != -1)
        {
            doors[index].isActive = true;
            return;
        }
        else
        {
            foreach (OpenObj i in doors)
            {
                i.isActive = true;
            }
        }
    }
}
